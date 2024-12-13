using System;
using System.Collections;
using Core.Scripts.Entity.Interfaces;
using Core.Scripts.StatesMachine;
using Core.Sounds.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonoBowArrow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private LayerMask entityLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private SoundAsset explodeSound;
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private GameObject explosionIndicator;

    private Transform _transform;
    private Vector3 _startPosition;

    private void Start()
    {
        _transform = transform;
        _startPosition = _transform.position;
        rigidBody.AddForce(-_transform.up * speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (entityLayerMask.value.Includes(other.gameObject.layer) && other.transform != ArrowOwnwer)
        {
            var damageable = other.gameObject.TryGetComponent(out IPlayerInteractor playerController);

            if (damageable)
            {
                if (playerController.CurrentEntityState is JumpAttackEntityState)
                {
                    playerController.ExecuteExtraJump();

                    Destroy(gameObject);
                    return;
                }
            }

            Explode();
        }

        if (groundLayerMask.value.Includes(other.gameObject.layer))
        {
            rigidBody.isKinematic = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(ExplodeIndicator());
        }
    }

    private void Explode()
    {
        explodeSound.Play(Random.Range(0.95f, 1.1f));
        explosionParticle.SetActive(true);
        explosionParticle.transform.SetParent(null);

        GetComponent<Collider>().enabled = false;
        rigidBody.isKinematic = true;

        var colliders = Physics.OverlapSphere(_transform.position, 1f, entityLayerMask);
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.GetComponent<Collider>().enabled = false;
        sphere.transform.position = _transform.position;
        sphere.transform.localScale = Vector3.one * 2;
        Destroy(sphere, 2);
        foreach (var collider in colliders)
        {
            if (collider.isTrigger) continue;
            collider.GetComponent<IDamageable>()?.TakeDamage(20, 5 * new Vector3(Mathf.Sign(_transform.position.x - _startPosition.x), 1, 0));
        }

        Destroy(gameObject);
    }

    private IEnumerator ExplodeIndicator()
    {
        explosionIndicator.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        explosionIndicator.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        explosionIndicator.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        explosionIndicator.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        Explode();
    }

    public Transform ArrowOwnwer { get; set; }
}