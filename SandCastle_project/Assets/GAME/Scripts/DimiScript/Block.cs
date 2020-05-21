using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Transform[] offset = new Transform[4];
    public LayerMask layerMask;


    private float[] offsetDistance = new float[4]; 

    private void OnEnable()
    {


        for (int i = 0; i < offset.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(offset[i].transform.position, offset[i].transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
            {
                //Debug.DrawRay(offset[i].transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                //Debug.Log("Did Hit");

                offsetDistance[i] = hit.distance;

            }
            else
            {
              
            }
        }


    }
}

