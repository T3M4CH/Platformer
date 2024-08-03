using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Core.Scripts.StatesMachine
{
    public class PlayerMoveEntityState : MoveEntityState
    {
        private readonly Button _jumpButton;
        private readonly Button _attackButton;
        private readonly MonoJoystick _joystick;

        public PlayerMoveEntityState(EntityStateMachine entityStateMachine, MonoPlayerController baseEntity, MonoInteractionSystem interactionSystem, ControlsWindow controlsWindow) : base(entityStateMachine, baseEntity, interactionSystem)
        {
            _joystick = controlsWindow.Joystick;
            _jumpButton = controlsWindow.JumpButton;
            _attackButton = controlsWindow.AttackButton;
        }

        public override void Enter()
        {
            base.Enter();

            _attackButton.interactable = true;
            _jumpButton.interactable = true;
            _jumpButton.onClick.AddListener(Jump);
            _attackButton.onClick.AddListener(Attack);
        }

        public override void FixedUpdate()
        {
            _jumpButton.interactable = InteractionSystem.IsGround.Under && !IsCloseToEnemy;
            _attackButton.interactable = InteractionSystem.IsGround.Under && Direction.x != 0;

            base.FixedUpdate();
        }

        public override void Update()
        {
            base.Update();

            Direction.x = _joystick.Direction.x;

            if (Keyboard.current.spaceKey.wasPressedThisFrame && _jumpButton.interactable)
            {
                Jump();
            }

            if (Keyboard.current.fKey.wasPressedThisFrame && _attackButton.interactable)
            {
                Attack();
            }
        }

        public override void Exit()
        {
            base.Exit();

            _jumpButton.onClick.RemoveListener(Jump);
            _attackButton.onClick.RemoveListener(Attack);
        }

        public override void Dispose()
        {
            base.Dispose();

            _jumpButton.onClick.RemoveListener(Jump);
            _attackButton.onClick.RemoveListener(Attack);
        }
    }
}