// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System;
using Google.OrTools.ConstraintSolver;
using Google.OrTools.LinearSolver;
using static Google.OrTools.ConstraintSolver.RoutingModel.ResourceGroup;

class SPP
{
    static void Main(string[] args)
    {
        // Create the linear solver with the SCIP backend.
        Google.OrTools.LinearSolver.Solver solver = Google.OrTools.LinearSolver.Solver.CreateSolver("SCIP");
        if (solver is null)
        {
            return;
        }

        //Declare parameters

            // c is a cost matrix. now we have 2 optimal solution 0-2-4 or 0-2-3-4
        int[,] c = {
            { 10000, 100,   30, 10000, 10000 },
            { 10000, 10000, 20, 10000, 10000 },
            { 10000, 10000, 10000, 10,  60    },
            { 10000, 15,    10000, 10000, 50  },
            { 10000, 10000, 10000, 10000, 10000 }
        };
            // R is a relation matrix
        int[,] R = {
            { 0, 1, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
            { 0, 0, 0, 1, 1 },
            { 0, 1, 0, 0, 1 },
            { 0, 0, 0, 0, 0 }
        };
        //n is number of nodes, ıI and J are index limits
        int n = c.GetLength(0);
        int I = n;
        int J = n;

        //declare Variables
            //decision variable size
        Variable[,] x = new Variable[I, J];
        for (int i = 0; i < I; i++)
            for (int j = 0; j < J; j++)
            {
                // naming: x_0_0, x_0_1, ...
                x[i, j] = solver.MakeIntVar(0.0, 1.0, $"x_{i}_{j}"); // binary decision varibale ınt and {0,1}
                
            }
        // Print number of variables.
        Console.WriteLine("Number of variables = " + solver.NumVariables());


        //declare Constraints 1
            // \sum_k R[k,i] X[k,i] - \sum_j R[i,j] X[i,j] = 0  forall i in 1..n and .!= {0,n}
        for (int i = 1; i <= n - 2; i++)
        {
            
            var ct = solver.MakeConstraint(0.0, 0.0, $"flow_{i}");

            // in:  + R[k,i] * X[k,i]
            for (int k = 0; k < n; k++)
            {
                if (k == i) continue;                // 
                if (R[k, i] == 0.0) continue;        // optional
                ct.SetCoefficient(x[k, i], R[k, i]);
            }

            // out:  - R[i,j] * X[i,j]
            for (int j = 0; j < n; j++)
            {
                if (j == i) continue;                // köşegen değişkenini atla
                if (R[i, j] == 0.0) continue;
                ct.SetCoefficient(x[i, j], -R[i, j]);
            }
        }


        /*
        *******   Constraint: Source Specification   *******
            Guarantees that the path starts at the designated source node,
            which in this setting is node(0).
        */
        var ct1 = solver.MakeConstraint(1.0, 1.0, $"start");

        for(int j = 0; j < n; j++)
        {
            ct1.SetCoefficient(x[0, j], R[0, j]);
        }

        /*
          *******  Last node Reachability    *******  
            Ensures that the path reaches the designated terminal node, i.e.,
            the final destination of the route.
        */
        var ct2 = solver.MakeConstraint(1.0, 1.0, $"end");
        for (int k = 0; k < n; k++)
        {
            ct2.SetCoefficient(x[k, n - 1], R[k, n - 1]); // array index start 0 so  n-1 
        }


        /*
        ************* Objective Function ******************
            Minimizes the total cost to reach the target node, i.e., it seeks the lowest-cost path
            from the source to the destination.
         */
        Objective obj = solver.Objective();

        for (int i = 0; i < I; i++)
            for (int j = 0; j < J; j++)
            {
                
                    obj.SetCoefficient(x[i, j], c[i, j]);
            }

        obj.SetMinimization(); //minimize objective duncion

        //solve
        Google.OrTools.LinearSolver.Solver.ResultStatus resultStatus = solver.Solve();

        //print result
        if (resultStatus != Google.OrTools.LinearSolver.Solver.ResultStatus.OPTIMAL)
        {
            Console.WriteLine("The problem does not have an optimal solution!");
            return;
        }
        Console.WriteLine("Total cost = " + obj.Value());
        for (int i = 0; i < I; i++)
            for (int j = 0; j < J; j++)
            {
                if (x[i, j].SolutionValue() > 0)
                {
                    Console.WriteLine($"x[{i},{j}] = {x[i, j].SolutionValue()}");
                }
            }






    }
}
