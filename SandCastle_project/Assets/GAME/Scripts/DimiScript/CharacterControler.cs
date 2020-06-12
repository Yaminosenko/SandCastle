using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CharacterControler : MonoBehaviour
{
    #region Variables 

    [Header("Reference")]
    public Transform skin;
    public Camera cam;
    public int testInt;
    public LineRenderer pathPreviewLine;
    public Material mRenderClassic;
    public Material mRenderCant;
    public Animator anim;
    public SytemTurn system;
    public GameObject offsetCamShoulder;
    public GameObject camShoulder;
    public CameraShoulderMouv camScriptInst;
    public CameraController camControl;
    public GameObject trapToInst;

    [Header("FreeMode")]
    public float speedPlayer = 4;
    public bool isOnCombat;
    public List<Device> deviceList = new List<Device>();
    public float Water;
    public int Trap;
    public float waterWasteOnAction = 10f;

    [Header("TacticalMode")]
    public bool TacticalMode = false;
    public int invisibilityDuration = 2;
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
    public List<GameObject> limits = new List<GameObject>();
    public bool notInArea;
    public bool SettingPathBool;
    public bool selectDevice;
    public bool targetPlayer;
    public bool isInvisble;

    //private
    private Vector3 move;
    private Vector3 vel;
    private Rigidbody rb;
    private Vector3 velocity;
    public  NavMeshAgent nav;
    private int indexRangeMovement;
    private int actionPointIndex;
    private Vector3 secuPathPreview;
    private List<Block> blockList = new List<Block>();
    private List<Vector3> pathWaypoint = new List<Vector3>();
    private bool statick;
    private FieldOfView fov;
    private bool isAiming;
    private bool occuped;
    private int deviceIndex;
    private int maxDeviceIndex;
    private Device selectDeviceScript;
    private bool selectInvisiblity;
    public bool selectTrap;
    private float baseTacticalSpeed;


    #endregion 

    #region UnityMethods

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        system = system = GameObject.Find("SystemTurn").GetComponent<SytemTurn>();
        InvokeRepeating("WaterFreeWaste", 1f, 0.1f);
        baseTacticalSpeed = nav.speed;
        if(system != null)
            system.player = gameObject.GetComponent<CharacterControler>();
    }
    private void Update()
    {
        skin.position = transform.position;
        if (!TacticalMode)
        {
            SimpleMove();
            FinalMove();
        }
        else
        {
            if(isCover == 1)
            {
                CoverIdle(false);
                CoverCrouchIdle(true);
            }
            else if(isCover == 2)
            {
                CoverIdle(true);
                CoverCrouchIdle(false);
            }
            else
            {
                CoverIdle(false);
                CoverCrouchIdle(false);
            }


            if (turnPlayer)
            {
                
                skin.rotation = transform.rotation;
                if(!isAiming)
                    Movement();
                else
                {
                    if (Input.GetKeyDown(KeyCode.L))
                        ShootKill();
                }
                if (!SettingPathBool)
                    StartCoroutine(LateUp(0.1f));
                if(isMoving)
                    ReachPointDestination();

                if (!cantMove)
                {
                    if (selectDevice)
                        DeviceChoice();
                    else if (selectInvisiblity)
                        Invisibility();
                    //else if (selectTrap)
                    //TrapSetUp();
                    UpdateInpute();
                }
            }
        }

        ChangeMode();
    }

    #endregion

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
            statick = false;
            Run(true);
        }
        else
        {
            statick = true;
            Run(false);
        }
    }

    private void FinalMove()
    {

        Vector3 vel = new Vector3(velocity.x, velocity.y, velocity.z) * speedPlayer;
        vel = transform.TransformDirection(vel);
        transform.position += vel * Time.deltaTime;


        Quaternion _rotation = Quaternion.LookRotation(move);

        if(!statick)
            skin.rotation = _rotation * transform.rotation;
       

        velocity = Vector3.zero;
    }

    private void WaterFreeWaste()
    {
        if(!TacticalMode)
            Water -= 0.05f;
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
                if (hit.transform.gameObject.layer == 8 && !occuped)
                {

                    PreviewPath(hit);


                    if (Input.GetMouseButtonDown(1) && hit.transform.GetComponent<Block>().pathIndex != 0)
                    {
                        pos = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 0.5f, hit.collider.transform.position.z);

                        camControl.target = transform;
                        nav.SetDestination(pos);
                        cantMove = true;
                        isMoving = true;
                        actionPointIndex++;
                        WaterWaste(waterWasteOnAction);
                        ResetAllPreview();
                        WalkTactical(true);
                    }
                }
                else if (selectTrap && hit.transform.gameObject.layer == 8)
                {
                    if (Input.GetMouseButtonDown(0) && hit.transform.GetComponent<Block>().pathIndex != 0 && hit.transform.GetComponent<Block>().pathIndex <= unitsRangeMovement / 2)
                    {
                        pos = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z);
                        GameObject trap = Instantiate(trapToInst, pos, Quaternion.identity);
                        trap.transform.position = pos;
                        WaterWaste(waterWasteOnAction);
                        trap.GetComponent<TrapScript>().player = this;
                        trap.GetComponentInChildren<MeshRenderer>().enabled = false;
                        StartCoroutine(TrapEnable(1, trap.GetComponentInChildren<MeshRenderer>(), trap.GetComponent<TrapScript>()));
                        Trap--;
                        //Debug.Log(AnimationLength("Trap"));
                    }
                }
                else if(hit.transform.gameObject.layer == 13)
                {
                    RaycastHit[] hits;
                    hits = Physics.RaycastAll(ray, Mathf.Infinity);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        RaycastHit hitAll = hits[i];
                        if (hitAll.transform.gameObject.layer == 8 && !occuped)
                        {
                            PreviewPath(hit);


                            if (Input.GetMouseButtonDown(1) && hitAll.transform.GetComponent<Block>().pathIndex != 0 )
                            {
                                pos = new Vector3(hitAll.collider.transform.position.x, hitAll.collider.transform.position.y + 0.5f, hitAll.collider.transform.position.z);
                                nav.SetDestination(pos);
                                camControl.target = transform; 
                                cantMove = true;
                                isMoving = true;
                                actionPointIndex++;
                                WaterWaste(waterWasteOnAction);
                                ResetAllPreview();
                                WalkTactical(true);
                            }
                        }
                        else if(selectTrap && hitAll.transform.gameObject.layer == 8)
                        {
                            if(Input.GetMouseButtonDown(0) && hitAll.transform.GetComponent<Block>().pathIndex != 0 && hitAll.transform.GetComponent<Block>().pathIndex <= unitsRangeMovement / 2)
                            {
                                pos = new Vector3(hitAll.collider.transform.position.x, hitAll.collider.transform.position.y, hitAll.collider.transform.position.z);
                                GameObject trap = Instantiate(trapToInst, pos, Quaternion.identity);
                                trap.transform.position = pos;
                                trap.GetComponent<TrapScript>().player = this;
                                WaterWaste(waterWasteOnAction);
                                trap.GetComponentInChildren<MeshRenderer>().enabled = false;
                                StartCoroutine(TrapEnable(1, trap.GetComponentInChildren<MeshRenderer>(), trap.GetComponent<TrapScript>()));
                                Trap--;
                                //Debug.Log(AnimationLength("Trap"));
                            }
                        }
                    }
                }
            }
        }
    }

    private void PreviewPath(RaycastHit hit)
    {
        if (!occuped)
        {
            if (secuPathPreview != hit.transform.position)
            {
                pathWaypoint.Clear();
                NavMeshPath path = new NavMeshPath();
                nav.CalculatePath(hit.transform.position, path);

                for (int i = 0; i < path.corners.Length; i++)
                {
                    Vector3 corners = new Vector3(path.corners[i].x, path.corners[i].y + 0.5f, path.corners[i].z);
                    RaycastHit wayHit;
                    if (Physics.Raycast(corners, Vector3.down, out wayHit, blockMask))
                    {
                        pathWaypoint.Add(new Vector3(wayHit.transform.position.x, wayHit.transform.position.y + 0.5f, wayHit.transform.position.z));
                    }
                }


                if (hit.transform.gameObject.GetComponent<Block>() != null)
                {
                    if (hit.transform.gameObject.GetComponent<Block>().pathIndex == 0)
                    {
                        pathPreviewLine.material = mRenderCant;
                        if (limits.ToArray().Length != 0)
                        {
                            for (int i = 0; i < limits.ToArray().Length; i++)
                            {
                                limits.ToArray()[i].GetComponent<MeshRenderer>().material = mRenderCant;
                                notInArea = true;
                            }
                        }
                    }
                    else
                    {
                        pathPreviewLine.material = mRenderClassic;
                        if (limits.ToArray().Length != 0)
                        {
                            for (int i = 0; i < limits.ToArray().Length; i++)
                            {
                                limits.ToArray()[i].GetComponent<MeshRenderer>().material = mRenderClassic;
                                notInArea = false;
                            }
                        }
                    }
                }


                Vector3[] waypoints = pathWaypoint.ToArray();
                pathPreviewLine.positionCount = waypoints.Length;
                pathPreviewLine.SetPositions(waypoints);

                secuPathPreview = hit.transform.position;
            }
        }
    }

    private void ReachPointDestination()
    {
        Vector3 newPos = new Vector3(pos.x, transform.position.y - 0.5f, pos.z);
        float dist = Vector3.Distance(transform.position, newPos);

        //if(nav.isOnOffMeshLink)
        //{
        //    Debug.Log("navLink");
        //    nav.speed = 1;
        //    Ladder(true);
        //}
        //else if(isMoving)
        //{
        //    nav.speed = baseTacticalSpeed;
        //    Ladder(false);
        //}
           // Debug.Log(dist);
        if (dist <= 0.5f)
        {
            isMoving = false;
            WalkTactical(false);
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
                            if(blockOrigins[o].blockAdjacent[a] != null)
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

    private void SetLimitsSign()
    {
        if (!SettingPathBool)
        {
            //Debug.Log(blockList.ToArray().Length);
            testInt++;
            foreach (Block block in blockList)
            {
                int index = 0;

                for (int i = 0; i < block.blockAdjacent.Length; i++)
                {
                    if (block.blockAdjacent[i] != null)
                        index++;
                }

                if (index < 9 || block.pathIndex == unitsRangeMovement)
                {
                    block.isIndexer = true;
                    block.PathLimits();
                }
            }
            SettingPathBool = true;
        }
            
    }
    #endregion

    #region Action Tactical Methods

    public void DeviceChoice()
    {
        //Debug.Log(deviceIndex);
        Device[] deviceTab = deviceList.ToArray();
        maxDeviceIndex = deviceTab.Length;
        
        for (int i = 0; i < deviceTab.Length; i++)
        {
            if(deviceIndex == i)
            {
                camControl.target = deviceTab[i].transform;
                selectDeviceScript = deviceTab[i];
            }
        }

        selectDeviceScript.PreviewRangeDevice();
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(selectDeviceScript);
            WaterWaste(waterWasteOnAction);
            selectDeviceScript.SecuSound();
            selectDeviceScript.ResetPreview();
            Hack();
            actionPointIndex++;
            if (StillYourTurn())
            {
                selectDevice = false;
                occuped = false;
                camControl.target = transform;
                StartCoroutine(CamTargetPlayer());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            deviceIndex++;
            selectDeviceScript.ResetPreview();
            if (deviceIndex == maxDeviceIndex)
                deviceIndex = 0;
        }
    }

    private void Aim()
    {
        pathPreviewLine.positionCount = 0;
        pathWaypoint.Clear();
        isAiming = true;
        fov._swap = true;
        fov._isAimaing = true;
        InstanciateCamera();
        fov.Refresh();
    }

    private void UnAim()
    {
        isAiming = false;
        occuped = false;
        fov.Refresh();
        if (camScriptInst != null)
        {
            camScriptInst._isActive = true;
            camScriptInst.DestroyObject();
        }
    }

    private void Invisibility()
    {
        camControl.target = transform;
        targetPlayer = true;
        if (Input.GetKeyDown(KeyCode.L))
        {
            WaterWaste(waterWasteOnAction);
            isInvisble = true;
            system.cdInvisibilty = true;
            StartCoroutine(getInvisible(1));
        }
        
        //MeshRenderer mesh = transform.GetComponent<MeshRenderer>();
        //mesh.material = /*new Material(0,0,0)*/
    }


    #endregion

    #region Other Methods

    private bool StillYourTurn()
    {
        if (actionPointIndex == actionPoint)
        {
            ResetAllPreview();
            turnPlayer = false;
            selectInvisiblity = false;
            selectDevice = false;
            actionPointIndex = 0;
            UnAim();
            occuped = false;
            StartCoroutine(ChangeTurn());
            return false;
        }
        return true;
    }

    public void ResetAllPreview()
    {
        indexRangeMovement = 0;
        foreach (Block blockAll in blockList)
        {
            //blockAll.GetComponent<MeshRenderer>().material = defaultMat;
            blockAll.pathIndex = 0;
            for (int i = 0; i < blockAll.limitsLine.Length; i++)
            {
                blockAll.limitsLine[i].SetActive(false);
            }
        }
        blockList.Clear();
        pathPreviewLine.positionCount = 0;
        pathWaypoint.Clear();
        limits.Clear();
    }

    private void InstanciateCamera()
    {

        GameObject _currentCam = (GameObject)Instantiate(camShoulder, cam.transform.position, cam.transform.rotation) as GameObject;
        //Debug.Log(_currentCam);
        camScriptInst = _currentCam.GetComponent<CameraShoulderMouv>();
        //_camSwitch.cameraOne = _currentCam;
        //_camSwitch.cameraChangeCounter();
        camScriptInst.views[0] = offsetCamShoulder.transform;
        camScriptInst.LetsGo();
        fov.camShoulder = camScriptInst;

        //if (_currentFov._actualTarget != null)
        //{
        //    _currentFov._actualTarget = _camMouv.targetObj;
        //}

    }

    private void ShootKill()
    {
        if (fov._actualTarget != null)
        {
            WaterWaste(waterWasteOnAction);
            StartCoroutine(KillNPC(1.5f));
        }
    }

    public void DontMove()
    {
        nav.isStopped = true;
        WalkTactical(false);
    }

    public Vector3 RandomPositionAroundPlayer()
    {
        Vector3 randomDirection = Random.insideUnitSphere * unitsRangeMovement;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        NavMesh.SamplePosition(randomDirection, out hit, unitsRangeMovement, 1);
        RaycastHit hitBlock;
        if (Physics.Raycast(hit.position, Vector3.down, out hitBlock, blockMask))
        {
            //Debug.Log("good");
            //Debug.Log(hitBlock.transform.gameObject);
            Vector3 posFinal = new Vector3(hitBlock.transform.position.x, hitBlock.transform.position.y + 1, hitBlock.transform.position.z);
            finalPosition = posFinal;
            return finalPosition;
        }
        else
        {
            Debug.Log("fuck");
            finalPosition = hit.position;
            return finalPosition;
        }
    }

    private void WaterWaste(float percents)
    {
        Water -= percents;
    }

    private void PassTurn()
    {
        actionPointIndex = actionPoint;
        if (StillYourTurn())
        {

        }
    }

    public void ChangeModePublic()
    {
        Debug.Log("free");
        TacticalAnim(false);
        TacticalMode = false;
        Run(false);
        WalkTactical(false);
        ResetAllPreview();
        SettingPathBool = false;
        fov._isActive = false;
        nav.ResetPath();
        camControl.ChangeMode();
        //StartCoroutine(changeCamMode(1f, false));
    }

    private  void ChangeMode()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (TacticalMode && !isOnCombat)
            {

                TacticalAnim(false);
                TacticalMode = false;
                Run(false);
                WalkTactical(false);
                ResetAllPreview();
                SettingPathBool = false;
                fov._isActive = false;
                nav.ResetPath();
                camControl.ChangeMode();
                //StartCoroutine(changeCamMode(1f, false));
            }
            else if(!TacticalMode)
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position, Vector3.down, out hit, blockMask))
                {
                    if(hit.transform.gameObject.layer == 8)
                    {
                        Debug.Log("tactic");
                        Run(false);
                        WalkTactical(false);
                        TacticalAnim(true);
                        fov._isActive = true;
                        TacticalMode = true;
                        camControl.ChangeMode();
                    }
                }
                //sStartCoroutine(changeCamMode(1f, true));
            }
        }
    }

    private void UpdateInpute()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !occuped)
        {
            if (fov.visibleTargets.ToArray().Length != 0)
            {
                Aim();
                occuped = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && occuped)
        {
            UnAim();
            selectDevice = false;
            selectInvisiblity = false;
            selectTrap = false;
            occuped = false;
            camControl.target = transform;
            if (selectDeviceScript != null)
                selectDeviceScript.ResetPreview();
            StartCoroutine(CamTargetPlayer());
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && !occuped)
        {
            if(deviceList.ToArray().Length != 0)
            {
                selectDevice = true;
                occuped = true;
                pathPreviewLine.positionCount = 0;
                pathWaypoint.Clear();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && !occuped && !system.cdInvisibilty)
        {
            occuped = true;
            selectInvisiblity = true;
            pathPreviewLine.positionCount = 0;
            pathWaypoint.Clear();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && !occuped && Trap != 0)
        {
            occuped = true;
            selectTrap = true;
            StartCoroutine(CamTargetPlayer());
            pathPreviewLine.positionCount = 0;
            pathWaypoint.Clear();
        }

        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    PassTurn();
        //}
    }

    #endregion

    #region Animations

    public float AnimationLength(string name)
    {
        float time = 0;
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;

        //for (int i = 0; i < ac.animationClips.Length; i++)
        //    if (ac.animationClips[i].name == name)
        //        time = ac.animationClips[i].length;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            Debug.Log(ac.animationClips[i].name);
        }
        return time;
    }

    public void Run(bool b)
    {
        anim.SetBool("Run", b);
    }

    public void WalkTactical(bool b)
    {
        anim.SetBool("crouchWalk", b);
    }

    public void CoverIdle(bool b)
    {
        anim.SetBool("coverIdle", b);
    }

    public void CoverCrouchIdle(bool b)
    {
        anim.SetBool("coverCrouchIdle", b);
    }

    public void TacticalAnim(bool b)
    {
        anim.SetBool("tactical", b);
    }

    public void Ladder(bool b)
    {
        anim.SetBool("ladder", b);
    }

    public void Shoot()
    {
        anim.SetTrigger("fire");
    }

    public void Death()
    {
        anim.SetTrigger("death");
    }

    public void Hack()
    {
        anim.SetTrigger("hack");
    }

    public void TrapAnim()
    {
        anim.SetTrigger("trap");
    }

    #endregion

    #region Coroutine
    IEnumerator LateUp(float time)
    {
        yield return new WaitForSeconds(time);
        SetPath();
        SetLimitsSign();
    }

    IEnumerator ChangeTurn()
    {
        yield return new WaitForSeconds(2);
        system.EndPlayerTurn();
        //turnPlayer = true;
        //cantMove = false;
        //SettingPathBool = false;
        //NextTurn();
    }

    IEnumerator CamTargetPlayer()
    {
        camControl.target = transform;
        targetPlayer = true;
        yield return new WaitForSeconds(0.5f);
        targetPlayer = false;
    }

    IEnumerator KillNPC(float time)
    {
        Transform npc;
        npc = fov._actualTarget;
        transform.LookAt(npc);
        
        yield return new WaitForSeconds(time);
        Shoot();
        npc.GetComponent<NPCcontroller>().GetKill(false);
        for (int i = 0; i < npc.GetComponent<NPCcontroller>().allyNPC.ToArray().Length; i++)
        {
            NPCcontroller npcAlly = npc.GetComponent<NPCcontroller>().allyNPC.ToArray()[i];
            npcAlly.alertedPos = transform.position;
            npcAlly.playerSpotted = true;
            npcAlly.GetAlerted();
        }
        yield return new WaitForSeconds(time);
        
        actionPointIndex++;
        if (StillYourTurn())
        {
            UnAim();
        }
    }

    IEnumerator getInvisible(float time)
    {
        yield return new WaitForSeconds(time);

        actionPointIndex++;
        if (StillYourTurn())
        {
            selectInvisiblity = false;
            selectDevice = false;
            occuped = false;
            camControl.target = transform;
            targetPlayer = false;
            //StartCoroutine(CamTargetPlayer());
        }
    }

    IEnumerator TrapEnable(float time, MeshRenderer mesh, TrapScript trap)
    {
        TrapAnim();
        yield return new WaitForSeconds(time);
        mesh.enabled = true;
        trap.active = true;
        actionPointIndex++;
        if (StillYourTurn())
        {
            selectInvisiblity = false;
            selectDevice = false;
            selectTrap = false;
            occuped = false;
            camControl.target = transform;
            targetPlayer = false;
            //StartCoroutine(CamTargetPlayer());
        }
    }
    #endregion

    #region Handles
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        if (TacticalMode && pathWaypoint.ToArray().Length != 0)
        {
            Vector3[] pw = pathWaypoint.ToArray();
            for (int i = 0; i < pathWaypoint.ToArray().Length; i++)
            {
                Handles.SphereHandleCap(-1, pw[i], Quaternion.identity, 0.25f, EventType.Repaint);
            }
            
        }

        //Handles.color = Color.red;
        //Handles.SphereHandleCap(-1, testVect, Quaternion.identity, 1, EventType.Repaint);
    

}
#endif
    #endregion
}
