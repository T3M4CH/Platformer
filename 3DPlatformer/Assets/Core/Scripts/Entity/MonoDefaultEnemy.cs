using Core.Scripts.Entity;
using Core.Scripts.Healthbars;
using Core.Scripts.StatesMachine;
using UnityEngine;

public class MonoDefaultEnemy : DefaultEntity
{
    [SerializeField] private GameObject weapon;
        
    private HealthbarManager _healthBarManager;

    public override bool TakeDamage(float damage, Vector3? force = null)
    {
        if (!base.TakeDamage(damage))
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

    private void Start()
    {
        StateMachine = new EntityStateMachine();
        var patrolMove = new PatrolMoveEntityState(StateMachine, this, InteractionSystem);
        StateMachine.AddState(patrolMove);
        StateMachine.AddState(new MeleeAttackEntityState(StateMachine, patrolMove, weapon, this));
        StateMachine.AddState(new DamagedEntityState(StateMachine, patrolMove, this, EffectService));
        StateMachine.AddState(new ThrownEntityState(StateMachine, patrolMove, this, EffectService));

        StateMachine.SetState<PatrolMoveEntityState>();
    }

    public override EntityStateMachine StateMachine { get; protected set; }
}