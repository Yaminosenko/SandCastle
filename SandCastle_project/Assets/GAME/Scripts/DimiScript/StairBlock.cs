using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class StairBlock : MonoBehaviour
{

    public Transform targetStair;
    public LayerMask layerMask;
    public Transform stair;
    public float angle;
 
    void Start()
    {
        //Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        //RaycastHit hit;
        //if(Physics.Raycast(pos, targetStair.TransformDirection(Vector3.forward), out hit, 2, layerMask))
        //{
            
        //}
    }

    //private void OnTriggerEnter(Collider col)
    //{
    //    if (col.gameObject.layer == 11)
    //    {
    //        //CharacterControler player = col.gameObject.GetComponent<CharacterControler>();
    //        //Debug.Log(col.gameObject + " " + player);
    //        //player.transform.rotation = Quaternion.LookRotation(stair.position);
    //        //player.nav.isStopped = true;
    //        //player.WalkTactical(false);
    //        //player.Ladder(true);
    //        //player.nav.speed = 1;
    //        //player.nav.isOnOffMeshLink;

    //    }
    //}

    //IEnumerator
}
