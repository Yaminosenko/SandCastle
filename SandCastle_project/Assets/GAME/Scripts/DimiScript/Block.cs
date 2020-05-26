using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Block : MonoBehaviour
{
    [Header("SetUp")]
    public LayerMask layerMask;
    public LayerMask blockMask;
    public LayerMask obstacleMask;
    public Material mouseNotOver;
    public Material mouseOver;
    public GameObject cover;


    [Header("Reference")]
    public Transform[] offset = new Transform[4];
    public GameObject[] blockAdjacent = new GameObject[10];
    public GameObject[] limitsLine = new GameObject[4]; 
    public CharacterControler player;
    public bool inside;
    public float angleSloap;
    public float positionY;
    public int test;
    public int one;
    public float[] offsetDistance = new float[4];
    public int pathIndex;
    public MeshRenderer mesh;
    public bool isStair;
    public NavMeshLink stairScript;


    private void Start()
    {
        SetRotationBlock();
        SetCover();
        mesh = GetComponent<MeshRenderer>();
    }

    private void LateUpdate()
    {
        if(one < 2)
        {
            GetAdjacentBlock();
            StairSetUp();
            one++;
        }

        if (player.FreeMode)
            mesh.enabled = true;
        else
            mesh.enabled = false;

        //if (player.isMoving)
        //    ResetIndex();

    }

    private void Update()
    {
        //ChangeColor();
    }


    private void SetRotationBlock()
    {
        for (int i = 0; i < offset.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(offset[i].transform.position, offset[i].transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                // Debug.DrawRay(offset[i].transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow, Mathf.Infinity);


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

    private void SetCover()
    {
        RaycastHit hitForward;
        RaycastHit hitBack;
        RaycastHit hitRight;
        RaycastHit hitLeft;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        if (Physics.Raycast(pos, Vector3.forward,out hitForward, 0.6f, obstacleMask))
        {
            Vector3 instPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z - 0.1f);
            GameObject coverInst = Instantiate(cover, instPos, cover.transform.rotation);
            coverInst.transform.parent = transform;
            coverInst.GetComponent<Cover>().player = player;

            if (hitForward.collider.gameObject.tag == "Obs1")
            {
                coverInst.GetComponent<Cover>().little = true;
            }
            else if(hitForward.collider.gameObject.tag == "Obs2")
            {
                coverInst.GetComponent<Cover>().little = false;
            }
        }
        else if (Physics.Raycast(pos, Vector3.back, out hitBack, 0.6f, obstacleMask))
        {
            Vector3 instPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z + 0.1f);
            GameObject coverInst = Instantiate(cover, instPos, Quaternion.Euler(0, 180, 0));
            coverInst.transform.parent = transform;
            coverInst.GetComponent<Cover>().player = player;

            if (hitBack.collider.gameObject.tag == "Obs1")
            {
                coverInst.GetComponent<Cover>().little = true;
            }
            else if (hitBack.collider.gameObject.tag == "Obs2")
            {
                coverInst.GetComponent<Cover>().little = false;
            }
        }
        else if (Physics.Raycast(pos, Vector3.right, out hitRight, 0.6f, obstacleMask))
        {
            Vector3 instPos = new Vector3(transform.position.x - 0.1f, transform.position.y + 0.5f, transform.position.z);
            GameObject coverInst = Instantiate(cover, instPos, Quaternion.Euler(0, 90, 0));
            coverInst.transform.parent = transform;
            coverInst.GetComponent<Cover>().player = player;

            if (hitRight.collider.gameObject.tag == "Obs1")
            {
                coverInst.GetComponent<Cover>().little = true;
            }
            else if (hitRight.collider.gameObject.tag == "Obs2")
            {
                coverInst.GetComponent<Cover>().little = false;
            }
        }
        else if (Physics.Raycast(pos, Vector3.left, out hitLeft, 0.6f, obstacleMask))
        {
            Vector3 instPos = new Vector3(transform.position.x + 0.1f, transform.position.y + 0.5f, transform.position.z);
            GameObject coverInst = Instantiate(cover, instPos, Quaternion.Euler(0, -90, 0));
            coverInst.transform.parent = transform;
            coverInst.GetComponent<Cover>().player = player;

            if (hitLeft.collider.gameObject.tag == "Obs1")
            {
                coverInst.GetComponent<Cover>().little = true;
            }
            else if (hitLeft.collider.gameObject.tag == "Obs2")
            {
                coverInst.GetComponent<Cover>().little = false;
            }
        }
        //else
        //{
        //    Debug.DrawRay(pos, Vector3.forward * 1.5f, Color.blue, Mathf.Infinity);
        //}

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

    public void PathLimits()
    {
        for (int i = 0; i < 4; i++)
        {
            Collider[] limitsBlocks = Physics.OverlapSphere(offset[i].position, 0.6f, blockMask);

            if (limitsBlocks.Length != 0)
                limitsLine[i].SetActive(false);
            else
                limitsLine[i].SetActive(true);
        }
    }

    public void StairSetUp()
    {
        if (isStair)
        {
            Vector3 pos = transform.TransformPoint(stairScript.endPoint);
            pos.y = 10;
            RaycastHit hit;
            if(Physics.Raycast(pos, Vector3.down, out hit, 100, blockMask))
            {
                blockAdjacent[9] = hit.collider.gameObject;
                hit.collider.GetComponent<Block>().blockAdjacent[9] = this.gameObject;
            }
        }
    }

    private void ResetIndex()
    {

        if(mesh.material != mouseNotOver)
            mesh.material = mouseNotOver;
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

