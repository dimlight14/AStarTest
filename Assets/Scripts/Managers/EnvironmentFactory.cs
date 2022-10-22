using UnityEngine;

namespace Managers
{
    public class EnvironmentFactory
    {
        private readonly GameObject _wallReference;
        private readonly Transform _wallParent;
        private readonly GameObject _floorTileReference;
        private readonly Transform _floorParent;

        public EnvironmentFactory(GameObject wallReference,Transform wallParent, GameObject floorTileReference, Transform floorParent)
        {
            _wallReference = wallReference;
            _wallParent = wallParent;
            _floorTileReference = floorTileReference;
            _floorParent = floorParent;
        }

        public GameObject CreateWallAt(Vector2 position)
        {
            return Object.Instantiate(_wallReference, position, Quaternion.identity, _wallParent);
        }
        public GameObject CreateFloorTileAt(Vector2 position)
        {
            return Object.Instantiate(_floorTileReference, position, Quaternion.identity, _floorParent);
        }
    }
}