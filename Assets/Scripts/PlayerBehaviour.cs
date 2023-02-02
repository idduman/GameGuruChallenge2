using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGuruChallenge
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _sideSpeed = 1f;
        
        private float _xOffset = 0f;

        private bool _moving;
        public bool Moving
        {
            get => _moving;
            set
            {
                _moving = value;
            }
        }

        void Update()
        {
            if (_moving)
            {
                var pos = transform.position;
                pos.x = _xOffset;
                transform.position = Vector3.Lerp(transform.position, pos, _sideSpeed*Time.deltaTime);
                
                transform.position += _moveSpeed * Time.deltaTime * Vector3.forward;
            }
        }

        public void SetNextCube(StackCube nextCube)
        {
            _xOffset = nextCube.CenterPosX;
        }
    }
}