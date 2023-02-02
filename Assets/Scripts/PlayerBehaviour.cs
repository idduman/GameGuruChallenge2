using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace GameGuruChallenge
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _sideSpeed = 1f;
        [SerializeField] private CinemachineVirtualCamera _victoryCam;
        
        private float _xOffset = 0f;
        private Rigidbody _rb;
        private Animator _animator;

        private bool _moving;
        private static readonly int Victory1 = Animator.StringToHash("Victory");
        private float perc;
        private float _previousOffset;

        public bool Moving
        {
            get => _moving;
            set
            {
                _moving = value;
            }
        }

        private void Awake()
        {
            _rb = GetComponentInChildren<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            if (Moving)
            {
                var prevPos = transform.position;
                var pos = prevPos;
                prevPos.x = _previousOffset;
                pos.x = _xOffset;
                if (perc < 1f)
                {
                    transform.position = Vector3.Lerp(prevPos, pos, perc);
                    perc += _sideSpeed * Time.deltaTime;
                }
                
                transform.position += _moveSpeed * Time.deltaTime * Vector3.forward;
            }
        }

        public void Initialize()
        {
            _victoryCam.enabled = false;
            _rb.isKinematic = true;
            transform.position = Vector3.zero;
            _animator.Play("Run");
        }

        public void SetNextCube(StackCube nextCube)
        {
            _previousOffset = transform.position.x;
            _xOffset = nextCube.CenterPosX;
            perc = 0f;
        }

        public void Victory()
        {
            _victoryCam.enabled = true;
            _animator.SetTrigger(Victory1);
        }
        public void Fall()
        {
            _rb.isKinematic = false;
        }
    }
}