using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEditor.VersionControl;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

public class MonoPlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed;
    [SerializeField] private float health;
    [SerializeField] private float damageCooldown;
    [SerializeField] private float jumpForce;
    [SerializeField] private Animator animator;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private MonoJoystick joystick;
    [SerializeField] private Transform swordTransform;
    [SerializeField] private LayerMask entityLayerMask;
    [SerializeField] private MonoInteractionSystem interactionSystem;

    private bool _isAttacking;
    private float _lastDirection;
    private float _currentCooldownTime;
    private Vector3 _direction;

    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Color1 = Shader.PropertyToID("_BaseColor");
    private static readonly int JoystickOffset = Animator.StringToHash("JoystickOffset");

    public void PerformAttackEvent()
    {
        _isAttacking = false;

        var damageable = Physics.OverlapSphere(swordTransform.position, 2f, entityLayerMask).Select(coll => coll.GetComponent<IDamageable>());

        foreach (var item in damageable)
        {
            item?.TakeDamage(10);
        }
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0 || _currentCooldownTime > 0) return;

        health -= damage;
        _currentCooldownTime = damageCooldown;
        meshRenderer.material.DOColor(Color.red, 0.1f).OnKill(() => meshRenderer.material.SetColor(Color1, Color.white));

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void Move()
    {
        if (_isAttacking) return;

        if (Mathf.Abs(_direction.x) > 0)
        {
            _lastDirection = _direction.x;
        }

        rigidBody.MovePosition(rigidBody.position + _direction * (speed * Time.deltaTime));
        rigidBody.MoveRotation(Quaternion.Euler(0, Math.Sign(_direction.x) * -90, 0));

        animator.SetFloat(JoystickOffset, Mathf.Abs(_direction.x));
    }

    private void PerformJump()
    {
        if (!interactionSystem.IsGround.Under) return;

        rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger(Jump);
    }

    private void PerformAttack()
    {
        if (!interactionSystem.IsGround.Under || _isAttacking) return;

        _isAttacking = true;

        rigidBody.MoveRotation(Quaternion.Euler(0, _lastDirection * -90, 0));
        animator.SetTrigger(Attack);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        _direction.x = joystick.Direction.x;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PerformJump();
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            PerformAttack();
        }

        _currentCooldownTime -= Time.deltaTime;
        _currentCooldownTime = Mathf.Max(0, _currentCooldownTime);
    }

    private void Start()
    {
        jumpButton.onClick.AddListener(PerformJump);
        attackButton.onClick.AddListener(PerformAttack);
    }

    private void OnDestroy()
    {
        jumpButton.onClick.RemoveAllListeners();
        attackButton.onClick.RemoveAllListeners();
    }
}