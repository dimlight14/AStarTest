using UnityEngine;

namespace Hero
{
    public class PlayerMovement
    {
        private readonly Transform _playerTransform;
        private readonly float _speed;

        private Vector2 _currentDirection = new Vector2(1, 0);

        public PlayerMovement(Transform playerTransform, float speed)
        {
            _speed = speed;
            _playerTransform = playerTransform;
        }

        public void SetDirection(Vector2 newDirection)
        {
            _currentDirection = newDirection;
        }

        public void Update(float deltaTime)
        {
            _playerTransform.Translate(_currentDirection * _speed * deltaTime);
        }
    }
}