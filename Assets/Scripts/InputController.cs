using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGuruChallenge
{
    public class InputController : SingletonBehaviour<InputController>
    {
        public static event Action<Vector3> Moved; 
        public static event Action<Vector3> Pressed;
        public static event Action<Vector3> Released;
        // Update is called once per frame

        private Vector3 _previousMousePos;

        private void Start()
        {
            _previousMousePos = Input.mousePosition;
        }
        void Update()
        {
            var mousePos = Input.mousePosition;
            
            if (_previousMousePos != mousePos)
            {
                _previousMousePos = mousePos;
                Moved?.Invoke(mousePos);
            }
            if (Input.GetMouseButtonDown(0))
            {
                Pressed?.Invoke(mousePos);
            }
            if (Input.GetMouseButtonUp(0))
            {
                Released?.Invoke(mousePos);
            }
        }
    }

}

