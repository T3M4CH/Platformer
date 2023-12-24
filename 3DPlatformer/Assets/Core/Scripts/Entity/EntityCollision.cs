using System;
using UnityEngine;

namespace Core.Scripts.Entity
{
    public class EntityCollision : MonoBehaviour
    {
        public event Action<Collider> TriggerEnter = _ => { };
        public event Action<Collider> TriggerExit = _ => { };
        public event Action<Collider> TriggerStay = _ => { };
        public event Action<Collision> CollisionEnter = _ => { };
        public event Action<Collision> CollisionExit = _ => { };
        public event Action<Collision> CollisionStay = _ => { };

        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExit.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TriggerStay.Invoke(other);
        }

        private void OnCollisionEnter(Collision other)
        {
            CollisionEnter.Invoke(other);
        }

        private void OnCollisionExit(Collision other)
        {
            CollisionExit.Invoke(other);
        }

        private void OnCollisionStay(Collision other)
        {
            CollisionStay.Invoke(other);
        }
        
        [field: SerializeField] public Collider Collider { get; private set; }
    }
}