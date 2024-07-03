using Core.Scripts.Bow;
using Core.Scripts.Effects.Interfaces;
using Core.Scripts.StatesMachine;
using Core.Scripts.Entity;
using Core.Scripts.Healthbars;
using Reflex.Attributes;
using UnityEngine;

public class MonoPlayerController : BaseEntity
{
    [SerializeField] private GameObject sword;
    [SerializeField] private BowController bowController;
    [SerializeField] private float damageCooldown;

    private float _currentCooldownTime;
    private ControlsWindow _controlsWindow;

    public void Construct(HealthbarManager healthbarManager, IEffectService effectService, WindowManager windowManager)
    {
        base.Construct(healthbarManager, effectService);

        _controlsWindow = windowManager.GetWindow<ControlsWindow>();
        _controlsWindow.Show();
    }

    public override bool TakeDamage(float damage, Vector3? force = null)
    {
        if (_currentCooldownTime > 0) return false;

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
        var playerMoveState = new PlayerMoveEntityState(StateMachine, this, _controlsWindow);
        StateMachine.AddState(playerMoveState);
        StateMachine.AddState(new JumpEntityState(StateMachine, this, _controlsWindow));
        StateMachine.AddState(new JumpAttackEntityState(StateMachine, this));
        StateMachine.AddState(new MeleeAttackEntityState(StateMachine, playerMoveState, sword, this));
        StateMachine.AddState(new BowAttackEntityState(StateMachine, this, bowController, playerMoveState));
        StateMachine.SetState<PlayerMoveEntityState>();
    }

    public override EntityStateMachine StateMachine { get; protected set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public Transform LookAtPosition { get; private set; }
    [field: SerializeField] public MonoInteractionSystem InteractionSystem { get; private set; }
}