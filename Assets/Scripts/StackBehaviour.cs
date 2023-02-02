using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameGuruChallenge
{
    public class StackBehaviour : MonoBehaviour
    {
        public event Action Completed;
        public event Action Failed;
        public event Action<StackCube> CubeCenterReached;

        [SerializeField] private float _fitTreshold = 0.4f;
        [SerializeField] private List<StackCube> _cubes = new();
        
        private bool _completed;
        public bool Active;

        private int _currentCubeIndex;
        public int CurrentCubeIndex => _currentCubeIndex;

        private StackCube _currentCube;
        private StackCube _nextCube;
        public StackCube NextCube => _nextCube;

        private Tween _cubeTween;
        private bool LastCube => _currentCubeIndex >= _cubes.Count - 1;

        public float Length => _cubes.Sum(c => c.LengthZ);

        public void Initialize()
        {
            _completed = false;
            
            if (_cubes.Count == 0)
            {
                Debug.LogError($"No cubes are present under controller {name}");
                return;
            }

            _currentCubeIndex = 0;
            _currentCube = _cubes[0];
            _currentCube.State = CubeState.Stopped;
            _nextCube = _cubes[1];

            for (int i = 1; i < _cubes.Count; i++)
            {
                _cubes[i].State = CubeState.Waiting;
                _cubes[i].gameObject.SetActive(false);
            }
        }

        public void UpdateStack(Vector3 playerPos)
        {
            if (!Active)
                return;
            
            if (playerPos.z >= _currentCube.EndPosZ)
            {
                if (LastCube)
                {
                    Completed?.Invoke();
                    return;
                }

                if (!_nextCube)
                {
                    Failed?.Invoke();
                    return;
                }

                if (_currentCube.State == CubeState.Completed)
                    return;

                _currentCube.State = CubeState.Completed;
                _currentCube = _nextCube;
                _currentCubeIndex++;
                _nextCube = LastCube ? null : _cubes[_currentCubeIndex + 1];
            }
            else if (playerPos.z >= _currentCube.CenterPosZ)
            {
                CubeCenterReached?.Invoke(_currentCube);
            }
            else if (playerPos.z >= _currentCube.StartPosZ && _nextCube && _nextCube.State == CubeState.Waiting)
            {
                _nextCube.State = CubeState.Moving;
                _nextCube.transform.position -= 6f * Vector3.right;
                _nextCube.gameObject.SetActive(true);
                _cubeTween = _nextCube.transform.DOMoveX(_nextCube.CenterPosX + 12f, 1f);
            }

        }

        public void StopNextCube()
        {
            if (!_nextCube || _nextCube.State != CubeState.Moving)
                return;

            _nextCube.State = CubeState.Stopped;
            _cubeTween.Kill();
            var offset = _nextCube.CenterPosX - _currentCube.CenterPosX;
            if (Mathf.Abs(offset) < _fitTreshold)
            {
                _nextCube.transform.DOMoveX(_currentCube.CenterPosX, Mathf.Abs(offset)*0.1f);
                return;
            }
            if (Mathf.Abs(offset) > _currentCube.Width)
            {
                _nextCube.GetComponent<Rigidbody>().isKinematic = false;
                _nextCube = null;
                return;
            }
            
            _nextCube.transform.position -= offset / 2f * Vector3.right;
            _nextCube.transform.localScale -= Mathf.Abs(offset) * Vector3.right;

            var fallCubePos = _nextCube.transform.position + (_nextCube.Width/2f * offset * Vector3.right);
            var fallCubeScale = new Vector3(offset, _nextCube.transform.localScale.y, _nextCube.transform.localScale.z);

            var fallCube = Instantiate(_nextCube, fallCubePos, _nextCube.transform.rotation);
            fallCube.transform.localScale = fallCubeScale;
            fallCube.GetComponent<Rigidbody>().isKinematic = false;
            Destroy(fallCube.gameObject, 2f);
        }
    }
}
