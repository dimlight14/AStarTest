using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace PathFinding
{
    public interface IPathFinder : IDisposable
    {
        void FindPathRepeatedTest(int repeats, int2 startingPosition, int2 endPosition, int gridSize);
        List<int2> FindSingularPath(int2 startingPosition, int2 endPosition, int gridSize);
    }
}