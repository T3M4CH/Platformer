using UnityEngine.UI;

namespace Core.Scripts.StatesMachine
{
    public class PlayerJumpEntityState : JumpEntityState
    {
        private readonly Button _jumpButton;
        private readonly Button _attackButton;
        private readonly MonoJoystick _joystick;

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

        public override void FixedUpdate()
        {
            _attackButton.interactable = Direction.x != 0 && IsAbleAttack;
            Direction.x = _joystick.Direction.x;

            base.FixedUpdate();
        }

        public override void Exit()
        {
            _attackButton.onClick.RemoveListener(PerformAttack);
            _attackButton.interactable = false;

            base.Exit();
        }
    }
}