using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameGuruChallenge
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        [SerializeField] private List<LevelBehaviour> _levels;

        private LevelBehaviour _previousLevel;
        private LevelBehaviour _currentLevel;
        private int _playerLevel;

        private void Start()
        {
            Load();
        }

        private void Load()
        {
            if (_levels.Count < 1)
            {
                Debug.LogError("No levels are present in GameManager");
                return;
            }
            StartCoroutine(LoadRoutine());
        }

        private IEnumerator LoadRoutine()
        {

            if (_currentLevel)
                _previousLevel = _currentLevel;
            
            _currentLevel = Instantiate(_levels[_playerLevel % _levels.Count]);
            _currentLevel.Load(_previousLevel);
            yield return new WaitForEndOfFrame();
        }
        
        public void FinishLevel(bool success)
        {
        }

        public void EndLevel(bool success)
        {
            if (success)
            {
                if (_previousLevel)
                {
                    Destroy(_previousLevel.gameObject);
                    _previousLevel = null;
                }
            }
            ++_playerLevel;
            Load();
        }
    }
}
