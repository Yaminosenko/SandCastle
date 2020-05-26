using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterControler : MonoBehaviour
{
    [Header("Reference")]
    public Transform skin;
    public bool FreeMode = false;
    public Camera cam;


    [Header("FreeMode")]
    public float speedPlayer = 4;
    public bool isOnCombat;


    [Header("TacticalMode")]
    public Vector3 pos;
    public int unitsRangeMovement = 3;
    public LayerMask blockMask = 8;
    public Material testMat;
    public Material defaultMat;
    public int actionPoint = 2;
    public bool turnPlayer = true;
    public bool cantMove = false;
    public bool isMoving;
    public int isCover;

    //private
    private Vector3 move;
    private Vector3 vel;
    private Rigidbody rb;
    private Vector3 velocity;
    private NavMeshAgent nav;
    private bool SettingPathBool;
    private int indexRangeMovement;
    private int actionPointIndex;
    private List<Block> blockList = new List<Block>();

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();

    }
    private void Update()
    {
        if (!FreeMode)
        {
            SimpleMove();
            FinalMove();
        }
        else
        {
            if (turnPlayer)
            {
                Movement();
                if (!SettingPathBool)
                    StartCoroutine(LateUp(0.1f));
                if(isMoving)
                    ReachPointDestination();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isOnCombat)
        {
            if (FreeMode)
            {
                FreeMode = false;
                //StartCoroutine(changeCamMode(1f, false));
            }
            else
            {
                FreeMode = true;
                //sStartCoroutine(changeCamMode(1f, true));
            }
        }
    }
    #region Free Movement Methode 
    private void Move()
    {
        float mH = Input.GetAxis("Horizontal");
        float mV = Input.GetAxis("Vertical");

        Vector3 _inpute = new Vector3(mH, 0, mV);
        Vector3 _frontGrav = Vector3.zero;
        _frontGrav = transform.forward * speedPlayer;
        //Debug.Log(mH);

        if (mH > 0.2f || mH < -0.2f || mV > 0.2f || mV < -0.2f)
        {
            _frontGrav.y = rb.velocity.y;
            Quaternion _rotation = Quaternion.LookRotation(_inpute);
            //transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, Time.deltaTime * _rotateSpeed);
            transform.rotation = _rotation;
            rb.velocity = _frontGrav;
            // Run(true);
        }
        else
        {
            _frontGrav = new Vector3(0, rb.velocity.y, 0);
            rb.velocity = _frontGrav;
            //Run(false);
        }
    }

    private void SimpleMove()
    {

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1 || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1)
        {
            move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            velocity += move;
        }
    }

    private void FinalMove()
    {

        Vector3 vel = new Vector3(velocity.x, velocity.y, velocity.z) * speedPlayer;
        vel = transform.TransformDirection(vel);
        transform.position += vel * Time.deltaTime;


        Quaternion _rotation = Quaternion.LookRotation(move);

        skin.rotation = _rotation * transform.rotation;


        velocity = Vector3.zero;
    }
    #endregion

    #region Tactical Movement Methode

    private void Movement()
    {
        if (!cantMove)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == 8)
                {
                    if (Input.GetMouseButtonDown(1) && hit.transform.GetComponent<Block>().pathIndex != 0)
                    {
                        pos = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + transform.position.y, hit.collider.transform.position.z);
                        nav.SetDestination(pos);
                        cantMove = true;
                        isMoving = true;
                        actionPointIndex++;
                        foreach (Block blockAll in blockList)
                        {
                            blockAll.GetComponent<MeshRenderer>().material = defaultMat;
                            blockAll.pathIndex = 0;
                        }
                    }
                }
                else if(hit.transform.gameObject.layer == 13)
                {
                    RaycastHit[] hits;
                    hits = Physics.RaycastAll(ray, Mathf.Infinity);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        RaycastHit hitAll = hits[i];
                        if (hitAll.transform.gameObject.layer == 8)
                        {
                            if (Input.GetMouseButtonDown(1) && hitAll.transform.GetComponent<Block>().pathIndex != 0)
                            {
                                pos = new Vector3(hitAll.collider.transform.position.x, hitAll.collider.transform.position.y + transform.position.y, hitAll.collider.transform.position.z);
                                nav.SetDestination(pos);
                                cantMove = true;
                                isMoving = true;
                                actionPointIndex++;
                                foreach (Block blockAll in blockList)
                                {
                                    blockAll.GetComponent<MeshRenderer>().material = defaultMat;
                                    blockAll.pathIndex = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void ReachPointDestination()
    {
        float dist = Vector3.Distance(transform.position, pos);

           // Debug.Log(dist);
        if (dist <= 0.5f)
        {
            isMoving = false;
            if (StillYourTurn())
            {
                cantMove = false;
                SettingPathBool = false;
            }
        }
    }

    private void SetPath()
    {
        if (!SettingPathBool)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, blockMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow, Mathf.Infinity);
                Block block = hit.collider.GetComponent<Block>();
                block.pathIndex++;
                block.GetComponent<MeshRenderer>().material = testMat;
                List<Block> tabBlock = new List<Block>();
                tabBlock.Add(block);
                blockList.Add(block);
                Block[] blockOrigins = tabBlock.ToArray();
                tabBlock.Clear();

                for (int u = 0; u < unitsRangeMovement; u++)
                {
                    indexRangeMovement++;
                    for (int o = 0; o < blockOrigins.Length; o++)
                    {
                        for (int a = 0; a < blockOrigins[o].blockAdjacent.Length; a++)
                        {
                            if(blockOrigins[o].blockAdjacent[a] != null)
                            {
                                Block blockAdj = blockOrigins[o].blockAdjacent[a].GetComponent<Block>();
                                if (blockAdj.pathIndex == 0)
                                {
                                    tabBlock.Add(blockAdj);
                                    blockList.Add(blockAdj);
                                    blockAdj.pathIndex += indexRangeMovement;
                                    blockAdj.GetComponent<MeshRenderer>().material = testMat;
                                }
                            }
                        }
                    }
                    blockOrigins = tabBlock.ToArray();
                    tabBlock.Clear();
                }
            }
            else
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white, Mathf.Infinity);

            SettingPathBool = true;
        }
    }

    private void SetLimitsSign()
    {
        foreach (Block block in blockList)
        {
            if(block.blockAdjacent.Length < 9)
            {

            }
        }
    }
    #endregion


    #region ActionTacticalMode

    private bool StillYourTurn()
    {
        if (actionPointIndex == actionPoint)
        {
            turnPlayer = false;
            actionPointIndex = 0;
            StartCoroutine(ChangeTurn());
            return false;
        }
        return true;
    }

    #endregion

    #region Coroutine
    IEnumerator LateUp(float time)
    {
        yield return new WaitForSeconds(time);
        SetPath();
        Debug.Log(blockList.ToArray().Length);
    }

    IEnumerator ChangeTurn()
    {
        yield return new WaitForSeconds(2);
        turnPlayer = true;
        cantMove = false;
        SettingPathBool = false;
    }
    #endregion
}
