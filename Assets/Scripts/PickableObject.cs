using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableObject : MonoBehaviour, IPickable
{
    public bool canUse = true;
    [SerializeField] float damage = 20f;
    [SerializeField] float heal = 20f;
    [SerializeField] float speedMultipler = 1.2f;
    [SerializeField] float duration = 10f;
    public bool canPickUp = true;
    private Rigidbody rb;
    [SerializeField] private TrailRenderer tr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.WakeUp();
        rb.isKinematic = false;
        StartCoroutine(WaitForSecondsToDisableRB());
    }

    IEnumerator WaitForSecondsToDisableRB()
    {
        yield return new WaitForSeconds(5f);
        rb.Sleep();
        rb.isKinematic = true;
        tr.enabled = false;
    }

    public void SelectObject()
    {

    }

    public void Pick(Transform target)
    {
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;
        transform.parent = target;
        transform.localPosition = Vector3.zero;
    }
    
    public void Throw(Vector3 force)
    {
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.WakeUp();
        transform.parent = null;
        rb.isKinematic = false;
        tr.enabled= true;
        rb.AddForce(force, ForceMode.Impulse);
        StartCoroutine(DisableTrailRenderer());
    }

    IEnumerator DisableTrailRenderer()
    {
        yield return new WaitForSeconds(0.3f);
        while (rb.velocity.magnitude >= 0.1f)
        {
            yield return null;
        }
        if (rb.velocity.magnitude < 0.1f)
        {
            tr.enabled = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            yield break;
        }
    }

    public void Use()
    {
        if (!canUse)
            return;
        PickabelSpawner.instances[Random.Range(0, PickabelSpawner.instances.Count)].SpawnSingleItem(this.gameObject);
        PlayerManager.instance.ChangeSpeed(speedMultipler, true);
        PlayerManager.instance.AddHealth(heal);
        StartCoroutine(ResetPlayer(duration));
    }

    IEnumerator ResetPlayer(float duration)
    {
        yield return new WaitForSeconds(duration);
        PlayerManager.instance.ChangeSpeed(speedMultipler, false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (tr.enabled)
        {
            if(collision.gameObject.GetComponent<IDamageable>() != null)
            {
                collision.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
            }
            
            if(collision.gameObject.GetComponentInParent<Animal>() != null)
            {
                collision.gameObject.GetComponentInParent<Animal>().WakeUp();
                collision.gameObject.GetComponentInParent<Animal>().TakeDamage(damage);
            }
        }
    }
}
