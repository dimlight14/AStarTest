using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Hero
{
    public class PathFollow
    {
        private const float Threshold = 0.07f;
        private readonly Transform _playerTransform;

        private List<int2> _path;
        private int _pathIndex = 0;

        public PathFollow(Transform playerTransform)
        {
            _playerTransform = playerTransform;
        }

        public void SetPath(List<int2> newPath)
        {
            _path = newPath;
            _pathIndex = _path.Count;
        }

        public Vector2 TryGetNexDirection()
        {
            _pathIndex--;
            if (_pathIndex < 0)
            {
                return Vector2.zero;
            }

            var dirX = _path[_pathIndex].x - _playerTransform.position.x;
            var dirY = _path[_pathIndex].y - _playerTransform.position.y;
            var nextDirection = new Vector2(dirX, dirY);
            nextDirection.Normalize();
            return nextDirection;
        }

        public void SnapPosition()
        {
            if (_pathIndex < 0 || _pathIndex >= _path.Count)
            {
                return;
            }

            var currPos = _path[_pathIndex];
            _playerTransform.position = new Vector3(currPos.x, currPos.y, _playerTransform.position.z);
        }

        public bool HasReachedNextGoal(out bool hasReachedEnd)
        {
            if (_pathIndex < 0 || _pathIndex >= _path.Count)
            {
                hasReachedEnd = true;
                return true;
            }

            var playerPos = _playerTransform.position;
            var currGoal = _path[_pathIndex];
            var reachedGoal = Mathf.Abs(playerPos.x - currGoal.x) <= Threshold
                              && Mathf.Abs(playerPos.y - currGoal.y) <= Threshold;

            if (!reachedGoal)
            {
                hasReachedEnd = false;
            }
            else
            {
                hasReachedEnd = _pathIndex == 0;
            }

            return reachedGoal;
        }
    }
}