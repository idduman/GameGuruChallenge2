using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace GameGuruChallenge
{
    public class StackBehaviour : MonoBehaviour
    {
        public event Action Completed;
        public event Action Failed;
        public event Action<StackCube> PlayerMovePointReached;
        public bool Active;

        [SerializeField] private float _fitTreshold = 0.4f;
        [SerializeField] private List<StackCube> _cubes = new();

        private bool _completed;
        private int _currentCubeIndex;
        private float _currentWidth;
        private Tween _cubeTween;
        private StackCube _currentCube;
        private StackCube _nextCube;
        public StackCube NextCube => _nextCube;
        public int CurrentCubeIndex => _currentCubeIndex;
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
            _currentWidth = _currentCube.Width;
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

                if (!_nextCube || _nextCube.State is CubeState.Moving or CubeState.Failed)
                {
                    Failed?.Invoke();
                    return;
                }

                if (_currentCube.State == CubeState.Completed)
                    return;

                _currentCube.State = CubeState.Completed;
                _currentCube = _nextCube;
                _currentWidth = Mathf.Min(_currentWidth, _currentCube.Width);
                _currentCubeIndex++;
                _nextCube = LastCube ? null : _cubes[_currentCubeIndex + 1];
            }
            else if (playerPos.z >= _currentCube.PlayerMovePosZ && _nextCube && _nextCube.State == CubeState.Stopped)
            {
                PlayerMovePointReached?.Invoke(_currentCube);
            }
            else if (playerPos.z >= _currentCube.StartPosZ && _nextCube && _nextCube.State == CubeState.Waiting)
            {
                _nextCube.State = CubeState.Moving;
                
                _nextCube.transform.position -= 6f * Vector3.right;
                var newScale = _nextCube.transform.localScale;
                newScale.x = _currentWidth;
                _nextCube.transform.localScale = newScale;
                
                _nextCube.gameObject.SetActive(true);
                _cubeTween = _nextCube.transform.DOMoveX(_nextCube.CenterPosX + 12f, 1f)
                    .OnComplete(() => _nextCube.State = CubeState.Failed);
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
                var nextPos = _nextCube.transform.position;
                _nextCube.transform.position =
                    new Vector3(_currentCube.CenterPosX, nextPos.y, nextPos.z);
                return;
            }
            if (Mathf.Abs(offset) > _currentCube.Width)
            {
                _nextCube.GetComponent<Rigidbody>().isKinematic = false;
                _nextCube.State = CubeState.Failed;
                return;
            }
            
            _nextCube.transform.position -= offset / 2f * Vector3.right;
            _nextCube.transform.localScale -= Mathf.Abs(offset) * Vector3.right;

            var fallCubeScale = new Vector3(offset, _nextCube.transform.localScale.y, _nextCube.transform.localScale.z);
            var fallCubePos = _nextCube.transform.position
                              + (Mathf.Sign(offset) * _nextCube.Width + offset)/2f * Vector3.right;
            var fallCube = Instantiate(_nextCube, fallCubePos, _nextCube.transform.rotation);
            fallCube.transform.localScale = fallCubeScale;
            fallCube.GetComponent<Rigidbody>().isKinematic = false;
            Destroy(fallCube.gameObject, 2f);
        }
    }
}
