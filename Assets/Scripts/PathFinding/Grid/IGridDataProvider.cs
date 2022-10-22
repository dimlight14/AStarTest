using System.Collections.Generic;

namespace PathFinding.Grid
{
    public interface IGridDataProvider
    {
        IReadOnlyCollection<GridNode> GridNodes { get; }
    }
}