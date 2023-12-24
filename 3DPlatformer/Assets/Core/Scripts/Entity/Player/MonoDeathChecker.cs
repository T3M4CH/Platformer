using Core.Scripts.Entity;
using UnityEngine;

namespace Core.Scripts
{
    public class MonoDeathChecker : MonoBehaviour
    {
        [SerializeField] private BaseEntity baseEntity;
        [SerializeField] private MonoInteractionSystem interactionSystem;

        private Vector3 _lastPosition;
        private LayerMask _waterLayerMask;
        private Transform _transform;

        private void Update()
        {
            if (interactionSystem.IsGround is { Front: true, Under: true })
            {
                _lastPosition = _transform.position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_waterLayerMask.value.Includes(other.gameObject.layer))
            {
                _transform.position = _lastPosition;
            }
        }

        private void Start()
        {
            _transform = transform;
            _waterLayerMask = baseEntity.WaterLayerMask;
        }
    }
}