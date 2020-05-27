using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridSpawner : MonoBehaviour
{
    public GameObject blockPrefab;

    public int worldWidth = 10;
    public int worldHeight = 10;
    public float multiply = 1f;
    public float angleSlop = 20;
    public LayerMask layerMask;
    public CharacterControler chara;
    public int nbrOfBlocks;

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
                block.GetComponent<Block>().angleSloap = angleSlop;
                block.GetComponent<Block>().player = chara;
                block.transform.localPosition = new Vector3(x * multiply, 0, z * multiply);


                RaycastHit hit;
                if (Physics.Raycast(block.transform.position, block.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider.gameObject.layer == 9)
                    {
                        block.GetComponent<Block>().positionY = hit.point.y;
                        nbrOfBlocks++;
                    }
                    else
                        Destroy(block);
                    if (hit.collider.gameObject.tag == "Stair")
                    {
                        block.GetComponent<Block>().isStair = true;
                        block.GetComponent<Block>().stairScript = hit.collider.GetComponentInChildren<NavMeshLink>();

                    }
                }
                else
                {
                    Destroy(block);
                }
            }
        }
    }
}
