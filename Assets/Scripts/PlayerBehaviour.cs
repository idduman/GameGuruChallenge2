using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GameGuruChallenge
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _sideSpeed = 1f;
        [SerializeField] private CinemachineVirtualCamera _victoryCam;
        [SerializeField] private Rigidbody _modelRb;
        
        private float _xOffset = 0f;

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
                if (_moving)
                    _animator.speed = 1f;
            }
        }

        private void Awake()
        {
            _modelRb = GetComponentInChildren<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Collectible") && other.TryGetComponent<ICollectible>(out var collectible))
            {
                collectible.Collect();
            }
        }

        public void Initialize(Vector3 position)
        {
            _victoryCam.enabled = false;
            _modelRb.isKinematic = true;
            transform.position = position + Vector3.forward;
            _modelRb.transform.localPosition = Vector3.zero;
            _modelRb.transform.rotation = Quaternion.identity;
            _animator.Play("Run");
            _animator.speed = 0f;
        }

        public void SetNextCube(StackCube nextCube)
        {
            _previousOffset = transform.position.x;
            _xOffset = nextCube.CenterPosX;
            perc = 0f;
        }

        public void Victory()
        {
            StartCoroutine(VictoryRoutine());
        }
        public void Fall()
        {
            _modelRb.isKinematic = false;
        }

        private IEnumerator VictoryRoutine()
        {
            _victoryCam.enabled = true;
            yield return new WaitForSeconds(0.1f);
            _animator.SetTrigger(Victory1);
            yield return new WaitForSeconds(0.4f);
            var transposer = _victoryCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            if (transposer)
            {
                DOTween.To(() => transposer.m_Heading.m_Bias,
                    x => transposer.m_Heading.m_Bias = x, 360f, 2f)
                    .OnComplete(() => transposer.m_Heading.m_Bias = 0f);
            }
        }
    }
}