using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class HeadbobController : MonoBehaviour
{
    [SerializeField] bool enable = true;

    [SerializeField] float amplitude = 1.0f;
    [SerializeField] float frequency = 1.0f;

    [SerializeField] private Transform cam;
    [SerializeField] private Transform camHolder;

    private float toogleSpeed = 5f;

    private Vector3 startPos;
    private CharacterController controller;

    private PlayerManager player;

    private void Awake()
    {
        controller = GetComponentInParent<CharacterController>();
        player = GetComponentInParent<PlayerManager>();
        startPos = cam.transform.localPosition;
    }

    private void Update()
    {
        if (!enable)
            return;
        CheckMotion();
        ResetPosition();
        cam.LookAt(FocusTarget());
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + camHolder.localPosition.y, transform.position.z);
        pos += camHolder.forward * 15f;
        return pos;
    }

    private void CheckMotion()
    {
        float speed = controller.velocity.magnitude;
        if(speed < toogleSpeed) { return; }
        if(!controller.isGrounded) { return; }
        PlayMotion(FootStepMotion());
    }

    private void PlayMotion(Vector3 pos)
    {
        cam.localPosition += pos;
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return pos;
    }

    private void ResetPosition()
    {
        if (cam.localPosition == startPos)
            return;
        cam.localPosition = Vector3.Lerp(cam.localPosition, startPos, Time.deltaTime);
    }
}