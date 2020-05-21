using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public GameObject blockPrefab;

    public int worldWidth = 10;
    public int worldHeight = 10;
    public float multiply = 1f;
    public LayerMask layerMask;

    //public float spawnSpeed = 0.2f;

    void Start()
    {
        for (int x = 0; x < worldWidth; x++)
        {
            for (int z = 0; z < worldHeight; z++)
            {
               Vector3 test = new Vector3(x * multiply, 0, z * multiply);


                GameObject block = Instantiate(blockPrefab, Vector3.zero, blockPrefab.transform.rotation) as GameObject;
                block.transform.parent = transform;
                block.transform.localPosition = new Vector3(x * multiply, 0, z * multiply);

                RaycastHit hit;
                if (Physics.Raycast(block.transform.position, block.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(block.transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                    Debug.Log("Did Hit");
                }
                else
                {
                    Destroy(block);
                }


            }
        }
    }
}
