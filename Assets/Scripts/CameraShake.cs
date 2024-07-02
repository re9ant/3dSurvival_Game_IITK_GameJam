using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float amplitude = 3f;
    [SerializeField] float duration = 1f;

    [SerializeField] Transform camHolder;

    [SerializeField] private Vector3 basePos;

    public static CameraShake instance;

    private void Awake()
    {
        basePos = camHolder.localPosition;
        instance = this;
    }

    public void ShakeCamera(float amplitude, float duration)
    {
        StartCoroutine(ShakeCameraCoroutine(amplitude, duration));
        StartCoroutine(ResetCam(duration));
    }

    [ContextMenu("Shake Camera")]
    public void ShakeCamera()
    {
        StartCoroutine(ShakeCameraCoroutine(amplitude, duration));
        StartCoroutine(ResetCam(duration));
    }

    IEnumerator ShakeCameraCoroutine(float amplitude, float duration)
    {
        while(duration > 0)
        {
            duration -= Time.deltaTime;
            Vector3 randomPos = camHolder.position;
            randomPos.x += Random.Range(-amplitude, +amplitude);
            randomPos.y += Random.Range(-amplitude, +amplitude);
            camHolder.position = randomPos;
            yield return null;
        }
        camHolder.localPosition = basePos;
        yield break;
    }

    IEnumerator ResetCam(float duration) 
    {
        yield return new WaitForSeconds(duration +0.05f);
        camHolder.localPosition = basePos;
    }

}
