using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

using Random = UnityEngine.Random;
using Color = UnityEngine.Color;

public interface IDamageablee
{
    public void TakeDamage(float damage);
}

[RequireComponent(typeof(NavMeshAgent))]
public class Animmal : MonoBehaviour, IDamageable
{
    [SerializeField] GameObject[] deathFx;
    [SerializeField] GameObject deathFood;
    [SerializeField] bool canSleep = false;

    [SerializeField] Slider healthSlider;
    [SerializeField] float health = 100f;
    [SerializeField] private Animator animator;

    [SerializeField] Transform rayCastStartPos;
    [SerializeField] float detectionRadius = 20f;
    [SerializeField] LayerMask playerMask;
    [SerializeField] LayerMask detectionMask;

    NavMeshAgent agent;

    private bool isAttacking = false;
    private bool isSleeping = false;

    private float coolDown = 20f;

    private float coolDownRef;

    public bool isDead = false;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        coolDownRef = coolDown;
        if (canSleep)
        {
            Sleep();
            StartCoroutine(SleepCycle());
        }
        agent = GetComponent<NavMeshAgent>();
        healthSlider.value = health;
        healthSlider.gameObject.SetActive(false);
    }

    IEnumerator SleepCycle()
    {
        yield return new WaitForSeconds(coolDownRef);
        if (coolDown <= 0f)
        {
            coolDown = coolDownRef;
            Sleep();
        }
    }

    private void Sleep()
    {
        isSleeping = !isSleeping;
        animator.SetBool("Sleep", isSleeping);
        StartCoroutine(SleepCycle());
    }

    public void WakeUp()
    {
        isSleeping = false;
        coolDown = coolDownRef;
        animator.SetBool("Sleep", isSleeping);
        StartCoroutine(SleepCycle());
        StartCoroutine(WaitAfterWakeUp());
    }

    IEnumerator WaitAfterWakeUp()
    {
        yield return new WaitForSeconds(1f);
        isSleeping = false;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }
        if (canSleep)
        {
            coolDown -= Time.deltaTime;
            if (isSleeping)
            {
                return;
            }
        }
        Collider[] colliders = Physics.OverlapSphere(rayCastStartPos.position, detectionRadius, playerMask);
        if(colliders.Length > 0 )
        {
#if UNITY_EDITOR
            Debug.DrawRay(colliders[0].transform.position, rayCastStartPos.position - colliders[0].transform.position);
#endif
            //if (Vector3.Angle(agent.velocity, agent.desiredVelocity) > 130f && !agent.isStopped)
            //{
            //    agent.isStopped = true;
            //    return;
            //}
            isAttacking = false;
            if (Physics.Raycast(colliders[0].transform.position, rayCastStartPos.position - colliders[0].transform.position, out RaycastHit hitInfo
                                                                                                            , detectionRadius, detectionMask))
            {
                agent.isStopped = true;
                if (colliders[0].GetComponent<PlayerManager>())
                {
                    agent.isStopped = false;
                    if (Vector3.Angle(agent.velocity, agent.desiredVelocity) > 30f)
                    {
                        transform.Rotate((agent.velocity - agent.desiredVelocity) * Time.deltaTime);
                        //agent.isStopped = true;
                    }
                    agent.SetDestination(colliders[0].transform.position);
                    if (agent.remainingDistance > detectionRadius)
                    {
                        agent.isStopped = true;
                        isAttacking = false;
                    }
                    if (agent.remainingDistance <= 2f)
                    {
                        isAttacking = true;
                        Attack();
                        Quaternion prevRot = transform.rotation;
                        transform.LookAt(PlayerManager.instance.transform.position);
                        transform.rotation = new Quaternion(prevRot.x, transform.rotation.y, 0, transform.rotation.w);
                        //transform.rotation = Quaternion.Euler(new Vector3(prevRot.x, transform.rotation.y, prevRot.z));
                    }
                    else
                    {
                        isAttacking = false;
                    }
                }
            }
            else
            {
                agent.isStopped = false;
                if (Vector3.Angle(agent.velocity, agent.desiredVelocity) > 30f)
                {
                    transform.Rotate((agent.velocity - agent.desiredVelocity)* Time.deltaTime);
                    //agent.isStopped = true;
                }
                agent.SetDestination(colliders[0].transform.position);
                if(agent.remainingDistance <= 2f )
                {
                    isAttacking = true;
                    Attack();
                    Quaternion prevRot = transform.rotation;
                    transform.LookAt(PlayerManager.instance.transform.position);
                    transform.rotation = new Quaternion(prevRot.x, transform.rotation.y, 0, transform.rotation.w);
                    //transform.rotation = Quaternion.Euler(new Vector3(prevRot.x, transform.rotation.y, prevRot.z));
                }
                else
                {
                    isAttacking = false;
                }
            }
        }
        else
        {
            //if (Vector3.Distance(transform.position, startPos) > 10f)
            //{
            //    Debug.Log("Go HOme");
            //    agent.SetDestination(startPos);
            //}
        }
        animator.SetFloat("Speed", agent.velocity.magnitude);
        animator.SetBool("isAttacking", isAttacking);
    }

    private void Attack()
    {
        if (!isDead)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(ToogleHealthbar());
        health -= damage;
        healthSlider.value = health;
        if (health <= 0)
        {
            isDead = true;
            animator.SetBool("Dead", true);
            healthSlider.gameObject.SetActive(false);
            StartCoroutine(StartDeathFX());
            //Destroy(this);
        }
    }

    IEnumerator StartDeathFX()
    {
        yield return new WaitForSeconds(3f);
        GameObject spwnDfx = Instantiate(deathFx[Random.Range(0, deathFx.Length)], transform.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Instantiate(deathFood, PlayerManager.instance.transform.position + new Vector3(0, 5f, 0f), Quaternion.identity);
        Destroy(spwnDfx);
        Instantiate(gameObject, startPos, Quaternion.identity);
        Destroy(gameObject);
    }

    IEnumerator ToogleHealthbar()
    {
        healthSlider.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        healthSlider.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.up, detectionRadius);
    }
#endif

}
