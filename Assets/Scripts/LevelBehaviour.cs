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
        [SerializeField] private GameObject _finishLine;
        
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
            _stack.PlayerMovePointReached += OnStackPlayerMovePointReached;
            InputController.Pressed += OnPressed;
        }

        private void Unsubscribe()
        {
            _stack.Completed -= OnStackCompleted;
            _stack.Failed -= OnStackFailed;
            _stack.PlayerMovePointReached -= OnStackPlayerMovePointReached;
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
        
        private void OnStackPlayerMovePointReached(StackCube cube)
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
            
            _player.Initialize(transform.position);
            _stack.Initialize();
            Subscribe();
        }

        public void PostFinish()
        {
            _finishLine.gameObject.SetActive(false);
        }


        private void FinishLevel(bool success)
        {
            if (_finished)
                return;
            
            _finished = true;
            InputController.Pressed -= OnPressed;
            StartCoroutine(FinishRoutine(success));
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

        private IEnumerator FinishRoutine(bool success)
        {
            yield return new WaitForSeconds(0.25f);
            _player.Moving = false; 
            if (success)
            {
                _player.Victory();
                yield return new WaitForSeconds(3f);
            }
            else
            {
                _player.Fall();
                yield return new WaitForSeconds(1f);
            }
            GameManager.Instance.FinishLevel(success);
        }
    }
}
