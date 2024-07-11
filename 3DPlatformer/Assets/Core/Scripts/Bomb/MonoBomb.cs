using Core.Scripts.Entity;
using Core.Sounds.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonoBomb : MonoBehaviour
{
    [SerializeField] private Transform bombModel;
    [SerializeField] private Material[] materials;
    [SerializeField] private Collider bombCollider;
    [SerializeField] private Renderer bombRenderer;
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private SoundAsset explosionSound;

    private int _explodeCount;

    private void Start()
    {
        _explodeCount = Random.Range(1, 4);
        bombRenderer.material = materials[_explodeCount - 1];

        UniTask.Void(async () =>
        {
            bombCollider.enabled = false;
            await UniTask.WaitForSeconds(0.4f, cancellationToken: this.GetCancellationTokenOnDestroy());
            bombCollider.enabled = true;
        });
    }

    private void OnCollisionEnter(Collision other)
    {
        var colliderObject = other.gameObject;

        if (collisionMask.value.Includes(colliderObject.layer))
        {
            if (colliderObject.TryGetComponent(out BaseEntity _))
            {
                Explode();
            }
            else
            {
                _explodeCount -= 1;

                if (_explodeCount <= 0)
                {
                    Explode();
                    return;
                }

                bombRenderer.material = materials[_explodeCount - 1];
                bombModel.DOScale(0.9f, 0.3f).SetEase(Ease.OutBack).SetLink(gameObject);
            }
        }
    }

    private void Explode()
    {
        explosionSound.Play(Random.Range(0.95f, 1.1f));
        bombCollider.enabled = false;
        explosionParticle.SetActive(true);
        explosionParticle.transform.SetParent(null);

        var colliders = Physics.OverlapSphere(transform.position, 0.5f, collisionMask);

        foreach (var collider in colliders)
        {
            collider.GetComponent<IDamageable>()?.TakeDamage(20);
        }

        Destroy(gameObject);
    }
}