## Shortest Path Problem - C# & OR-Tools

This console app shows how to formulate a shortest-path problem as an integer linear program using Google OR-Tools. It connects a source node (0) to a sink node (`n - 1`) while minimizing the total travel cost over all available edges.

### Highlights
- **Language / runtime:** C# on .NET 9.0
- **Solver:** Google OR-Tools (SCIP backend)
- **Model:** Model: shortest path problem mathematical model

Two matrices drive the model:
- `c[i,j]` holds the edge cost (use a very large number to forbid an edge).
- `R[i,j]` is the adjacency matrix (1 = edge exists, 0 = no edge).

The solver creates binary variables `x[i,j]` for each potential edge, enforces the constraints, and minimizes `sum(c[i,j] * x[i,j])`. The solution prints the optimal cost and every `x[i,j]` set to 1 (which together describe the path, e.g., `0 -> 2 -> 4`).

### Prerequisites
1. Install [.NET SDK 9.0](https://dotnet.microsoft.com/download).
2. Make sure the Google OR-Tools native dependencies for your platform are available (NuGet pulls them automatically).

### Getting Started
```bash
dotnet restore
dotnet run
```

`dotnet restore` downloads the `Google.OrTools` package. `dotnet run` compiles the app and solves the default matrices in `Program.cs`.

### Customizing the Model
- Update the `c` and `R` arrays in `Program.cs` to represent your own graph. Keep them square and of the same dimension.
- Increase or decrease the node count by resizing both matrices; the code derives `n` from `c.GetLength(0)`, so no other changes are needed.
- Ensure that every edge with `R[i,j] = 1` has a reasonable cost in `c[i,j]`.

### Project Layout
- `Program.cs` - defines the matrices, builds the ILP model, and prints the solution.
- `Shortest-Path-Problem-CSharp.csproj` - targets `net9.0` and references `Google.OrTools`.
- `README.md` - this document.

### Troubleshooting
- **`solver` is null:** SCIP might not be available on your platform. Reinstall OR-Tools or switch to a different backend (e.g., `CBC`).
- **Build or restore fails:** confirm that .NET SDK 9.0 is installed and `dotnet --version` reports 9.x.
- **Unexpected solution:** check that `R` correctly describes allowed edges and that forbidden edges carry a very high cost.

The sample data purposely yields two optimal routes (`0-2-4` and `0-2-3-4`). Feel free to plug in larger graphs or different cost structures to explore how the solver behaves.
