using Core.Scripts.Bow;
using Core.Scripts.StatesMachine;
using UnityEngine;

namespace Core.Scripts.Entity
{
    public class BossPolymorph : BossEntity
    {
        [SerializeField] private GameObject weapon;
        [SerializeField] private BowController bowController;

        public override bool TakeDamage(float damage, Vector3? force = null)
        {
            if (!base.TakeDamage(damage, force))
            {
                weapon.SetActive(false);
                return false;
            }

            if (force.HasValue)
            {
                StateMachine.SetState<ThrownEntityState>();

                RigidBody.linearVelocity = Vector3.zero;
                RigidBody.AddForce(force.Value, ForceMode.Impulse);
            }
            else
            {
                StateMachine.SetState<DamagedEntityState>();
            }

            return true;
        }

        private void Update()
        {
            StateMachine.Update();
        }

        private void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        protected override void Start()
        {
            base.Start();

            StateMachine = new EntityStateMachine();
            var idleState = new BossIdleEntityState(StateMachine, PlayerService.PlayerInstance, this);
            StateMachine.AddState(idleState);
            var moveState = new BossMoveEntityState(StateMachine, this, PlayerService, InteractionSystem);
            StateMachine.AddState(moveState);
            StateMachine.AddState(new MeleeAttackEntityState(StateMachine, moveState, weapon, this));
            StateMachine.AddState(new BossJumpEntityState(StateMachine, moveState, 5, InteractionSystem, this));
            StateMachine.AddState(new JumpAttackEntityState(StateMachine, moveState, this));
            StateMachine.AddState(new BowAttackEntityState(StateMachine, this, bowController, idleState));
            StateMachine.AddState(new DamagedEntityState(StateMachine, moveState, this, EffectService));
            StateMachine.AddState(new ThrownEntityState(StateMachine, moveState, this, EffectService));

            StateMachine.SetState<BossIdleEntityState>();
        }

        public override EntityStateMachine StateMachine { get; protected set; }
    }
}