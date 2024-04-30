using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Scripts.Rats
{
    public class MonoRatsManager : MonoBehaviour
    {
        [SerializeField] private float _startDelay;
        [SerializeField] private float _minInterval;
        [SerializeField] private float _maxInterval;
        [SerializeField] private MonoRatsSpawner[] spawners;

        private bool _isShaked;
        private float _currentCooldown;

        private void Update()
        {
            _startDelay -= Time.deltaTime;

            if (_startDelay > 0) return;

            _currentCooldown -= Time.deltaTime;

            if (_currentCooldown < 1f && !_isShaked)
            {
                _isShaked = true;
                
                foreach (var spawner in spawners)
                {
                    spawner.ShakeWood();
                }
            }

            if (_currentCooldown < 0)
            {
                _isShaked = false;
                
                spawners[Random.Range(0, spawners.Length)].CreateRat();

                _currentCooldown = Random.Range(_minInterval, _maxInterval);
            }
        }

        private void Start()
        {
            _currentCooldown = Random.Range(_minInterval, _maxInterval);
        }
    }
}