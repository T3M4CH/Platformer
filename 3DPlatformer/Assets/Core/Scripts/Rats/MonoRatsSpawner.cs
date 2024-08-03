using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.Rats
{
    public class MonoRatsSpawner : MonoBehaviour
    {
        [SerializeField] private float lifeTime;
        [SerializeField] private MonoRat ratPrefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private Transform[] woodParts;
        [SerializeField, Range(-1, 1)] private float direction;

        private Vector3[] _woodPositions;

        public void CreateRat()
        {
            var ratInstance = Instantiate(ratPrefab, spawnPosition.position, Quaternion.Euler(0, direction * 90, 0));
            ratInstance.transform.SetParent(transform.parent);
            ratInstance.Initialize(direction, lifeTime);
        }

        public void ShakeWood()
        {
            for (var i = 0; i < woodParts.Length; i++)
            {
                var index = i;
                var part = woodParts[i];

                part.DOShakePosition(2, 0.05f).OnComplete(() =>
                {
                    
                    part.position = _woodPositions[index];
                }).SetLink(gameObject);
            }
        }

        private void Start()
        {
            _woodPositions = woodParts.Select(part => part.position).ToArray();
        }
    }
}