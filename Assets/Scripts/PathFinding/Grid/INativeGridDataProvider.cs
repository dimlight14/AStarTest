using Unity.Collections;

namespace PathFinding.Grid
{
    public interface INativeGridDataProvider
    {
        NativeArray<GridNode> NativeGridNodes { get; }
    }
}