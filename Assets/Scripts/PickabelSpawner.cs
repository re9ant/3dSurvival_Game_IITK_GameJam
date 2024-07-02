using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PickabelSpawner : MonoBehaviour
{
    public static List<PickabelSpawner> instances = new List<PickabelSpawner>();

    [SerializeField] int spawnCount = 10;
    [SerializeField] GameObject[] pickablePrefabs;
    [SerializeField] List<GameObject> spawnedPrefabs;

    [SerializeField] Vector2 spawnBoxSize;

    private void Start()
    {
        instances.Add(this);
    }

    [ContextMenu("Spawn Prefabs")]
    public void SpawnAtRandom()
    {
        for(int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(transform.position.x, transform.position.x + spawnBoxSize.x), transform.position.y
                                            , Random.Range(transform.position.z, transform.position.z + spawnBoxSize.y));
            int randomInt = Random.Range(0, pickablePrefabs.Length);
            GameObject spawnedObj = Instantiate(pickablePrefabs[randomInt]);
            spawnedPrefabs.Add(spawnedObj);
            spawnedObj.transform.position = randomPos;
            spawnedObj.transform.parent = transform;

            if (Physics.Raycast(spawnedObj.transform.position, Vector3.down, out RaycastHit hitInfo, 50f))
            {
                spawnedObj.transform.position = hitInfo.point + new Vector3(0, 0.1f, 0f);
            }
        }
    }

    public void SpawnSingleItem(GameObject prfb)
    {
        Vector3 randomPos = new Vector3(Random.Range(transform.position.x, transform.position.x + spawnBoxSize.x), transform.position.y
                                            , Random.Range(transform.position.z, transform.position.z + spawnBoxSize.y));
        GameObject spawnedObj = Instantiate(prfb);
        spawnedPrefabs.Add(spawnedObj);
        spawnedObj.transform.position = randomPos;
        spawnedObj.transform.parent = transform;

        if (Physics.Raycast(spawnedObj.transform.position, Vector3.down, out RaycastHit hitInfo, 50f))
        {
            spawnedObj.transform.position = hitInfo.point + new Vector3(0, 0.1f, 0f);
        }
    }
    
    [ContextMenu("Delete Spawn Prefabs")]
    public void DeleteSpawned()
    {
        if (spawnedPrefabs.Count <= 0)
        {
            return;
        }
        while (spawnedPrefabs.Count > 0)
        {
            for(int i = 0; i < spawnedPrefabs.Count; i++)
            {
                GameObject spwn = spawnedPrefabs[i];
                DestroyImmediate(spawnedPrefabs[i].gameObject);
                spawnedPrefabs.Remove(spwn);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.yellow;
        Handles.DrawLine(transform.position, new Vector3(transform.position.x + spawnBoxSize.x, transform.position.y, transform.position.z));
        Handles.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + spawnBoxSize.y));
        Handles.DrawLine(new Vector3(transform.position.x + spawnBoxSize.x, transform.position.y, transform.position.z),
                        new Vector3(transform.position.x  + spawnBoxSize.x, transform.position.y, transform.position.z + spawnBoxSize.y));
        Handles.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z + spawnBoxSize.y)
                        , new Vector3(transform.position.x + spawnBoxSize.x , transform.position.y, transform.position.z + spawnBoxSize.y));
    }
#endif
}
