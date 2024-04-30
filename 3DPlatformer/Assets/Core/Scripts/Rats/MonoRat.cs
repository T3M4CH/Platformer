using Core.Scripts.Entity;
using Core.Scripts.StatesMachine;
using UnityEngine;

public class MonoRat : MonoBehaviour, IDamageable
{
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask entityCollisionMask;

    private float _direction;
    private float _remainingTime;
    
    private static readonly int Speed = Animator.StringToHash("Speed");

    public void Initialize(float direction, float duration)
    {
        _direction = direction;
        _remainingTime = duration;
    }

    private void Start()
    {
        animator.SetFloat(Speed, 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        var collObject = other.gameObject;
        if (entityCollisionMask.value.Includes(collObject.layer))
        {
            if (collObject.TryGetComponent(out BaseEntity baseEntity) && baseEntity.StateMachine.CurrentEntityState is not JumpEntityState)
            {
                baseEntity.TakeDamage(10);
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(_direction, 0, 0) * (speed * Time.fixedDeltaTime);

        _remainingTime -= Time.deltaTime;

        if (_remainingTime < 0)
        {
            Destroy(gameObject);
        }
    }

    public bool TakeDamage(float damage, Vector3? force = null)
    {
        Destroy(gameObject);
        return true;
    }
}
