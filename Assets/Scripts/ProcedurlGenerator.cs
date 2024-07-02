using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedurlGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabs;
    [SerializeField] List<GameObject> generatedObjects;

    [ContextMenu("Generate World")]
    private void Generate()
    {
        int randomNum = Random.Range(0, prefabs.Count);
        Instantiate(prefabs[randomNum], GetSurroundingGridPos(), Quaternion.identity);

    }

    private Vector3 GetSurroundingGridPos()
    {
        Vector3 resultPos;
        resultPos = transform.position;
        return resultPos;
    }
}
