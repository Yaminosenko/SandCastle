using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShoulderMouv : MonoBehaviour
{
    public Transform[] views = new Transform[2];
    public float transitionSpeed;
    Transform currentView;
    public Transform targetObj;
    public int speed;
    public bool _isActive = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentView = views[0];
        }
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    currentView = views[1];
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    currentView = views[2];
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    currentView = views[3];
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    currentView = views[4];
        //}

        if (targetObj != null)
        {
            SmoothLookAt();
        }
    }

    public void LetsGo()
    {
        currentView = views[0];
    }

    void LateUpdate()
    {
        if (currentView != null /*&& targetObj == null*/)
        {
            transform.position = Vector3.Lerp(transform.position, currentView.position, Time.deltaTime * transitionSpeed);
            Vector3 currentAngle = new Vector3(Mathf.LerpAngle(transform.rotation.eulerAngles.x, currentView.transform.rotation.eulerAngles.x, Time.deltaTime * transitionSpeed),
                Mathf.LerpAngle(transform.rotation.eulerAngles.y, currentView.transform.rotation.eulerAngles.y, Time.deltaTime * transitionSpeed),
                Mathf.LerpAngle(transform.rotation.eulerAngles.z, currentView.transform.rotation.eulerAngles.z, Time.deltaTime * transitionSpeed));
            transform.eulerAngles = currentAngle;
        }
    }

    void SmoothLookAt()
    {
        var targetRotation = Quaternion.LookRotation(targetObj.position - transform.position);

        // Smoothly rotate towards the target point.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
    }


    public void DestroyObject()
    {
        if (_isActive == true)
        {
            Destroy(this.gameObject);
            _isActive = false;
        }
    }
}
