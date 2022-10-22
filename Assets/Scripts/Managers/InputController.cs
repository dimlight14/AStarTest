using System;
using UnityEngine;

namespace Managers
{
    public class InputController
    {
        private readonly Camera _mainCamera;

        public event Action<Vector2> OnPlayerClick; 
        public event Action<Vector2> OnPlayerRightClick; 
        public event Action<Vector2> OnPlayerShiftClick; 
        public event Action OnSpaceBar;

        public InputController(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    OnPlayerShiftClick?.Invoke(mousePos); 
                }
                else
                {
                    OnPlayerClick?.Invoke(mousePos); 
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                var mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                OnPlayerRightClick?.Invoke(mousePos);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnSpaceBar?.Invoke();
            }
        }
    }
}