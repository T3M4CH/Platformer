using Core.Scripts.Entity;
using Core.Scripts.Healthbars;
using Core.Scripts.StatesMachine;
using Reflex.Attributes;
using UnityEngine;

public class MonoDefaultEnemy : BaseEntity
{
    [SerializeField] private MonoInteractionSystem interactionSystem;
    
    private HealthbarManager _healthBarManager;
    
    public override bool TakeDamage(float damage, Vector3? force = null)
    {
        if(!base.TakeDamage(damage)) return false;

        if (force.HasValue)
        {
            StateMachine.SetState<ThrownEntityState>();
            RigidBody.AddForce(force.Value * 5, ForceMode.Impulse);
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

    private void OnCollisionEnter(Collision other)
    {
        if (EntityLayerMask.value.Includes(other.gameObject.layer))
        {
            var damageable = other.gameObject.GetComponent<IDamageable>();
            damageable?.TakeDamage(5);
        }
    }

    private void Start()
    {
        StateMachine = new EntityStateMachine();
        StateMachine.AddState(new PatrolMoveEntityState(StateMachine, this, interactionSystem));
        StateMachine.AddState(new ThrownEntityState(StateMachine, this));
        
        StateMachine.SetState<PatrolMoveEntityState>();
    }

    public override EntityStateMachine StateMachine { get; protected set; }
}