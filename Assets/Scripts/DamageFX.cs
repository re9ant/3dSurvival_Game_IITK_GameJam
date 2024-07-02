using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageFX : MonoBehaviour
{
    public static DamageFX instance;

    [SerializeField] Color damageColo = Color.red;
    [SerializeField] Image damageRef;

    private void Awake()
    {
        instance = this;
    }


    public void ToogleDamageFX(float duration)
    {
        damageRef.color = damageColo;
        StartCoroutine(StartFX(duration));
    }

    [ContextMenu("Test Damage")]
    public void ToogleDamageFX()
    {
        damageRef.color = damageColo;
        StartCoroutine(StartFX(5f));
    }

    IEnumerator StartFX(float duration)
    {
        float elapsedTime = 0f;
        Color initialColor = damageRef.color;

        while (elapsedTime < duration)
        {
            float alpha = 1 - (elapsedTime / duration);
            Color updatedColor = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            damageRef.color = updatedColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        damageRef.color = new Color(0, 0, 0, 0);
    }

}
