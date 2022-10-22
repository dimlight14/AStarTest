using System.Collections.Generic;
using PathFinding.Grid;
using Unity.Mathematics;

namespace PathFinding
{
    public class PathFindingAlgorithm
    {
        private readonly (int2 offset, int cost)[] _neighboursOffsets = new[]
        {
            (new int2(-1, 0), PathFindingUtility.MoveStraightCost),
            (new int2(1, 0), PathFindingUtility.MoveStraightCost),
            (new int2(0, 1), PathFindingUtility.MoveStraightCost),
            (new int2(0, -1), PathFindingUtility.MoveStraightCost),
            (new int2(-1, 1), PathFindingUtility.MoveDiagonalCost),
            (new int2(1, -1), PathFindingUtility.MoveDiagonalCost),
            (new int2(1, 1), PathFindingUtility.MoveDiagonalCost),
            (new int2(-1, -1), PathFindingUtility.MoveDiagonalCost)
        };
        
        private readonly MinHeap _openList;
        private readonly HashSet<int> _closedList;
        private List<int2> _result;

        public PathFindingAlgorithm()
        {
            _openList = new MinHeap();
            _closedList = new HashSet<int>();
        }

        public List<int2> CalculatePath(IReadOnlyCollection<GridNode> initialGrid, int2 startPosition, int2 endPosition, int gridSize)
        {
            var workingGrid = GenerateWorkingArray(initialGrid, endPosition.x, endPosition.y);

            _openList.Initialize(workingGrid);
            _closedList.Clear();
            if (_result == null)
            {
                _result = new List<int2>(gridSize);
            }
            else
            {
                _result.Clear();
            }

            var endNodeIndex = PathFindingUtility.GetIndex(endPosition.x, endPosition.y, gridSize);
            var startIndex = PathFindingUtility.GetIndex(startPosition.x, startPosition.y, gridSize);
            workingGrid[startIndex].GCost = 0;
            _openList.Enqueue(workingGrid[startIndex]);

            while (_openList.Count > 0)
            {
                var currentNode = _openList.Dequeue();
                var currentNodeIndex = currentNode.Index;

                if (currentNodeIndex == endNodeIndex)
                {
                    GeneratePath(workingGrid, endNodeIndex);
                    break;
                }

                _closedList.Add(currentNodeIndex);

                foreach (var neighbourOffset in _neighboursOffsets)
                {
                    var neighbourPosition = new int2(currentNode.X + neighbourOffset.offset.x,
                        currentNode.Y + neighbourOffset.offset.y);

                    if (!PathFindingUtility.IsPositionInsideGrid(neighbourPosition.x, neighbourPosition.y, gridSize))
                    {
                        continue;
                    }

                    var neighbourNodeIndex =
                        PathFindingUtility.GetIndex(neighbourPosition.x, neighbourPosition.y, gridSize);

                    if (_closedList.Contains(neighbourNodeIndex))
                    {
                        continue;
                    }

                    var neighbourNode = workingGrid[neighbourNodeIndex];
                    if (!neighbourNode.Walkable)
                    {
                        continue;
                    }

                    var newGCost = currentNode.GCost + neighbourOffset.cost;
                    if (newGCost < neighbourNode.GCost)
                    {
                        neighbourNode.ParentIndex = currentNodeIndex;
                        neighbourNode.GCost = newGCost;
                        neighbourNode.ResetFCost();
                        workingGrid[neighbourNodeIndex] = neighbourNode;

                        if (!_openList.Contains(neighbourNode.HeapIndex, neighbourNode.Index))
                        {
                            _openList.Enqueue(neighbourNode);
                        }
                        else
                        {
                            _openList.UpdateItem(neighbourNode);
                        }
                    }
                }
            }
            
            return _result;
        }

        private void GeneratePath(PathFindingNode[] workingArray, int endNodeIndex)
        {
            var prevNode = workingArray[endNodeIndex];
            
            while (prevNode.ParentIndex != -1)
            {
                var nextNode = workingArray[prevNode.ParentIndex];
                _result.Add(new int2(prevNode.X, prevNode.Y));
                prevNode = nextNode;
            }
        }

        private static PathFindingNode[] GenerateWorkingArray(IReadOnlyCollection<GridNode> initialGrid, int endPositionX, int endPositionY)
        {
            var workingArray = new PathFindingNode[initialGrid.Count];
            foreach (var initNode in initialGrid)
            {
                var newPathNode = new PathFindingNode()
                {
                    X = initNode.X,
                    Y = initNode.Y,
                    Index = initNode.Index,
                    Walkable = initNode.IsWalkable,

                    GCost = int.MaxValue,
                    HCost =
                        PathFindingUtility.CalculateDistanceCost(initNode.X, initNode.Y, endPositionX, endPositionY),
                    ParentIndex = -1
                };
                workingArray[newPathNode.Index] = newPathNode;
            }

            return workingArray;
        }
    }
}