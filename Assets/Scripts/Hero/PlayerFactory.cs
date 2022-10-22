using UnityEngine;

namespace Hero
{
    public class PlayerFactory
    {
        private readonly GameObject _playerReference;
        private readonly float _speed;

        public PlayerFactory(GameObject playerReference, float speed)
        {
            _playerReference = playerReference;
            _speed = speed;
        }

        public PlayerController CreatePlayer(Vector2 position)
        {
            var gameObject = Object.Instantiate(_playerReference, position, Quaternion.identity); 
            var playerMovement = new PlayerMovement(gameObject.transform, _speed);
            var pathFollow = new PathFollow(gameObject.transform);
            return new PlayerController(gameObject.transform, playerMovement, pathFollow);
        }
    }
}