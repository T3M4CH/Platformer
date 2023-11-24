using System;
using Core.Scripts.Healthbars;
using DG.Tweening;
using Reflex.Attributes;
using UnityEngine;

public class MonoDefaultEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed;
    [SerializeField] private float health;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Renderer renderer;
    [SerializeField] private LayerMask entityLayerMask;
    [SerializeField] private MonoInteractionSystem interactionSystem;

    private bool _isDead;
    private float _maxHealth;
    private Vector3 _savedPosition;
    private Vector3 _direction = Vector3.right;
    private HealthbarManager _healthBarManager;

    private static readonly int Color1 = Shader.PropertyToID("_BaseColor");
    private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

    [Inject]
    private void Construct(HealthbarManager healthbarManager)
    {
        _maxHealth = health;
        _healthBarManager =  healthbarManager;
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0) return;

        health -= damage;
        _healthBarManager.UpdateHp(health, _maxHealth, transform, Vector3.up);
        renderer.material.DOColor(Color.red, 0.1f).OnKill(() => renderer.material.SetColor(Color1, Color.white));

        if (health <= 0)
        {
            _isDead = true;
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (!interactionSystem.IsGround.Under)
        {
            rigidBody.position = _savedPosition;
            _direction *= -1;
            rigidBody.MoveRotation(Quaternion.LookRotation(_direction));
        }
        else
        {
            _savedPosition = rigidBody.position;
        }

        rigidBody.MovePosition(rigidBody.position + _direction * (Time.fixedDeltaTime * speed));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (entityLayerMask.value.Includes(other.gameObject.layer))
        {
            var damageable = other.GetComponent<IDamageable>();
            Debug.LogWarning(other.name + " " + damageable);
            damageable?.TakeDamage(5);
        }
    }

    private void Start()
    {
        _savedPosition = rigidBody.position;
        animator.SetFloat(JoystickOffset, 0.5f);
    }

    private void OnDestroy()
    {
        renderer.material.DOKill();
    }
}