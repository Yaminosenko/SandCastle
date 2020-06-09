using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Block : MonoBehaviour
{
    [Header("SetUp")]
    public LayerMask layerMask;
    public LayerMask blockMask;
    public LayerMask obstacleMask;
    public Material mouseNotOver;
    public Material mouseOver;
    public GameObject cover;
    public bool isIndexer;
    public List<GameObject> testList = new List<GameObject>();
    public int testIndex;

    [Header("Reference")]
    public bool isCover;
    public Cover coverScript;
    public Transform[] offset = new Transform[4];
    public GameObject[] blockAdjacent = new GameObject[10];
    public GameObject[] limitsLine = new GameObject[4];
    public CharacterControler player;
    public bool mainCell;
    public bool adjCell;
    public float angleSloap;
    public float positionY;
    public int one;
    public float[] offsetDistance = new float[4];
    public int pathIndex;
    public MeshRenderer mesh;
    public bool isStair;
    public NavMeshLink stairScript;
    public Color colorBaseInArea;
    public Color colorMainInArea;
    public Color colorBaseOutArea;
    public Color colorMainOutArea;


    private Vector3[] lineOffset = new Vector3[4];

    private void Start()
    {
        SetRotationBlock();
        SetCover();
        mesh = GetComponent<MeshRenderer>();
    }

    private void LateUpdate()
    {
        if (one < 3)
        {
            
            GetAdjacentBlock();
            StairSetUp();
            one++;
        }

        if (!player.TacticalMode || player.selectDevice)
            mesh.enabled = false;
            
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    if (isIndexer)
        //        Destroy(gameObject);
        //}
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
        }

        else if (offsetDistance[3] > offsetDistance[0])
        {
            transform.rotation = Quaternion.Euler(-angleSloap, transform.rotation.y, transform.rotation.z);
        }

        else if (offsetDistance[1] > offsetDistance[2])
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angleSloap);
        }

        else if (offsetDistance[2] > offsetDistance[1])
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -angleSloap);
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
        if (Physics.Raycast(pos, Vector3.forward, out hitForward, 0.6f, obstacleMask))
        {
            Vector3 instPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z - 0.1f);
            GameObject coverInst = Instantiate(cover, instPos, cover.transform.rotation);
            coverInst.transform.parent = transform;
            coverInst.GetComponent<Cover>().player = player;
            coverScript = coverInst.GetComponent<Cover>();
            isCover = true;

            if (hitForward.collider.gameObject.tag == "Obs1")
            {
                coverInst.GetComponent<Cover>().little = true;
            }
            else if (hitForward.collider.gameObject.tag == "Obs2")
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
            coverScript = coverInst.GetComponent<Cover>();
            isCover = true;

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
            coverScript = coverInst.GetComponent<Cover>();
            isCover = true;

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
            coverScript = coverInst.GetComponent<Cover>();
            isCover = true;

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
            if (AdjacentBlock[i] != gameObject)
                blockAdjacent[i] = AdjacentBlock[i].gameObject;
        }
    }

    public void PathLimits()
    {
        for (int i = 0; i < 4; i++)
        {
            int index = 0;

            Collider[] limitsBlocks = Physics.OverlapSphere(offset[i].position, 0.4f, blockMask);


            for (int e = 0; e < limitsBlocks.Length; e++)
            {

                if (limitsBlocks[e].GetComponent<Block>().pathIndex != 0 && limitsBlocks[e].gameObject != gameObject)
                {
                    index++;
                    //for (int o = 0; o < testList.ToArray().Length; o++)
                    //{
                    //    if(limitsBlocks[e].transform.gameObject != testList.ToArray()[o])
                    //        testList.Add(limitsBlocks[e].transform.gameObject);
                    //}
                }
            }


            //Debug.Log(index);
            testIndex = index;
            if (index > 0)
            {
                limitsLine[i].SetActive(false);
            }
            else
            {
                limitsLine[i].SetActive(true);
                player.limits.Add(limitsLine[i]);
            }
        }
    }

    public void StairSetUp()
    {
        if (isStair)
        {
            Vector3 pos = transform.TransformPoint(stairScript.endPoint);
            pos.y = 10;
            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 100, blockMask))
            {
                blockAdjacent[9] = hit.collider.gameObject;
                hit.collider.GetComponent<Block>().blockAdjacent[9] = this.gameObject;
            }
        }
    }

    public void Over()
    {
        if (player.TacticalMode && !player.isMoving && player.turnPlayer && !player.selectDevice)
        {
            mesh.enabled = true;

            int indexPath = 0;
            if (!player.selectTrap)
                indexPath = player.unitsRangeMovement;
            else
                indexPath = player.unitsRangeMovement / 2;

            if(pathIndex != 0 && pathIndex <= indexPath)
                mesh.material.SetColor("_TintColor", colorMainInArea);
            else
                mesh.material.SetColor("_TintColor", colorMainOutArea);
            for (int i = 0; i < blockAdjacent.Length; i++)
            {
                if(blockAdjacent[i] != null)
                {
                    if(blockAdjacent[i] != gameObject)
                    {
                        Block b = blockAdjacent[i].GetComponent<Block>();
                        b.mesh.enabled = true;
                        if(pathIndex != 0 && pathIndex <= indexPath)
                            b.mesh.material.SetColor("_TintColor", colorBaseInArea);
                        else
                            b.mesh.material.SetColor("_TintColor", colorBaseOutArea);
                    }
                }
            }
        }
    }

    public void Exit()
    {
        if (player.TacticalMode)
        {
            mesh.enabled = false;
            for (int i = 0; i < blockAdjacent.Length; i++)
            {
                if (blockAdjacent[i] != null)
                {
                    if (blockAdjacent[i] != gameObject)
                    {
                        Block b = blockAdjacent[i].GetComponent<Block>();
                        b.mesh.enabled = false;
                        mesh.material.SetColor("_TintColor", new Color32(0,0,0,0));
                    }
                }
            }
        }
    }

    public void OnMouseOver()
    {
        Over();
    }

    public void OnMouseExit()
    {
        Exit();
    }

   

    #region Handles
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (isIndexer && mainCell)
        {
            //Handles.color = Color.black;
            //Handles.RadiusHandle(Quaternion.identity, offset[1].position, 0.6f);
            //Handles.RadiusHandle(Quaternion.identity, offset[2].position, 0.6f);
            //Handles.RadiusHandle(Quaternion.identity, offset[3].position, 0.6f);
            //Handles.RadiusHandle(Quaternion.identity, offset[0].position, 0.4f);
        }
        //Handles.RadiusHandle(Quaternion.identity, radiusKillCenter.position, radiusKillSphere);

        //Handles.SphereHandleCap(-1, crosshair.transform.position, Quaternion.identity, 0.5f, EventType.Repaint);
    }
#endif
    #endregion
}

