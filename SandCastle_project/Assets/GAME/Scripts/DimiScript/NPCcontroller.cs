using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCcontroller : MonoBehaviour
{
    #region Variables

    [Header("Patrol")]
    public Transform[] PatrolPath;
    public float waitTime;
    public bool loop = false;
    public bool isPatroling;
    public bool isTurningHead = true;

    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float walkRadius;
    public int numberOfRandomPosition;
    public float cooldownSeekModeMax;

    //[Header("Communication")]
    //public NPController[] otherGuard;

    [Header("combat")]
    public float damage = 25;


    [Header("Tactical")]
    public Vector3 pos;
    public int unitsRangeMovement = 5;
    public bool yourTurn;
    //public int indexTurn;

    [Header("Other")]
    public float cooldownShootMax = 0.2f;
    public Animator anim;
    public Vector3 position;
    public Transform player;
    public Vector3 targetPosition;
    public Vector3 lastKnowPosition;
    public Transform head;
    public GameObject trigger;
    public bool heard = false;
    public bool stopPatrol = false;
    public bool stopMove = false;
    public bool seekMode = false;
    public bool heardSomthing = false;
    public bool dead = false;
    public bool alerteZone = false;
    public NavMeshAgent nav;
    public LayerMask blockMask;
    public int IndexRandomPos;
    public SytemTurn system;



    private Vector3 sentinelPosition;
    private int indexPatrol = 0;
    private int IndexPatrolMax;
    private Vector3 targetRotate;
    private FieldOfView fov;
    private float cooldownShoot;
    private bool turnHead;
    private Quaternion angleTarget;
    private Quaternion defaultRotation;
    private CharacterControler playerScript;
    private bool FreeMode;
    private bool isMoving;
    private bool SettingPathBool;
    private List<Block> blockList = new List<Block>();
    private int indexRangeMovement;
    private bool oneAction;
    private int indexAction;

    #endregion

    private void OnEnable()
    {
        playerScript = GameObject.Find("Character").GetComponentInChildren<CharacterControler>();
        FreeMode = !playerScript.TacticalMode;
        IndexPatrolMax = PatrolPath.Length;
        nav = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        nav.speed = walkSpeed;
        sentinelPosition = transform.position;
        defaultRotation = transform.rotation;
        fov.player = playerScript;
    }

    private void Update()
    {
        if (!dead)
        {
            FreeMode = !playerScript.TacticalMode;

            if (FreeMode)
                UpdateFreeMode();
            else
                UpdateTacticalMode();
        }
    }

    #region IA Free Mode 

    private void UpdateFreeMode()
    {
        if (!stopPatrol)
        {
            if (isPatroling)
            {
                Patrol();
            }
            else
            {
                SentinelSpot();
            }
        }

        if (turnHead)
        {
            TurnHead(angleTarget);
        }
        else
        {
            //TurnHead(defaultRotation);
        }


        if (heard)
            AlerteMode(1f);

        if (seekMode)
            AlerteMode(2f);

        if (!stopMove)
            Movement();

        if (heardSomthing)
            RotateToTarget();
    }

    private void Movement()
    {
        Walk(true);
        nav.speed = walkSpeed;
        nav.isStopped = false;
        //Run(false);

        if (targetPosition != null)
            nav.SetDestination(targetPosition);
    }

    private void Patrol()
    {
        Vector3 TargetPoint = PatrolPath[indexPatrol].position;
        TargetPoint.y = 0.5f;
        float distance = Vector3.Distance(TargetPoint, transform.position);
        targetPosition = TargetPoint;



        if (distance <= 1)
        {
            switch (loop)
            {
                case true:
                    if (indexPatrol < IndexPatrolMax - 1)
                    {
                        indexPatrol++;
                        IndexPatrolMax = PatrolPath.Length;
                        StartCoroutine(StopMove(waitTime));
                    }
                    else
                    {
                        indexPatrol--;
                        IndexPatrolMax = 2;
                        StartCoroutine(StopMove(waitTime));
                    }
                    break;

                case false:
                    if (indexPatrol < IndexPatrolMax - 1)
                    {
                        indexPatrol++;
                        StartCoroutine(StopMove(waitTime));
                    }
                    else
                    {
                        indexPatrol = 0;
                        StartCoroutine(StopMove(waitTime));
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void SentinelSpot()
    {
        Vector3 TargetPoint = sentinelPosition;
        // TargetPoint.y = 0.5f;
        float distance = Vector3.Distance(TargetPoint, transform.position);
        //Debug.Log(sentinelPosition);
        targetPosition = TargetPoint;

        if (distance <= 1)
        {
            stopMove = true;
            Walk(false);

            if (!turnHead)
                StartCoroutine(TurnHeadOff(4));
        }
    }

    public void AlerteMode(float multiplicateur)
    {
        Vector3 TargetPoint = targetPosition;
        //TargetPoint.y = 0.5f;
        float distance = Vector3.Distance(TargetPoint, transform.position);


        if (distance <= 1)
        {
            if (fov.shootMode == false)
            {
                StartCoroutine(StopMove(waitTime));
                if (IndexRandomPos < numberOfRandomPosition * multiplicateur)
                {
                    RandomPosition();
                    IndexRandomPos++;
                }
                else
                {
                    heard = false;
                    stopPatrol = false;
                    //fov._oneTime = false;
                    //fov.timeSpeed = multiplicateur;
                    seekMode = false;
                    IndexRandomPos = 0;
                }
            }
        }
    }

    //Looking for a random position in the NavaMesh
    private void RandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += lastKnowPosition;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;
        targetPosition = finalPosition;
    }

    //Rotate the NPC in the direction of the target
    private void RotateToTarget()
    {
        var targetRotation = Quaternion.LookRotation(lastKnowPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
    }

    //Turn head with the wanted angle
    private void TurnHead(Quaternion targetAngles)
    {
        if (isTurningHead)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetAngles, 0.02f);
    }

    //Action when the NPC get kill
    public void GetSilentKill(bool firstOrSecond)
    {
        if (firstOrSecond)
        {
            dead = true;
            nav.SetDestination(transform.position);
            nav.isStopped = true;
            Walk(false);
        }
        else
        {
            fov.viewMeshFilter.gameObject.SetActive(false);
            //fov.dead = true;
            nav.enabled = false;
            GetComponent<Collider>().isTrigger = true;
            BackStabDeath();
            TriggerEffect();
        }
    }

    public void TriggerEffect()
    {
        if (trigger != null)
        {
            //trigger.transform.position = new Vector3(trigger.transform.position.x, trigger.transform.position.y + 4, trigger.transform.position.z);
            trigger.gameObject.SetActive(false);
        }
    }

    public void KillPlayer()
    {
        Debug.Log("oui");
    }

    #endregion

    #region IA Tactical Mode

    private void UpdateTacticalMode()
    {
        if (yourTurn)
        {
            if (!SettingPathBool)
                StartCoroutine(LateUp(0.1f));
            if (isMoving)
                ReachPointDestination();

            if(isPatroling)
                if (!oneAction)
                    PatrolTactial();
        }
        else
        {
            nav.isStopped = true;
            Walk(false);
        }
    }

    private void MovementTactical()
    {
        nav.isStopped = false;
        nav.SetDestination(pos);
        //cantMove = true;
        isMoving = true;
        Walk(true);
    }

    private void PatrolTactial()
    {
        Vector3 TargetPoint = PatrolPath[indexPatrol].position;

        RaycastHit hit;
        if(Physics.Raycast(TargetPoint, Vector3.down, out hit, blockMask))
        {
            pos = hit.transform.position;
            MovementTactical();
        }
        //float distance = Vector3.Distance(TargetPoint, transform.position);
        //targetPosition = TargetPoint;
    }

    private void ReachPointDestination()
    {
        float dist = Vector3.Distance(transform.position, pos);

        // Debug.Log(dist);
        if (dist <= 0.5f)
        {
            if (indexAction == 1)
            {
                Walk(false);
                oneAction = true;
                isMoving = false;
                if (fov.iSeeYou)
                {
                    KillPlayer();
                }
                else
                {
                    system.NextTurn();
                    ResetVariables();
                }
            }
            else
                indexAction++;

            if (isPatroling)
            {
                switch (loop)
                {
                    case true:
                        if (indexPatrol < IndexPatrolMax - 1)
                        {
                            indexPatrol++;
                            IndexPatrolMax = PatrolPath.Length;
                           // StartCoroutine(StopMove(waitTime));
                        }
                        else
                        {
                            indexPatrol--;
                            IndexPatrolMax = 2;
                           // StartCoroutine(StopMove(waitTime));
                        }
                        break;

                    case false:
                        if (indexPatrol < IndexPatrolMax - 1)
                        {
                            indexPatrol++;
                            //StartCoroutine(StopMove(waitTime));
                        }
                        else
                        {
                            indexPatrol = 0;
                            //StartCoroutine(StopMove(waitTime));
                        }
                        break;
                    default:
                        break;
                }
            }

           
        }
    }

    private void SetPath()
    {
        if (!SettingPathBool)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, blockMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow, Mathf.Infinity);
                Block block = hit.collider.GetComponent<Block>();
                block.pathIndex++;
                //block.GetComponent<MeshRenderer>().material = testMat;
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
                            if (blockOrigins[o].blockAdjacent[a] != null)
                            {
                                Block blockAdj = blockOrigins[o].blockAdjacent[a].GetComponent<Block>();
                                if (blockAdj.pathIndex == 0)
                                {
                                    tabBlock.Add(blockAdj);
                                    blockList.Add(blockAdj);
                                    blockAdj.pathIndex += indexRangeMovement;
                                    // blockAdj.GetComponent<MeshRenderer>().material = testMat;
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


        }
    }

    private void ResetVariables()
    {
        yourTurn = false;
        indexAction = 0;
        oneAction = false;
    }
    #endregion


    #region Animation

    public void Walk(bool b)
    {
        anim.SetBool("Walk", b);
    }

    public void Run(bool b)
    {
        anim.SetBool("Run", b);
    }

    public void Fire(bool b)
    {
        anim.SetBool("Fire", b);
    }

    public void BackStabDeath()
    {
        anim.SetTrigger("Death");
    }
    #endregion

    #region Coroutine

    IEnumerator StopMove(float timeToWait)
    {
        stopMove = true;
        Walk(false);
        yield return new WaitForSeconds(timeToWait);
        stopMove = false;
    }


    public IEnumerator HeardOrSeeWait(float WaitTime, Vector3 pos, bool sound)
    {
        stopMove = true;
        stopPatrol = true;
        nav.isStopped = true;
        Walk(false);
        turnHead = false;
        StopCoroutine(TurnHeadOff(4));
        lastKnowPosition = pos;
        if (sound)
        {
            heardSomthing = true;
            //StartCoroutine(SetActiveWithCooldown(timeToStay, hear));
        }
        else
        {
            //StartCoroutine(SetActiveWithCooldown(timeToStay, yellowEye));
        }
        yield return new WaitForSeconds(WaitTime);
        if (sound)
            heardSomthing = false;
        nav.isStopped = false;
        //fov.timeSpeed = 2;
        stopMove = false;
        heard = true;
        targetPosition = pos;
    }

    public IEnumerator TurnHeadOff(float waitTime)
    {

        angleTarget = transform.rotation * Quaternion.Euler(0, 90, 0);
        turnHead = true;
        yield return new WaitForSeconds(waitTime);
        angleTarget = transform.rotation * Quaternion.Euler(0, -90, 0);
        yield return new WaitForSeconds(waitTime);
        turnHead = false;
    }

    IEnumerator LateUp(float time)
    {
        yield return new WaitForSeconds(time);
        SetPath();
        //SetLimitsSign();
    }

    //public IEnumerator SetActiveWithCooldown(float time, Image image)
    //{
    //    image.gameObject.SetActive(true);
    //    yield return new WaitForSeconds(time);
    //    image.gameObject.SetActive(false);
    //}

    //IEnumerator BulletTraceCD(float time)
    //{
    //    trace.laserLineRenderer.enabled = true;
    //    yield return new WaitForSeconds(time);
    //    trace.laserLineRenderer.enabled = false;
    //}


    #endregion

}
