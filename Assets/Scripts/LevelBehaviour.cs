using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using TMPro;
using UnityEngine;

namespace GameGuruChallenge
{
    public class LevelBehaviour : MonoBehaviour
    {
        [SerializeField] private StackBehaviour _stack;

        private bool _started;
        private bool _finished;
        private PlayerBehaviour _player;

        public float Length => _stack.Length;
        private void Awake()
        {
            _started = false;
            _player = FindObjectOfType<PlayerBehaviour>();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public void Update()
        {
            if (!_started || _finished)
                return;
            
            var playerPos = _player.transform.position;
            
            _stack.UpdateStack(playerPos);
        }

        private void Subscribe()
        {
            _stack.Completed += OnStackCompleted;
            _stack.Failed += OnStackFailed;
            _stack.CubeCenterReached += OnStackCubeCenterReached;
            InputController.Pressed += OnPressed;
        }

        private void Unsubscribe()
        {
            _stack.Completed -= OnStackCompleted;
            _stack.Failed -= OnStackFailed;
            _stack.CubeCenterReached -= OnStackCubeCenterReached;
            InputController.Pressed -= OnPressed;
        }

        private void OnStackFailed()
        {
            FinishLevel(false);
        }

        private void OnStackCompleted()
        {
            FinishLevel(true);
        }
        
        private void OnStackCubeCenterReached(StackCube cube)
        {
            if (_stack.NextCube)
                _player.SetNextCube(_stack.NextCube);
        }

        public void Load(LevelBehaviour previousLevel)
        {
            if (!_player)
            {
                Debug.LogError($"There is no player currently in the scene");
                return;
            }
            if (!_stack)
            {
                Debug.LogError($"No stack controller found in Level: {name}");
                return;
            }
            
            if (previousLevel)
            {
                transform.position = previousLevel.transform.position
                                     + previousLevel.Length * Vector3.forward;
            }
            
            _player.transform.position = transform.position;
            _stack.Initialize();
            Subscribe();
        }

        private void FinishLevel(bool success)
        {
            _finished = true;
            InputController.Pressed -= OnPressed;
            GameManager.Instance.FinishLevel(success);
        }
        
        private void OnPressed(Vector3 pos)
        {
            if (!_started && !_finished)
            {
                _started = true;
                _stack.Active = true;
                _player.Moving = true;
                return;
            }
            
            _stack.StopNextCube();
        }
    }
}
