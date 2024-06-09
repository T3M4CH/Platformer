using System;
using Core.Scripts.Healthbars;
using Core.Scripts.StatesMachine;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;

namespace Core.Scripts.Entity
{
    public abstract class BaseEntity : MonoBehaviour, IDamageable
    {
        public event Action OnDead = () => { };
        
        [SerializeField] private float maxHealth;
        [SerializeField] private Renderer skinRenderer;
        [SerializeField] private Material dissolvePrefab;

        private float _health;
        private Material _dissolveMaterial;
        private HealthbarManager _healthBarManager;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

        public void Construct(HealthbarManager healthbarManager)
        {
            _health = maxHealth;
            _healthBarManager = healthbarManager;

            _dissolveMaterial = new Material(dissolvePrefab);
            var texture = skinRenderer.material.GetTexture(MainTex);
            _dissolveMaterial.SetTexture(MainTex, texture);
        }

        public virtual bool TakeDamage(float damage, Vector3? force = null)
        {
            if (_health < 0) return false;

            _health -= damage;
            _healthBarManager.UpdateHp(_health, maxHealth, transform, Vector3.up);
            skinRenderer.material.DOColor(Color.red, 0.2f).OnKill(() => skinRenderer.material.SetColor(BaseColor, Color.white));

            if (_health < 0)
            {
                skinRenderer.materials = new[] { _dissolveMaterial };
                Animator.speed = 0;

                RigidBody.isKinematic = true;
                var colliders = GetComponentsInChildren<Collider>();
                foreach (var entityCollider in colliders)
                {
                    entityCollider.enabled = false;
                }

                DOTween.To(t => skinRenderer.material.SetFloat(DissolveAmount, t), 0, 1, 1).SetLink(gameObject).OnKill(() =>
                {
                    Destroy(gameObject);
                });

                enabled = false;
                
                OnDead.Invoke();
                return false;
            }

            return true;
        }

        private void OnDestroy()
        {
            skinRenderer.material.DOKill();
        }
        
        public abstract EntityStateMachine StateMachine { get; protected set; }
        
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public Transform AttackTransform { get; private set; }
        [field: SerializeField] public Rigidbody RigidBody { get; private set; }
        [field: SerializeField] public EntityCollision EntityCollision { get; private set; }
        [field: SerializeField] public MonoAnimatorHelper AnimatorHelper { get; private set; }
        [field: SerializeField] public LayerMask WaterLayerMask { get; private set; }
        [field: SerializeField] public LayerMask EntityLayerMask { get; private set; }
    }
}