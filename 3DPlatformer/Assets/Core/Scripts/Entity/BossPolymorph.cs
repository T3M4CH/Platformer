using System;
using System.Globalization;
using Core.Scripts.Bow;
using Core.Scripts.Entity.Interfaces;
using Core.Scripts.StatesMachine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Core.Scripts.Entity
{
    public class BossPolymorph : BossEntity
    {
        [SerializeField] private GameObject weapon;
        [SerializeField] private BowController bowController;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text statusText2;

        [Button]
        private void TakeDamageTest()
        {
            TakeDamage(5, new Vector3(1, 0, 0));
        }

        public override bool TakeDamage(float damage, Vector3? force = null)
        {
            if (!base.TakeDamage(damage, force))
            {
                weapon.SetActive(false);
                return false;
            }

            if (force.HasValue)
            {
                StateMachine.SetState<ThrownExplodeEntityState>().SetForce(force.Value);
            }
            else
            {
                StateMachine.SetState<DamagedEntityState>();
            }

            return true;
        }

        [Button]
        private void ShodExplode()
        {
            var auraCharger = GetComponent<IAuraCharger>();
            
            StateMachine.SetState<ExplodeEntityState>();
            //auraCharger.Show(transform.position.y + offset, time, false);
            // var prepareEffect = EffectService.GetEffect(EVfxType.ExplosionPrepare, true);
            // prepareEffect.SetPosition(transform.position + new Vector3(0, 1, 0), scale: Vector3.zero);
            // prepareEffect.transform.SetParent(transform);

        }

        private void Update()
        {
            StateMachine.Update();

            statusText.text = StateMachine.CurrentEntityState.GetType().Name;
            //statusText.text = StateMachine.CurrentEntityState?.GetType().Name;
            statusText2.text = StateMachine.PreviousEntityState?.GetType().Name;
        }

        private void LateUpdate()
        {
            var m_CurrentClipInfo = Animator.GetCurrentAnimatorClipInfo(0);
            //Debug.LogWarning(m_CurrentClipInfo.Length + m_CurrentClipInfo[0].clip.name);
        }

        private void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        private void Start()
        {
            StateMachine = new EntityStateMachine();
            var idleState = new BossIdleEntityState(StateMachine, PlayerService.PlayerInstance, this);
            StateMachine.AddState(idleState);
            var moveState = new BossMoveEntityState(StateMachine, this, PlayerService, InteractionSystem);
            StateMachine.AddState(moveState);
            StateMachine.AddState(new MeleeAttackEntityState(StateMachine, moveState, weapon, this));
            StateMachine.AddState(new BossJumpEntityState(StateMachine, moveState, 10, InteractionSystem, this));
            StateMachine.AddState(new JumpAttackEntityState(StateMachine, moveState, this));
            StateMachine.AddState(new BowAttackEntityState(StateMachine, this, bowController, moveState));
            StateMachine.AddState(new DamagedEntityState(StateMachine, moveState, this, EffectService));
            StateMachine.AddState(new ThrownExplodeEntityState(StateMachine, moveState, this, EffectService));
            StateMachine.AddState(new FallEntityState(StateMachine, InteractionSystem, this));
            StateMachine.AddState(new ExplodeEntityState(StateMachine, this, EffectService));

            StateMachine.SetState<BossIdleEntityState>();
        }

        public override EntityStateMachine StateMachine { get; protected set; }
    }
}