using System.Collections.Generic;
using System.Linq;
using PathFinding.Grid;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace PathFinding.JobsPathFinding
{
    public class JobPathFinder : IPathFinder
    {
        private readonly INativeGridDataProvider _gridDataProvider;
        
        private NativeArray<(int2 offset, int cost)> _neighboursOffsets;
        private readonly List<PathFindingAlgorithmJob> _jobsList = new List<PathFindingAlgorithmJob>();

        public JobPathFinder(INativeGridDataProvider gridDataProvider)
        {
            _gridDataProvider = gridDataProvider;

            _neighboursOffsets = new NativeArray<(int2 offset, int cost)>(8, Allocator.Persistent);
            _neighboursOffsets[0] = (new int2(-1, 0), PathFindingUtility.MoveStraightCost);
            _neighboursOffsets[1] = (new int2(1, 0), PathFindingUtility.MoveStraightCost);
            _neighboursOffsets[2] = (new int2(0, 1), PathFindingUtility.MoveStraightCost);
            _neighboursOffsets[3] = (new int2(0, -1), PathFindingUtility.MoveStraightCost);
            _neighboursOffsets[4] = (new int2(-1, 1), PathFindingUtility.MoveDiagonalCost);
            _neighboursOffsets[5] = (new int2(1, -1), PathFindingUtility.MoveDiagonalCost);
            _neighboursOffsets[6] = (new int2(1, 1), PathFindingUtility.MoveDiagonalCost);
            _neighboursOffsets[7] = (new int2(-1, -1), PathFindingUtility.MoveDiagonalCost);
        }

        public void FindPathRepeatedTest(int repeats, int2 startingPosition, int2 endPosition, int gridSize)
        {
            var jobHandleList = new NativeArray<JobHandle>(repeats, Allocator.Temp);
            
            for (var i = 0; i < repeats; i++)
            {
                var pathJob = CreateJob(startingPosition, endPosition, gridSize);
                _jobsList.Add(pathJob);
                var handle = pathJob.Schedule();
                jobHandleList[i] = handle;
            }
            
            JobHandle.CompleteAll(jobHandleList);
            foreach (var job in _jobsList)
            {
                job.Result.Dispose();
            }
            _jobsList.Clear();

            jobHandleList.Dispose();
        }

        public List<int2> FindSingularPath(int2 startingPosition, int2 endPosition, int gridSize)
        {
            var pathJob = CreateJob(startingPosition, endPosition, gridSize);
            var handle = pathJob.Schedule();
            handle.Complete();
            var result = pathJob.Result.ToArray().ToList();
            pathJob.Result.Dispose();
            
            return result;
        }

        private PathFindingAlgorithmJob CreateJob(int2 startingPosition, int2 endPosition, int gridSize)
        {
            return new PathFindingAlgorithmJob
            {
                GridSize = gridSize,
                InitialGrid = _gridDataProvider.NativeGridNodes,
                Neighbours = _neighboursOffsets,
                Result = new NativeList<int2>(gridSize*2, Allocator.TempJob),
                StartPosition = startingPosition,
                EndPosition = endPosition
            };
        }

        public void Dispose()
        {
            _neighboursOffsets.Dispose();
            foreach (var pathFindingJob in _jobsList)
            {
                pathFindingJob.Result.Dispose();
            }
        }
    }
}