using UnityEngine;

namespace Core.Scripts
{
    public class MonoDeathChecker : MonoBehaviour
    {
        [SerializeField] private MonoInteractionSystem interactionSystem;

        private Vector3 _lastPosition;
        private Transform _transform;

        private void Update()
        {
            if (interactionSystem.IsGround is { Front: true, Under: true })
            {
                _lastPosition = _transform.position;
            }

            if (_transform.position.y < -5)
            {
                _transform.position = _lastPosition;
            }
        }

        private void Start()
        {
            _transform = transform;
        }
    }
}