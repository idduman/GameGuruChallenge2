using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameGuruChallenge
{
    public class Collectible : MonoBehaviour , ICollectible
    {
        [SerializeField] private float _rotationSpeed = 30f;
        private bool _collected;
        public bool Collected => _collected;
        private ParticleSystem _particle;

        private void Awake()
        {
            _particle = GetComponentInChildren<ParticleSystem>();
        }

        private void Update()
        {
            transform.RotateAround(transform.position, Vector3.up, _rotationSpeed * Time.deltaTime);
        }
        
        public void Collect()
        {
            if (Collected)
                return;
            
            _collected = true;
            GetComponent<Renderer>().enabled = false;
            if(_particle)
                _particle.Play();
            
            Destroy(gameObject, 3f);
        }
    }
}

