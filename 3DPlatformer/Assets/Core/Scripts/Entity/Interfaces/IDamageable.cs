using UnityEngine;

public interface IDamageable
{
    bool TakeDamage(float damage, Vector3? force = null);
    Transform transform { get; }
}
