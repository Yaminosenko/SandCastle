using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SytemTurn : MonoBehaviour
{
    public List<NPCcontroller> actualEnnemy = new List<NPCcontroller>();
    public CharacterControler player;
    public CameraController cam;

    private NPCcontroller npcTurn;
    private int nbrTurnMax;
    private int indexNbrTurn;


    private void Start()
    {
        nbrTurnMax = actualEnnemy.ToArray().Length;    
    }


    public void NextTurn()
    {
        NPCcontroller[] npcTab = actualEnnemy.ToArray();
        npcTurn = null;
        indexNbrTurn++;
        if(indexNbrTurn == nbrTurnMax)
        {
            cam.target = player.transform;
            StartCoroutine(waitEndTurnNPC());
            indexNbrTurn = 0;
        }
        else
        {
            for (int i = 0; i < npcTab.Length; i++)
            {
                //Debug.Log(i + " " + indexNbrTurn);
                if (i == indexNbrTurn)
                    npcTurn = npcTab[i];
            }
            npcTurn.system = GetComponent<SytemTurn>();
            npcTurn.Walk(false);
            cam.target = npcTurn.transform;
            npcTurn.yourTurn = true;
        }
    }

    public void EndPlayerTurn()
    {
        NPCcontroller[] npcTab = actualEnnemy.ToArray();
       
        npcTurn = npcTab[0];
        npcTurn.system = GetComponent<SytemTurn>();
        npcTurn.Walk(false);
        nbrTurnMax = actualEnnemy.ToArray().Length;
        cam.target = npcTurn.transform;
        cam.turnPlayer = false;
        npcTurn.yourTurn = true;
    }

    public void EndNpcTurn()
    {
        indexNbrTurn = 0; 
    }


    IEnumerator waitEndTurnNPC()
    {
        yield return new WaitForSeconds(2);
        cam.turnPlayer = true;
        player.turnPlayer = true;
        player.cantMove = false;
        //cantMove = false;
        player.SettingPathBool = false;
        //NextTurn();
    }
}
