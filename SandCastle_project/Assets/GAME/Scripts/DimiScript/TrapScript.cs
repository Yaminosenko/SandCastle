using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    public LayerMask npcMask;
    public CharacterControler player;
  

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 12)
        {
            NPCcontroller npc = col.GetComponent<NPCcontroller>();
            for (int i = 0; i < npc.allyNPC.ToArray().Length; i++)
            {
                npc.allyNPC.ToArray()[i].alertedPos = player.RandomPositionAroundPlayer();
                npc.allyNPC.ToArray()[i].GetAlerted();
            }
            npc.GetKill(false);
            
            Debug.Log(col.gameObject.layer);
            //npc.system.NextTurn();
            StartCoroutine(NPCgetKill(2, npc));
            //explosion

            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }


    IEnumerator NPCgetKill(float time, NPCcontroller npc)
    {
        yield return new WaitForSeconds(time);
        npc.ResetVariables();
        npc.system.NextTurn();
        Destroy(gameObject);
    }
}
