using PathFinding.Grid;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace PathFinding.JobsPathFinding
{
    [BurstCompile]
    public struct PathFindingAlgorithmJob : IJob
    {
        [ReadOnly] public int GridSize;
        [ReadOnly] public NativeArray<GridNode> InitialGrid;
        [ReadOnly] public NativeArray<(int2 offset, int costToTravel)> Neighbours;
        [ReadOnly] public int2 EndPosition;
        [ReadOnly] public int2 StartPosition;
        public NativeList<int2> Result;

        public void Execute()
        {
            var workingGrid = new NativeArray<PathFindingNode>(InitialGrid.Length, Allocator.Temp);
            GenerateWorkingArray(InitialGrid, workingGrid, EndPosition);

            //I wasn't able to pull of a native version of a binary heap, so I used a simple list. I'm sure there is a way to do this, but I'm already overwhelmed.
            //To be honest, the ineffectiveness of a list search does in many ways negate the benefits of a parallel computing, but thankfully the burst compiler more than compensates it.
            var openList = new NativeList<int>(Allocator.Temp);
            //There's no NativeHashset in this version of Unity, so I had to use this.
            var closedList = new NativeArray<bool>(InitialGrid.Length, Allocator.Temp);
            
            var endNodeIndex = PathFindingUtility.GetIndex(EndPosition.x, EndPosition.y, GridSize);
            var startIndex = PathFindingUtility.GetIndex(StartPosition.x, StartPosition.y, GridSize);
            
            var startNode = workingGrid[startIndex];
            startNode.GCost = 0;
            workingGrid[startIndex] = startNode;
            openList.Add(startIndex);

            while (openList.Length > 0)
            {
                var currentNodeIndex = GetLowestCostFNodeIndex(openList, workingGrid, out var openListIndex);

                if (currentNodeIndex == endNodeIndex)
                {
                    GeneratePath(Result, workingGrid, endNodeIndex);
                    break;
                }

                openList.RemoveAtSwapBack(openListIndex);
                closedList[currentNodeIndex] = true;

                var currentNode = workingGrid[currentNodeIndex];
                
                foreach (var neighbourOffset in Neighbours)
                {
                    var neighbourPosition = new int2(currentNode.X + neighbourOffset.offset.x,
                        currentNode.Y + neighbourOffset.offset.y);

                    if (!PathFindingUtility.IsPositionInsideGrid(neighbourPosition.x, neighbourPosition.y, GridSize))
                    {
                        continue;
                    }

                    var neighbourNodeIndex =
                        PathFindingUtility.GetIndex(neighbourPosition.x, neighbourPosition.y, GridSize);

                    if (closedList[neighbourNodeIndex])
                    {
                        continue;
                    }

                    var neighbourNode = workingGrid[neighbourNodeIndex];
                    if (!neighbourNode.Walkable)
                    {
                        continue;
                    }

                    var newGCost = currentNode.GCost + neighbourOffset.costToTravel;
                    if (newGCost < neighbourNode.GCost)
                    {
                        neighbourNode.ParentIndex = currentNodeIndex;
                        neighbourNode.GCost = newGCost;
                        neighbourNode.ResetFCost();
                        workingGrid[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNodeIndex))
                        {
                            openList.Add(neighbourNodeIndex);
                        }
                    }
                }
            }
            
            closedList.Dispose();
            openList.Dispose();
            workingGrid.Dispose();
        }

        private static void GeneratePath(NativeList<int2> result, NativeArray<PathFindingNode> workingGrid, int endNodeIndex)
        {
            var prevNode = workingGrid[endNodeIndex];
            while (prevNode.ParentIndex != -1)
            {
                var nextNode = workingGrid[prevNode.ParentIndex];
                result.Add(new int2(prevNode.X, prevNode.Y));
                prevNode = nextNode;
            }
        }

        private static int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathFindingNode> pathNodeArray, out int openListIndex)
        {
            openListIndex = 0;
            var lowestCostPathNode = pathNodeArray[openList[0]];
            for (var i = 1; i < openList.Length; i++)
            {
                var testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.CompareTo(lowestCostPathNode) == 1)
                {
                    lowestCostPathNode = testPathNode;
                    openListIndex = i;
                }
            }

            return lowestCostPathNode.Index;
        }

        private static void GenerateWorkingArray(NativeArray<GridNode> initialGrid,
            NativeArray<PathFindingNode> workingGrid, int2 endPosition)
        {
            foreach (var initNode in initialGrid)
            {
                var newPathNode = new PathFindingNode()
                {
                    X = initNode.X,
                    Y = initNode.Y,
                    Index = initNode.Index,
                    Walkable = initNode.IsWalkable,

                    GCost = int.MaxValue,
                    HCost = PathFindingUtility.CalculateDistanceCost(initNode.X, initNode.Y, endPosition.x,
                        endPosition.y),
                    ParentIndex = -1
                };
                workingGrid[newPathNode.Index] = newPathNode;
            }
        }
    }
}