using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class StairBlock : MonoBehaviour
{

    public Transform offset;
    public bool mirror;
    public float angle = 0;
    
 
    void Start()
    {
        //int a = Random.RandomRange(0, 5);
        //for (int i = 0; i < .Lenght; i++)
        //{
        //    if(a == i)
        //    {

        //    }
        //}
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 11)
        {
            CharacterControler player = col.gameObject.GetComponent<CharacterControler>();
            if(player.ladderBool != true)
            {
                if (mirror)
                    player.Ladder(true);
                else
                    player.LadderMirror(true);
                player.transform.rotation = Quaternion.Euler(player.transform.rotation.x, angle, player.transform.rotation.z);
                player.nav.enabled = false;
                player.ladderTp = offset;
                player.ladderTarget = new Vector3(player.transform.position.x, offset.position.y, player.transform.position.z);
                player.ladderBool = true;
            }
            else
            {
                player.nav.enabled = true;
                player.ladderBool = false;
                player.Ladder(false);
                player.LadderMirror(false);
            }
        }
    }

    //IEnumerator
}
