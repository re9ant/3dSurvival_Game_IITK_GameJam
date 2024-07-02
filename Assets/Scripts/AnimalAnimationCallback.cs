using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimationCallback : MonoBehaviour
{
    private Animal animal;

    private void Awake()
    {
        animal = GetComponentInParent<Animal>();
    }

    public void AttackPlayer(float damage)
    {
        if(Vector3.Distance(transform.position, PlayerManager.instance.transform.position) > 3f)
            return;
        if (!animal.isDead)
            PlayerManager.instance.TakeDamage(damage);
    }
}
