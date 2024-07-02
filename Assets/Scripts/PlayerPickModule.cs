using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickModule : MonoBehaviour
{
    [SerializeField] float rayDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Transform cam;
    [SerializeField] float throwForce = 10f;

    [SerializeField] Material outlineMaterial;

    private PickableObject selectedPickable;

    private bool isHoldingPickable = false;

    void Update()
    {
        if (isHoldingPickable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!selectedPickable.canUse)
                {
                    return;
                }
                selectedPickable.Use();
                Destroy(selectedPickable.gameObject);
                selectedPickable = null;
                isHoldingPickable = false;
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo1, 20f);
                selectedPickable.transform.position = cam.position;
                Vector3 dir = cam.forward;
                selectedPickable.Throw(dir * throwForce);
                Material[] mats = selectedPickable.GetComponent<MeshRenderer>().materials;
                System.Array.Resize(ref mats, mats.Length - 1);
                selectedPickable.GetComponent<MeshRenderer>().materials = mats;
                selectedPickable = null;
                isHoldingPickable = false;
            }
            return;
        }

        if (selectedPickable != null)
        {
            Material[] mats = selectedPickable.GetComponent<MeshRenderer>().materials;
            System.Array.Resize(ref mats, mats.Length - 1);
            selectedPickable.GetComponent<MeshRenderer>().materials = mats;
            selectedPickable = null;
        }
#if UNITY_EDITOR
        Debug.DrawRay(cam.position, cam.forward * rayDistance, Color.red);
#endif

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, rayDistance, layerMask))
        {
            MeshRenderer mr = hitInfo.collider.GetComponent<MeshRenderer>();

            if (mr != null && mr.GetComponent<PickableObject>() != null)
            {
                if(selectedPickable == mr.GetComponent<PickableObject>())
                {
                    return;
                }
                Material[] materials = mr.materials;
                System.Array.Resize(ref materials, materials.Length + 1);
                materials[materials.Length - 1] = outlineMaterial;
                mr.materials = materials;
                selectedPickable = mr.GetComponent<PickableObject>();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && selectedPickable != null)
        {
            selectedPickable.Pick(PlayerManager.instance.handHolder);
            isHoldingPickable = true;
        }
    }
}
