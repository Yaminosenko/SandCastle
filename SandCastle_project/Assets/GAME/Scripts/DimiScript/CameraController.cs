using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables 

    [Header("General")]
    public Transform player;
    private Transform target;
    public bool tacticalEnable = false;

    [Header("FreeMode")]
    public float defaultAngleFree = 20;
    public float sensibilityRotateValue = 360;
    public float distToTarget = 10;
    public float returnSpeed = 9;

    [Header("TacticalMode")]
    public float defaultAngleTactical = 45;
    public bool TargetFollowing = false;
    public Transform targetToFollow;
    public Transform offsetRotate;
    public bool rotateVersion = false;

    public KeyCode moveUpCam;
    public KeyCode moveDownCam;
    public KeyCode moveRightCam;
    public KeyCode moveLeftCam;
    public float movementSpeed = 50;
    public float ScreenEdgeBorderSize = 25;
    public float LimiteX;
    public float LimiteZ;
    public KeyCode rotateLeft;
    public KeyCode rotateRight;
    public float keyBoardRotateSpeed = 100;
    public KeyCode zoomIn;
    public KeyCode zoomOut;
    public float keyBoardZoomSpeed = 25;
    public float minY;
    public float maxY;



    //private 
    private bool rotateCD;
    private float angleRotate;
    private bool changeModeCD;
    private float angleMode;
   
    #endregion

    #region Start & Update 

    private void OnEnable()
    {
        target = player;
        offsetRotate.position = player.position;
    }
    void LateUpdate()
    {
        if (!tacticalEnable)
        {
            RotateWithMouseAxis();
            FollowTarget();
        }
        else
        {
            UpdateCam();
        }

        ChangeMode();
    }
    #endregion


    #region Free Mode

    void RotateWithMouseAxis()
    {
        float mouseX = sensibilityRotateValue * Input.GetAxis("Mouse X");
        transform.RotateAround(target.position, Vector3.up, mouseX * Time.deltaTime);


        Vector3 e = transform.eulerAngles;
        e.x = 0;
        target.eulerAngles = e;
        //player.eulerAngles = e;
    }

    void FollowTarget()
    {
        Vector3 followDist = target.position - transform.forward * distToTarget;
        transform.position = Vector3.Lerp(transform.position, followDist, returnSpeed * Time.deltaTime);
    }




    #endregion

    #region Tactical Mode

    private void UpdateCam()
    {
        MoveCam();
        RotateCam();
    }

    private void MoveCam()
    {
        Vector3 pos = transform.position;
        float scroll = Input.GetAxis("Mouse ScrollWheel") * keyBoardZoomSpeed * 100 * Time.deltaTime;
        pos.y -= scroll ;
        float y = pos.y;

        //Mathf.Lerp(pos.y, (scroll * keyBoardZoomSpeed * 100 * Time.deltaTime), 0.5f);

        if (Input.GetKey(moveUpCam) || Input.mousePosition.y >= Screen.height - ScreenEdgeBorderSize)
            pos += transform.forward * movementSpeed * Time.deltaTime;

        if (Input.GetKey(moveDownCam) || Input.mousePosition.y <= ScreenEdgeBorderSize)
            pos -= transform.forward * movementSpeed * Time.deltaTime;

        if (Input.GetKey(moveRightCam) || Input.mousePosition.x >= Screen.width - ScreenEdgeBorderSize)
            pos += transform.right *  movementSpeed * Time.deltaTime;

        if (Input.GetKey(moveLeftCam) || Input.mousePosition.x <= ScreenEdgeBorderSize)
            pos -= transform.right * movementSpeed * Time.deltaTime;



        // pos.y = Mathf.Clamp(pos.y, minY, maxY);
        //pos.x = Mathf.Clamp(pos.x, -LimiteX, LimiteX);
        // pos.z = Mathf.Clamp(pos.x, -LimiteZ, LimiteZ);

        pos.y = y;
        transform.position = pos;
    }

    private void RotateCam()
    {
        if (rotateVersion)
        {
            if (Input.GetKey(rotateRight))
                transform.RotateAround(offsetRotate.position, Vector3.up, keyBoardRotateSpeed * Time.deltaTime);

            if (Input.GetKey(rotateLeft))
                transform.RotateAround(offsetRotate.position, Vector3.up, -keyBoardRotateSpeed * Time.deltaTime);
        }
        else
        {
            if (Input.GetKey(rotateRight) && !rotateCD)
                StartCoroutine(RotateSmooth(0.5f, true));

            if (Input.GetKey(rotateLeft) && !rotateCD)
                StartCoroutine(RotateSmooth(0.5f, false));

            if(rotateCD)
                transform.RotateAround(offsetRotate.position, Vector3.up, 180 * Time.deltaTime);
        }
     
    }

    private void FollowTargetTactical()
    {
        Vector3 followDist = target.position - transform.forward * distToTarget;
        transform.position = Vector3.Lerp(transform.position, followDist, returnSpeed * Time.deltaTime);
    }

    private void ChangeMode()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (tacticalEnable)
            {
                tacticalEnable = false;
                StartCoroutine(changeCamMode(1f, false));
            }
            else
            {
                tacticalEnable = true;
                StartCoroutine(changeCamMode(1f, true));
            }
        }

        if (changeModeCD)
        {
           Debug.Log(transform.eulerAngles.x + " " + angleMode);
           float rot =  Mathf.Lerp(transform.eulerAngles.x, angleMode, 5 * Time.deltaTime);
           transform.rotation = Quaternion.Euler(rot, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
    #endregion

    #region Coroutine
    IEnumerator RotateSmooth(float time, bool right)
    {
        rotateCD = true;
        if (right)
        {
            angleRotate = 180;
        }
        else
        {
            angleRotate = -180;
        }
        yield return new WaitForSeconds(time);
        rotateCD = false;
    }

    IEnumerator changeCamMode(float time, bool mode)
    {
        changeModeCD = true;
        if (mode)
            angleMode = defaultAngleTactical;
        else
            angleMode = defaultAngleFree;

        yield return new WaitForSeconds(time);
        transform.rotation = Quaternion.Euler(angleMode, transform.eulerAngles.y, transform.eulerAngles.z);
        changeModeCD = false;
    }
    #endregion
}
