using Core.Scripts.Entity;

namespace Core.Scripts.StatesMachine
{
    public class PlayerFallEntityState : FallEntityState
    {
        private readonly MonoJoystick _joystick;
        
        public PlayerFallEntityState(EntityStateMachine entityStateMachine, MonoInteractionSystem interactionSystem, BaseEntity baseEntity, ControlsWindow controlsWindow) : base(entityStateMachine, interactionSystem, baseEntity)
        {
            _joystick = controlsWindow.Joystick;
        }
    }
}