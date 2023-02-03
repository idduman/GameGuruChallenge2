using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGuruChallenge
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _notes;
        [SerializeField] private int _maxOctaves = 2;
        [SerializeField] private float _sustain = 0.5f;
        
        private AudioSource _audioSource;
        private int _currentFreqIndex;
        private int _octaves;
        private double _currentFreq;
        private Coroutine _playRoutine;
        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayNote(bool increment)
        {
            if (!_audioSource)
                return;
            
            if(_audioSource.isPlaying)
                _audioSource.Stop();
            
            if (!increment)
            {
                _currentFreqIndex = 0;
                _octaves = 0;
            }
            else
            {
                if (_currentFreqIndex >= _notes.Count - 1)
                {
                    _currentFreqIndex = 0;
                    _octaves = (_octaves + 1) % _maxOctaves;
                }
                else
                {
                    _currentFreqIndex++;
                }
            }

            _audioSource.clip = _notes[_currentFreqIndex];
            _audioSource.pitch = Mathf.Pow(2, _octaves); // 2^octaves
            
            if(_playRoutine != null && _audioSource.isPlaying)
                StopCoroutine(_playRoutine);
            

            _audioSource.Play();
        }

        private IEnumerator PlayRoutine()
        {
            _audioSource.Play();
            yield return new WaitForSeconds(_sustain);
            _audioSource.Stop();
        }
    }
}
