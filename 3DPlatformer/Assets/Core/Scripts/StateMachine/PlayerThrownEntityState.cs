using Core.Scripts.Effects.Interfaces;
using Core.Scripts.Entity;

namespace Core.Scripts.StatesMachine
{
    public class PlayerThrownEntityState : ThrownEntityState
    {
        private readonly ControlsWindow _controlsWindow;

        public PlayerThrownEntityState(EntityStateMachine entityStateMachine, EntityState exitState, BaseEntity baseEntity, IEffectService effectService, ControlsWindow controlsWindow) : base(entityStateMachine, exitState, baseEntity, effectService)
        {
            _controlsWindow = controlsWindow;
        }

        public override void Enter()
        {
            base.Enter();

            _controlsWindow.JumpButton.interactable = false;
            _controlsWindow.AttackButton.interactable = false;
        }
    }
}