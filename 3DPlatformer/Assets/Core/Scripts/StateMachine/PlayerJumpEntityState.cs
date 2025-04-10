using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.StatesMachine
{
    public class PlayerJumpEntityState : JumpEntityState
{
    private readonly Button _jumpButton;
    private readonly Button _attackButton;
    private readonly MonoJoystick _joystick;
    private Coroutine _flipAnimationCoroutine;

    public PlayerJumpEntityState(EntityStateMachine entityStateMachine, EntityState exitState, float jumpForce, MonoInteractionSystem interactionSystem, MonoPlayerController baseEntity, ControlsWindow controlsWindow)
        : base(entityStateMachine, exitState, jumpForce, interactionSystem, baseEntity)
    {
        _joystick = controlsWindow.Joystick;
        _jumpButton = controlsWindow.JumpButton;
        _attackButton = controlsWindow.AttackButton;
    }

    public override void Enter()
    {
        base.Enter();

        _jumpButton.interactable = false;
        _attackButton.interactable = false;

        _attackButton.onClick.AddListener(PerformAttack);
    }

    public void SetFlipAnimation()
    {
        if (_flipAnimationCoroutine != null)
        {
            CoroutineRunner.Instance.StopConcreteCoroutine(_flipAnimationCoroutine);
        }
        _flipAnimationCoroutine = CoroutineRunner.Instance.RunCoroutine(FlipAnimationCoroutine());
    }

    private IEnumerator FlipAnimationCoroutine()
    {
        Animator.CrossFade("JumpBounce", 0f, 0);
        RigidBody.linearVelocity = Vector3.zero;
        RigidBody.angularVelocity = Vector3.zero;
        RigidBody.AddForce(Vector3.up * JumpForce * 0.5f, ForceMode.Impulse);
        
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1f;
    }

    public override void FixedUpdate()
    {
        _attackButton.interactable = Direction.x != 0 && IsAbleAttack;
        Direction.x = _joystick.Direction.x;

        base.FixedUpdate();
    }

    public override void Exit()
    {
        if (_flipAnimationCoroutine != null)
        {
            CoroutineRunner.Instance.StopConcreteCoroutine(_flipAnimationCoroutine);
            _flipAnimationCoroutine = null;
        }
        
        _attackButton.onClick.RemoveListener(PerformAttack);
        _attackButton.interactable = false;

        base.Exit();
    }
}
}