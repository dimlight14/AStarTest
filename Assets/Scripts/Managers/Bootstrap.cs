using Hero;
using PathFinding;
using PathFinding.Grid;
using PathFinding.JobsPathFinding;
using UnityEngine;

namespace Managers
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject playerReference;
        [SerializeField] private float playerSpeed;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject wallReference;
        [SerializeField] private GameObject floorReference;
        [SerializeField] private Transform wallParent;
        [SerializeField] private Transform floorParent;
        [SerializeField] private GameObject target;
        [SerializeField] private Game game;

        private void Start()
        {
            var playerFactory = new PlayerFactory(playerReference, playerSpeed);
            var environmentFactory = new EnvironmentFactory(wallReference, wallParent, floorReference, floorParent);
            var inputController = new InputController(mainCamera);
            var gridHolder = new GridHolder();
            var jobPathFinder = new JobPathFinder(gridHolder);
            var usualPathFinder = new PathFinder(new PathFindingAlgorithm(), gridHolder);
            var objectSpawner =
                new ObjectSpawner(gridHolder, playerFactory, environmentFactory, inputController, target);

            game.Initialize(inputController, mainCamera, jobPathFinder, usualPathFinder, objectSpawner);
        }
    }
}