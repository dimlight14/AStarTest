using System.Collections.Generic;
using PathFinding.Grid;
using Unity.Mathematics;

namespace PathFinding
{
    public class PathFinder : IPathFinder
    {
        private readonly PathFindingAlgorithm _pathFindingAlgorithm;
        private readonly IGridDataProvider _gridDataProvider;

        public PathFinder(PathFindingAlgorithm pathFindingAlgorithm, IGridDataProvider gridDataProvider)
        {
            _pathFindingAlgorithm = pathFindingAlgorithm;
            _gridDataProvider = gridDataProvider;
        }
        public void FindPathRepeatedTest(int repeats, int2 startingPosition, int2 endPosition, int gridSize)
        {
            for (var i = 0; i < repeats; i++)
            {
                _pathFindingAlgorithm.CalculatePath(_gridDataProvider.GridNodes, startingPosition, endPosition, gridSize);
            }
        }

        public List<int2> FindSingularPath(int2 startingPosition, int2 endPosition, int gridSize)
        {
            return _pathFindingAlgorithm.CalculatePath(_gridDataProvider.GridNodes, startingPosition, endPosition, gridSize);
        }
        
        public void Dispose()
        {
        }
    }
}