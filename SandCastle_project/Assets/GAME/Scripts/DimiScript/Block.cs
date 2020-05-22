using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("SetUp")]
    public LayerMask layerMask;
    public LayerMask blockMask;
    public bool leTest;
    public Material mouseNotOver;
    public Material mouseOver;


    [Header("Reference")]
    public Transform[] offset = new Transform[4];
    public GameObject[] blockAdjacent = new GameObject[10];
    public CharacterControler player;
    public bool inside;
    public float angleSloap;
    public float positionY;
    public int test;
    public int one;
    public float[] offsetDistance = new float[4];
    public int pathIndex;
    private MeshRenderer mesh;


    private void Start()
    {
        SetRotationBlock();
        mesh = GetComponent<MeshRenderer>();
    }

    private void LateUpdate()
    {
        if(one < 2)
        {
            GetAdjacentBlock();
            one++;
        }

        if (player.Mode)
            mesh.enabled = true;
        else
            mesh.enabled = false;


        
    }

    private void Update()
    {
        if (leTest)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                for (int i = 0; i < blockAdjacent.Length; i++)
                {
                    if (blockAdjacent[i] != null) 
                        Destroy(blockAdjacent[i]);
                }
                Destroy(this);
            }
        }

        ChangeColor();
    }


    private void SetRotationBlock()
    {
        for (int i = 0; i < offset.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(offset[i].transform.position, offset[i].transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                // Debug.DrawRay(offset[i].transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow, Mathf.Infinity);
                Debug.Log("Did Hit");

                offsetDistance[i] = hit.distance;

            }
            else
            {
                offsetDistance[i] = 10000f;
            }
        }

        if (offsetDistance[0] == offsetDistance[1] && offsetDistance[1] == offsetDistance[2] && offsetDistance[2] == offsetDistance[3] && offsetDistance[3] == offsetDistance[0])
        {

        }
        else if (offsetDistance[0] > offsetDistance[3])
        {
            transform.rotation = Quaternion.Euler(angleSloap, transform.rotation.y, transform.rotation.z);
            test = 1;
        }

        else if (offsetDistance[3] > offsetDistance[0])
        {
            transform.rotation = Quaternion.Euler(-angleSloap, transform.rotation.y, transform.rotation.z);
            test = 2;
        }

        else if (offsetDistance[1] > offsetDistance[2])
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angleSloap);
            test = 3;
        }

        else if (offsetDistance[2] > offsetDistance[1])
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -angleSloap);
            test = 4;
        }

        //else if (offsetDistance[1] >= offsetDistance[2])
        //    transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -angleSloap);

        transform.position = new Vector3(transform.position.x, positionY, transform.position.z);
    }

    public void GetAdjacentBlock()
    {
        Collider[] AdjacentBlock = Physics.OverlapSphere(transform.position, 1, blockMask);

        for (int i = 0; i < AdjacentBlock.Length; i++)
        {
            if(AdjacentBlock[i] != this.gameObject)
                blockAdjacent[i] = AdjacentBlock[i].gameObject; 
        }
    }

    public void ChangeColor()
    {
    //    if (inside)
    //        mesh.material = mouseOver;
    //    else
    //        mesh.material = mouseNotOver;
        
    }

    private void OnMouseOver()
    {
        //inside = true;
    }

    private void OnMouseExit()
    {
        //inside = false;
    }
}

