using System;
using System.Collections.Generic;
using System.Linq;
using Hero;
using PathFinding.Grid;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Managers
{
    public class ObjectSpawner : IDisposable
    {
        private readonly IGridHolder _gridHolder;
        private readonly PlayerFactory _playerFactory;
        private readonly EnvironmentFactory _environmentFactory;
        private readonly InputController _inputController;
        private readonly GameObject _target;

        private readonly List<GameObject> _walls = new List<GameObject>();
        private readonly List<GameObject> _floorTiles = new List<GameObject>();
        
        private PlayerController _playerReference;
        private int _gridSize;
        private int2 _targetPosition = new int2(5,5);

        public int2 TargetPosition => _targetPosition;

        public ObjectSpawner(
            IGridHolder gridHolder,
            PlayerFactory playerFactory,
            EnvironmentFactory environmentFactory,
            InputController inputController,
            GameObject target)
        {
            _gridHolder = gridHolder;
            _playerFactory = playerFactory;
            _environmentFactory = environmentFactory;
            _inputController = inputController;
            _target = target;
        }

        public void SetUp(int gridSize)
        {
            
            _gridHolder.GenerateGridBase(new Vector2Int(gridSize, gridSize));
            
            _inputController.OnPlayerClick += SpawnOrMovePlayer;
            _inputController.OnPlayerRightClick += PlayerRightClick;
            _inputController.OnPlayerShiftClick += PlayerShiftClick;
            _gridSize = gridSize;

            _playerReference = _playerFactory.CreatePlayer(Vector2.zero);
            SpawnFloorTiles();
        }
        
        public PlayerController GetPlayer()
        {
            return _playerReference;
        }

        private void PlayerShiftClick(Vector2 position)
        {
            var snappedPosition = SnapPosition(position);
            if (ClickOutOfBounds(snappedPosition))
            {
                return;
            }
            
            _target.transform.position = snappedPosition;
            _targetPosition.x = (int)snappedPosition.x;
            _targetPosition.y = (int)snappedPosition.y;
        }
        private void PlayerRightClick(Vector2 mousePosition)
        {
            var snappedPosition = SnapPosition(mousePosition);
            if (ClickOutOfBounds(snappedPosition))
            {
                return;
            }
            
            var index = Mathf.FloorToInt(snappedPosition.y * _gridSize + snappedPosition.x);
            var wasWalkable = _gridHolder.IsNodeWalkable(index);
            if (wasWalkable)
            {
                _walls.Add(_environmentFactory.CreateWallAt(snappedPosition));
            }
            else
            {
                RemoveWallAt(snappedPosition);
            }

            _gridHolder.SetNodeWalkable(index, !wasWalkable);
        }
        private void SpawnOrMovePlayer(Vector2 newPosition)
        {
            var snappedPosition = SnapPosition(newPosition);
            if (ClickOutOfBounds(snappedPosition))
            {
                return;
            }
            
            _playerReference.SetPosition(snappedPosition);
        }

        private void SpawnFloorTiles()
        {
            var grid = _gridHolder.GridNodes;
            foreach (var gridNode in grid)
            {
                _floorTiles.Add(_environmentFactory.CreateFloorTileAt(new Vector2(gridNode.X, gridNode.Y)));
            }
        }

        private void RemoveWallAt(Vector2 snappedPosition)
        {
            var wallToFind = _walls.FirstOrDefault(wall =>
                Mathf.Approximately(wall.transform.position.x, snappedPosition.x) &&
                Mathf.Approximately(wall.transform.position.y, snappedPosition.y));

            _walls.Remove(wallToFind);
            Object.Destroy(wallToFind);
        }
        
        private Vector2 SnapPosition(Vector2 initialVector)
        {
            return new Vector2(Mathf.Round(initialVector.x), Mathf.Round(initialVector.y));
        }

        private bool ClickOutOfBounds(Vector2 mousePosition)
        {
            return mousePosition.x < 0 || mousePosition.y < 0 || mousePosition.x >= _gridSize ||
                   mousePosition.y >= _gridSize;
        }

        public void Dispose()
        {
            _gridHolder?.Dispose();
        }
    }
}