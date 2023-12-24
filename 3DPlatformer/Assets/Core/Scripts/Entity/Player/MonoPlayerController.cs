using Core.Scripts.StatesMachine;
using Core.Scripts.Entity;
using UnityEngine;

public class MonoPlayerController : BaseEntity
{
    [SerializeField] private float damageCooldown;

    private float _currentCooldownTime;

    public override bool TakeDamage(float damage, Vector3? force = null)
    {
        if (StateMachine.CurrentEntityState is JumpAttackEntityState || _currentCooldownTime > 0) return false;

        _currentCooldownTime = damageCooldown;

        return base.TakeDamage(damage);
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    private void Update()
    {
        _currentCooldownTime -= Time.deltaTime;
        _currentCooldownTime = Mathf.Max(0, _currentCooldownTime);

        StateMachine.Update();
    }

    private void Start()
    {
        StateMachine = new EntityStateMachine();
        StateMachine.AddState(new PlayerMoveEntityState(StateMachine, this));
        StateMachine.AddState(new JumpAttackEntityState(StateMachine, this));
        StateMachine.SetState<PlayerMoveEntityState>();
    }

    public override EntityStateMachine StateMachine { get; protected set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public MonoJoystick Joystick { get; private set; }
    [field: SerializeField] public MonoInteractionSystem InteractionSystem { get; private set; }
}