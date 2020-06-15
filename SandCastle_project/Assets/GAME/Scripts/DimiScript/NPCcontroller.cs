using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
    public int numberOfRandomPosition;
    public float cooldownSeekModeMax;

    //[Header("Communication")]
    //public NPController[] otherGuard;

    [Header("combat")]
    public float damage = 25;
    public List<NPCcontroller> deadAlly = new List<NPCcontroller>();
    public Transform offsetCam;
    public List<NPCcontroller> allyNPC = new List<NPCcontroller>();
    public int isCover;


    [Header("Tactical")]
    public Vector3 pos;
    public Vector3 distractPos;
    public Vector3 alertedPos;
    public int unitsRangeMovement = 5;
    public bool playerSpotted;
    public bool yourTurn;
    public bool alerted;
    public bool distracted;
    //public int indexTurn;

    [Header("Other")]
    public float cooldownShootMax = 0.2f;
    public Animator anim;
    public Vector3 targetPosition;
    public Vector3 lastKnowPosition;
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
    public CameraShoulderMouv camScriptInst;
    public GameObject camShoulder;
    public bool isOnCover;

    private Vector3 sentinelPosition;
    private int indexPatrol = 0;
    private int IndexPatrolMax;
    private Vector3 targetRotate;
    public FieldOfView fov;
    private float cooldownShoot;
    private bool turnHead;
    private Quaternion angleTarget;
    private Quaternion defaultRotation;
    private CharacterControler playerScript;
    private bool FreeMode;
    private bool isMoving;
    private bool SettingPathBool;
    private List<Block> blockList = new List<Block>();
    private List<Cover> coverList = new List<Cover>();
    private List<GameObject> maxRangeBlockList = new List<GameObject>();
    private int indexRangeMovement;
    private bool oneAction;
    private bool randomPosTrigger;
    private bool playerNear;
    private int indexAction;
    private int indexRandom;
    private int indexAlert;
    private Camera cam;
    private bool stop;
    private bool toShort;

    #endregion

    #region UnityMethods

    private void OnEnable()
    {
        playerScript = GameObject.Find("Character").GetComponentInChildren<CharacterControler>();
        system = GameObject.Find("SystemTurn").GetComponent<SytemTurn>();
        distractPos = playerScript.gameObject.transform.position;
        cam = Camera.main;
        FreeMode = !playerScript.TacticalMode;
        IndexPatrolMax = PatrolPath.Length;
        nav = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        nav.speed = walkSpeed;
        sentinelPosition = transform.position;
        defaultRotation = transform.rotation;
        fov.player = playerScript;
        FreeMode = true;
    }

    private void Update()
    {
        if (!dead)
        {
            

            if(playerScript != null)
                FreeMode = !playerScript.TacticalMode;


            if (FreeMode)
                UpdateFreeMode();
            else
                UpdateTacticalMode();
        }
        else
        {
            fov._isActive = false;
        }
    }

    #endregion

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
        walkRunMethods(true);
        nav.speed = walkSpeed;
        nav.isStopped = false;
        //Run(false);

        if (targetPosition != null)
        {
            nav.SetDestination(targetPosition);
        }
    }

    private void Patrol()
    {
        Vector3 TargetPoint = PatrolPath[indexPatrol].position;
        //TargetPoint.y = 0.5f;
        float distance = Vector3.Distance(TargetPoint, transform.position);
        targetPosition = TargetPoint;



        if (distance <= 0.5f)
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
            walkRunMethods(false);

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

    private void RandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * unitsRangeMovement;
        randomDirection += lastKnowPosition;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, unitsRangeMovement, 1);
        Vector3 finalPosition = hit.position;
        targetPosition = finalPosition;
    }

    private void RotateToTarget()
    {
        var targetRotation = Quaternion.LookRotation(lastKnowPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
    }

    private void TurnHead(Quaternion targetAngles)
    {
        if (isTurningHead)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetAngles, 0.02f);
    }

    public void GetKill(bool firstOrSecond)
    {
        if (firstOrSecond)
        {
            
        }
        else
        {
            dead = true;
            nav.SetDestination(transform.position);
            ResetVariables();
            nav.isStopped = true;
            walkRunMethods(false);
            fov.viewMeshFilter.gameObject.SetActive(false);
            fov._isActive = false;
            nav.enabled = false;
            GetComponent<Collider>().isTrigger = true;
            Death();
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
        if (yourTurn && !dead)
        {
            
            if (distracted || alerted || randomPosTrigger)
            {
                if (isOnCover)
                {
                    IdleAlert(false);
                    if (isCover == 1)
                    {
                        CrouchIdle(true);
                        CrouchIdleCover(false);
                    }
                    else
                    {
                        CrouchIdle(false);
                        CrouchIdleCover(true);
                    }
                }
                else
                {
                    IdleAlert(true);
                    CrouchIdle(false);
                    CrouchIdleCover(true);
                }
            }
            else
            {
                IdleAlert(false);
                CrouchIdle(false);
                CrouchIdleCover(false);
            }

            if (!SettingPathBool)
                StartCoroutine(LateUp(0.1f));
            if (isMoving)
                ReachPointDestination();


            if(randomPosTrigger && !oneAction)
            {
                Debug.Log("random");
                RandomPositionTactical();
            }
            else if (alerted && !oneAction)
            {
               // Debug.Log("alerted");
                AlertTactical(alertedPos);
            }
            else if (distracted && !oneAction)
            {
                Debug.Log("distract");
                DistractTactical(distractPos);
            }
            else if (isPatroling && !oneAction)
            {
                Debug.Log("patrol");
                PatrolTactial();
            }
            else if (!oneAction)
            {
                StartCoroutine(waitBeforeChangeTurn(2));
            }
        }
        else
        {
            nav.isStopped = true;
            walkRunMethods(false);
        }
    }

    private void MovementTactical()
    {
        if (!stop)
        {
            nav.isStopped = false;
            //Debug.Log(pos);
            nav.SetDestination(pos);
            //cantMove = true;
            isMoving = true;

            walkRunMethods(true);
        }
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

        Vector3 newPos = new Vector3(pos.x, transform.position.y - 0.5f, pos.z);
        float dist = Vector3.Distance(transform.position, newPos);

        // Debug.Log(dist);
        if (dist <= 0.5f)
        {
            if (indexAction == 1)
            {
                if (toShort)
                {
                    alerted = false;
                    distracted = false;
                    toShort = false;
                }
                Debug.Log("NPCReachDestination");
                walkRunMethods(false);
                WalkAlert(false);
                oneAction = true;
                isMoving = false;
                if (fov.iSeeYou || playerNear)
                {
                    KillPlayer();
                }
                else
                {
                    if(dead)
                        StartCoroutine(waitBeforeChangeTurn(0));
                    else
                        StartCoroutine(waitBeforeChangeTurn(2));
                }
            }
            else
                indexAction++;

            if (isPatroling && !distracted && !alerted)
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

            if (randomPosTrigger && indexRandom == 2)
            {
                randomPosTrigger = false;
                indexRandom = 0;
            }

            if(alerted && indexAlert == 3)
            {
                randomPosTrigger = true;
                alerted = false;
                indexAlert = 0;
            }
        }
    }

    private void SetPath()
    {
        if (!SettingPathBool)
        {
            RaycastHit hit;
            Vector3 transPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            if (Physics.Raycast(transPos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, blockMask))
            {
                Debug.DrawRay(transPos, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow, Mathf.Infinity);
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
                                    if (blockAdj.isCover)
                                        coverList.Add(blockAdj.coverScript);
                                    if (blockAdj.pathIndex == unitsRangeMovement)
                                        maxRangeBlockList.Add(blockAdj.gameObject);
                                }
                            }
                        }
                    }
                    blockOrigins = tabBlock.ToArray();
                    tabBlock.Clear();
                }
                    //Debug.Log(maxRangeBlockList.ToArray().Length);
                    SettingPathBool = true;
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white, Mathf.Infinity);
                SettingPathBool = true;
            }


        }
    }

    private void DistractTactical(Vector3 destination)
    {
        if (SettingPathBool)
        {
            Vector3 finalPos = Vector3.zero;
            Vector3 offset = destination;
            float lastClosestDistance = Vector3.Distance(transform.position, Vector3.positiveInfinity);
            int indexLength = 100;
            NavMeshPath path = new NavMeshPath();
            nav.CalculatePath(destination, path);

            RaycastHit hitFirst;
            if (Physics.Raycast(destination, Vector3.down, out hitFirst, blockMask))
            {
                if (hitFirst.transform.GetComponent<Block>().pathIndex != 0)
                {
                    pos = hitFirst.transform.position;
                    MovementTactical();
                    oneAction = true;
                    //randomPosTrigger = true;

                    toShort = true;
                    Debug.Log("toShort");
                    return;
                }
            }

            for (int i = 0; i < path.corners.Length; i++)
            {
                Vector3 corners = new Vector3(path.corners[i].x, path.corners[i].y + 0.5f, path.corners[i].z);
                RaycastHit hit;
                if (Physics.Raycast(corners, Vector3.down, out hit, blockMask))
                {
                    if (hit.transform.GetComponent<Block>().pathIndex != 0)
                    {
                        indexLength = i;
                    }
                    if (i == indexLength + 1)
                        offset = hit.transform.position;
                }
            }

            GameObject[] maxBlock = maxRangeBlockList.ToArray();
            for (int i = 0; i < maxBlock.Length; i++)
            {
                float testDistance = Vector3.Distance(maxBlock[i].transform.position, offset);
                //Debug.Log(testDistance);

                if (testDistance < lastClosestDistance)
                {
                    lastClosestDistance = testDistance;
                    finalPos = maxBlock[i].transform.position;
                }
            }
            Debug.Log(pos);
            pos = finalPos;
            MovementTactical();
            oneAction = true;
        }
    }

    private void AlertTactical(Vector3 position)
    {
        if (SettingPathBool)
        {
            Cover[] cover = coverList.ToArray();
            position = new Vector3(position.x, position.y + 0.5f, position.z);
            Debug.Log(position);
            //Debug.Log(coverList.ToArray().Length);
            List<GameObject> selectCoverList = new List<GameObject>();
            Vector3 finalPos = Vector3.zero;
            float lastClosestDistance = Vector3.Distance(transform.position, Vector3.positiveInfinity);
            for (int i = 0; i < cover.Length; i++)
            {
                cover[i].offsetLocalPos.transform.position = position;

                if (cover[i].offsetLocalPos.transform.localPosition.z > 1)
                    selectCoverList.Add(cover[i].gameObject);

                cover[i].offsetLocalPos.transform.position = cover[i].transform.position;
            }


            RaycastHit hitFirst;
            if (Physics.Raycast(position, Vector3.down, out hitFirst, blockMask))
            {
                //Debug.Log(hitFirst.transform.position);
                Debug.Log(hitFirst.transform.gameObject);
                Debug.DrawRay(position, Vector3.down*1000, Color.blue, Mathf.Infinity);
                if (hitFirst.transform.GetComponent<Block>().pathIndex != 0)
                {
                    //randomPosTrigger = true;
                    toShort = true;
                    Debug.Log("toShortAlerted");
                    if(hitFirst.transform.GetComponent<Block>().pathIndex <= unitsRangeMovement / 2 && playerSpotted)
                        playerNear = true;
                }
            }

            GameObject[] selectCover = selectCoverList.ToArray();
            for (int i = 0; i < selectCover.Length; i++)
            {
                float testDistance = Vector3.Distance(selectCover[i].transform.position, position);
               
                if (testDistance < lastClosestDistance)
                {
                    lastClosestDistance = testDistance;
                    finalPos = selectCover[i].transform.position;
                }
            }
            pos = new Vector3(finalPos.x,finalPos.y + 0.3f,finalPos.z);
            //Debug.Log(pos);
            Debug.Log(finalPos);
            indexAlert++;
            MovementTactical();
            oneAction = true;
        }
    }

    private void GetClosestCover()
    {
        if (SettingPathBool)
        {
            Cover[] cover = coverList.ToArray();
            List<GameObject> selectCoverList = new List<GameObject>();
            Vector3 finalPos = Vector3.zero;
            float lastClosestDistance = Vector3.Distance(transform.position, Vector3.positiveInfinity);
            for (int i = 0; i < cover.Length; i++)
            {
                cover[i].offsetLocalPos.transform.position = playerScript.transform.position;

                if (cover[i].offsetLocalPos.transform.localPosition.z > 1)
                    selectCoverList.Add(cover[i].gameObject);

                cover[i].offsetLocalPos.transform.position = cover[i].transform.position;
            }

            GameObject[] selectCover = selectCoverList.ToArray();
            for (int i = 0; i < selectCover.Length; i++)
            {
                float testDistance = Vector3.Distance(selectCover[i].transform.position, transform.position);

                if (testDistance < lastClosestDistance)
                {
                    lastClosestDistance = testDistance;
                    finalPos = selectCover[i].transform.position;
                }
            }

            pos = new Vector3(finalPos.x, 0.1f, finalPos.z);
            //Debug.Log(finalPos);
            MovementTactical();
            alerted = true;
            oneAction = true;
        }
    }

    private void RandomPositionTactical()
    {
        playerSpotted = false;
        Vector3 randomDirection = Random.insideUnitSphere * unitsRangeMovement;
        randomDirection += lastKnowPosition;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        NavMesh.SamplePosition(randomDirection, out hit, unitsRangeMovement, 1);
        RaycastHit hitBlock;
        if (Physics.Raycast(hit.position, Vector3.down, out hitBlock, blockMask))
        {
           finalPosition = hitBlock.transform.position;
        }
        else
        {
            finalPosition = hit.position;
        }

        indexRandom++;
        targetPosition = finalPosition;
        
        pos = finalPosition;
        MovementTactical();
        oneAction = true;
    }

    public void ResetVariables()
    {
        yourTurn = false;
        indexAction = 0;
        oneAction = false;
        SettingPathBool = false;
        for (int i = 0; i < blockList.ToArray().Length; i++)
        {
            blockList.ToArray()[i].pathIndex = 0;
        }
        blockList.Clear();
        coverList.Clear();
    }

    public void HeardSomething()
    {
        randomPosTrigger = false;
        indexRandom = 0;
        distracted = true;
        if (alerted)
            alertedPos = distractPos;
    }

    public void GetAlerted()
    {
        randomPosTrigger = false;
        alerted = true;
        indexAlert = 0;
        indexRandom = 0;
    }

    private void InstanciateCamera(Transform target)
    {

        GameObject _currentCam = (GameObject)Instantiate(camShoulder, cam.transform.position, cam.transform.rotation) as GameObject;
        //Debug.Log(_currentCam);
        camScriptInst = _currentCam.GetComponent<CameraShoulderMouv>();
        //_camSwitch.cameraOne = _currentCam;
        //_camSwitch.cameraChangeCounter();
        camScriptInst.targetObj = target;
        camScriptInst.views[0] = offsetCam.transform;
        camScriptInst.LetsGo();
        fov.camShoulder = camScriptInst;

        //if (_currentFov._actualTarget != null)
        //{
        //    _currentFov._actualTarget = _camMouv.targetObj;
        //}

    }

    public void KillPlayerTactical(Transform target)
    {
        int random = Random.Range(0, 1);
        nav.isStopped = true;
        stop = true;
        //Debug.Log("kill");
        walkRunMethods(false);
        //Debug.Log(random);
        if(random == 0)
            InstanciateCamera(target);
        system.KillPlayerSystem(this);
        StartCoroutine(KillPlayerAnim());
    }

    #endregion

    #region Animation

    public void Walk(bool b)
    {
        anim.SetBool("walk", b);
    }

    public void WalkAlert(bool b)
    {
        anim.SetBool("walkAlert", b);
    }

    public void Fire()
    {
        anim.SetTrigger("fire");
    }

    public void Death()
    {
        anim.SetTrigger("death");
    }

    public void CrouchIdle(bool b)
    {
        anim.SetBool("crouchIdle", b);
    }

    public void CrouchIdleCover(bool b)
    {
        anim.SetBool("crouchIldeCover", b);
    }


    public void IdleAlert(bool b)
    {
        anim.SetBool("idleAlert", b);
    }


    private void walkRunMethods(bool b)
    {
        if (distracted || alerted || randomPosTrigger)
        {
            WalkAlert(b);
            //Walk(b);
        }
        else 
        {
            Walk(b);
            //Run(b);
        }
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
        walkRunMethods(true);
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

    IEnumerator waitBeforeChangeTurn(float time)
    {
        oneAction = true;
        Debug.Log("END TURN");
        yield return new WaitForSeconds(time);
        system.NextTurn();
        ResetVariables();
    }

    public IEnumerator SeeCorpsTactical()
    {
        nav.isStopped = true;
        walkRunMethods(false);
        yield return new WaitForSeconds(2);
        GetClosestCover();
    }
    //public IEnumerator SetActiveWithCooldown(float time, Image image)
    //{
    //    image.gameObject.SetActive(true);
    //    yield return new WaitForSeconds(time);
    //    image.gameObject.SetActive(false);
    //}

    public IEnumerator KillPlayerAnim()
    {
        yield return new WaitForSeconds(1);
        Fire();
        playerScript.Death();
        yield return new WaitForSeconds(3);
        system.Restart();
    }



    #endregion

    #region Handles
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        //Handles.color = Color.red;
        //Handles.SphereHandleCap(-1, testVct, Quaternion.identity, 1, EventType.Repaint);
        //Handles.color = Color.blue;
        //Handles.SphereHandleCap(-1, testVct, Quaternion.identity, 1, EventType.Repaint);



    }
#endif
    #endregion
}
