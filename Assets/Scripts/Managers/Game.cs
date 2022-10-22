using Hero;
using PathFinding;
using Unity.Mathematics;
using UnityEngine;

namespace Managers
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private int gridSize;
        [SerializeField] private int pathFindingCyclesPerFrame = 100;
        [SerializeField] private bool useJobs;
        [SerializeField] private bool movePlayer;

        private InputController _inputController;
        private IPathFinder _jobPathFinder;
        private IPathFinder _pathFinder;
        private ObjectSpawner _spawner;

        private PlayerController _playerController;
        private bool _findingPath;
        private bool _simulationOn;

        private IPathFinder CurrentPathFinder => useJobs ? _jobPathFinder : _pathFinder;

        public void Initialize(
            InputController inputController,
            Camera mainCamera,
            IPathFinder jobPathFinder,
            IPathFinder pathFinder,
            ObjectSpawner spawner)
        {
            _inputController = inputController;
            _jobPathFinder = jobPathFinder;
            _pathFinder = pathFinder;
            _spawner = spawner;

            mainCamera.orthographicSize = gridSize / 2 + 1;
            mainCamera.transform.position = new Vector3(gridSize / 2, gridSize / 2, -10);

            _inputController.OnSpaceBar += SwitchSimulation;

            _spawner.SetUp(gridSize);
            _playerController = spawner.GetPlayer();
        }

        private void SwitchSimulation()
        {
            _simulationOn = !_simulationOn;

            _playerController.Stop();
            _findingPath = _simulationOn;
        }

        private void Update()
        {
            _inputController.Update();

            if (!_findingPath) return;

            var playerPos = _playerController.GetPosition();
            var startingPosition = new int2((int)playerPos.x, (int)playerPos.y);
            if (movePlayer)
            {
                var path = CurrentPathFinder.FindSingularPath(startingPosition, _spawner.TargetPosition, gridSize);
                _playerController.SetPath(path);
                _findingPath = false;
            }
            else
            {
                CurrentPathFinder.FindPathRepeatedTest(pathFindingCyclesPerFrame, startingPosition,
                    _spawner.TargetPosition,
                    gridSize);
            }
        }

        private void FixedUpdate()
        {
            _playerController.Update(Time.fixedDeltaTime);
        }

        private void OnDestroy()
        {
            _spawner.Dispose();
            _pathFinder.Dispose();
            _jobPathFinder.Dispose();
        }
    }
}