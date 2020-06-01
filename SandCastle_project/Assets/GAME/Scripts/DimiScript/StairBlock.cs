using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class StairBlock : MonoBehaviour
{

    public Transform targetStair;
    public LayerMask layerMask;
 
    void Start()
    {
        //navLink = GetComponent<NavMeshLink>();
        //navLink.startPoint = transform.position;
        //targetStair = GetComponentInChildren<Transform>();
        RaycastHit hit;
        if(Physics.Raycast(targetStair.position, targetStair.TransformDirection(Vector3.down), out hit, 2, layerMask))
        {
            if (hit.transform.gameObject.layer == 9)
            {

            }
                //navLink.endPoint = hit.point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
