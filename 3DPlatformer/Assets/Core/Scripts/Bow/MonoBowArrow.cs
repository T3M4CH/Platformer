using System.Collections;
using Core.Sounds.Scripts;
using UnityEngine;

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

    private void Start()
    {
        _transform = transform;
        rigidBody.AddForce(-_transform.up * speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (entityLayerMask.value.Includes(other.gameObject.layer) && other.transform != ArrowOwnwer)
        {
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

        var colliders = Physics.OverlapSphere(_transform.position, .5f, entityLayerMask);
        foreach (var collider in colliders)
        {
            collider.GetComponent<IDamageable>()?.TakeDamage(20, new Vector3(Mathf.Sign(collider.transform.position.x - _transform.position.x), 1, 0));
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