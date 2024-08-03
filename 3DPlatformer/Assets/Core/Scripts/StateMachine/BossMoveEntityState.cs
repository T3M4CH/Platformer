using Core.Scripts.Entity.Managers.Interfaces;
using Core.Scripts.Entity;

namespace Core.Scripts.StatesMachine
{
    public class BossMoveEntityState : MoveEntityState
    {
        private MonoPlayerController _playerController;
        private readonly IPlayerService _playerService;

        public BossMoveEntityState(EntityStateMachine entityStateMachine, BossEntity baseEntity, IPlayerService playerService, MonoInteractionSystem interactionSystem) : base(entityStateMachine, baseEntity, interactionSystem)
        {
            _playerService = playerService;
        }

        public override void Enter()
        {
            base.Enter();

            _playerController = _playerService.PlayerInstance;
        }

        public override void Update()
        {
            base.Update();

            var direction = _playerController.transform.position - BaseEntity.transform.position;

            Direction.x = direction.x;
        }
    }
}