using System;
using Core.Scripts.Bow;
using Core.Scripts.Effects.Interfaces;
using Core.Scripts.StatesMachine;
using Core.Scripts.Entity;
using Core.Scripts.Entity.Interfaces;
using Core.Scripts.Healthbars;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonoPlayerController : DefaultEntity, IPlayerInteractor
{
    [SerializeField] private GameObject sword;
    [SerializeField] private BowController bowController;
    [SerializeField] private TMP_Text stateText;
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
        if (_currentCooldownTime > 0)
        {
            Debug.LogWarning("cd > 0 tf");
            return false;
        }

        _currentCooldownTime = damageCooldown;

        if (!base.TakeDamage(damage, force))
        {
            sword.SetActive(false);
            return false;
        }

        if (force.HasValue)
        {
            StateMachine.SetState<ThrownEntityState>().SetForce(force.Value);
        }

        return true;
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    private void Update()
    {
        _currentCooldownTime -= Time.deltaTime;
        _currentCooldownTime = Mathf.Max(0, _currentCooldownTime);

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            Debug.Break();
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            StateMachine.SetState<BowAttackEntityState>();
        }

        StateMachine.Update();

        stateText.text = StateMachine.CurrentEntityState.GetType().Name;
    }

    public void ExecuteExtraJump()
    {
        UniTask.Void(async () =>
        {
            Time.timeScale = 0.5f;

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: this.GetCancellationTokenOnDestroy());

            Time.timeScale = 1f;
        });

        StateMachine.SetState<PlayerJumpEntityState>().SetFlipAnimation();
    }

    private void Start()
    {
        StateMachine = new EntityStateMachine();
        var playerMoveState = new PlayerMoveEntityState(StateMachine, this, InteractionSystem, _controlsWindow);
        StateMachine.AddState(playerMoveState);
        StateMachine.AddState(new PlayerJumpEntityState(StateMachine, playerMoveState, JumpForce, InteractionSystem, this, _controlsWindow));
        StateMachine.AddState(new FallEntityState(StateMachine, InteractionSystem, this));
        StateMachine.AddState(new JumpAttackEntityState(StateMachine, playerMoveState, this));
        StateMachine.AddState(new MeleeAttackEntityState(StateMachine, playerMoveState, sword, this));
        StateMachine.AddState(new BowAttackEntityState(StateMachine, this, bowController, playerMoveState));
        StateMachine.AddState(new ThrownEntityState(StateMachine, playerMoveState, this, EffectService));
        StateMachine.SetState(playerMoveState);
    }

    [Button]
    private void Test()
    {
        var x = StateMachine.SetInheritedState<MoveEntityState>();
        Debug.LogWarning(x);
    }

    public override EntityStateMachine StateMachine { get; protected set; }
    [field: SerializeField] public Transform LookAtPosition { get; private set; }

    public EntityState CurrentEntityState => StateMachine?.CurrentEntityState;
}