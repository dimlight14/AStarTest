using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Hero
{
    public class PlayerController
    {
        private readonly PlayerMovement _playerMovement;
        private readonly PathFollow _pathFollow;
        private readonly Transform _transform;

        private bool _reachedEnd = true;

        public PlayerController(Transform playerTransform, PlayerMovement movement, PathFollow pathFollow)
        {
            _transform = playerTransform;
            _playerMovement = movement;
            _pathFollow = pathFollow;
        }

        public void Update(float deltaTime)
        {
            if (_reachedEnd) return;

            if (_pathFollow.HasReachedNextGoal(out _reachedEnd))
            {
                _pathFollow.SnapPosition();
                if (_reachedEnd)
                {
                    return;
                }

                var nextDirection = _pathFollow.TryGetNexDirection();
                _playerMovement.SetDirection(nextDirection);
            }

            _playerMovement.Update(deltaTime);
        }

        public void SetPosition(Vector3 snappedPosition)
        {
            _transform.position = snappedPosition;
        }

        public Vector2 GetPosition()
        {
            return _transform.position;
        }

        public void SetPath(List<int2> pathList)
        {
            _reachedEnd = false;
            _pathFollow.SetPath(pathList);
            var nextDirection = _pathFollow.TryGetNexDirection();
            _playerMovement.SetDirection(nextDirection);
        }

        public void Stop()
        {
            _reachedEnd = true;
        }
    }
}