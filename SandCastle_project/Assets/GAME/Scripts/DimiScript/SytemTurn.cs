using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SytemTurn : MonoBehaviour
{
    public List<NPCcontroller> actualEnnemy = new List<NPCcontroller>();
    public CharacterControler player;
    public CameraController cam;
    public bool cdInvisibilty;
    public int coolDownInvisibility = 4;

    private NPCcontroller npcTurn;
    [SerializeField] private int nbrTurnMax;
    [SerializeField] private int indexNbrTurn;
    private int coolDownInv;
    private int deadIndex;
    public AudioClip[] ambianceSound;
    public AudioSource ambianceSource;
    public AudioSource speakSourceSystem;




    private void Start()
    {
        nbrTurnMax = actualEnnemy.ToArray().Length;
        cam = Camera.main.GetComponent<CameraController>();
    }


    public void NextTurn()
    {
        Debug.Log("nextTurn");
        NPCcontroller[] npcTab = actualEnnemy.ToArray();
        npcTurn = null;
        indexNbrTurn++;
        if (indexNbrTurn == nbrTurnMax)
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
                {
                    if (!npcTab[i].dead)
                        npcTurn = npcTab[i];
                    else
                    {
                        indexNbrTurn++;
                        if (indexNbrTurn == nbrTurnMax)
                        {
                            cam.target = player.transform;
                            StartCoroutine(waitEndTurnNPC());
                            indexNbrTurn = 0;
                            return;
                        }
                    }
                }
                //else if (indexNbrTurn == nbrTurnMax)
                //{
                //    cam.target = player.transform;
                //    StartCoroutine(waitEndTurnNPC());
                //    indexNbrTurn = 0;
                //    return;
                //}
            }
            npcTurn.system = GetComponent<SytemTurn>();
            npcTurn.Walk(false);
            cam.target = npcTurn.transform;
            npcTurn.yourTurn = true;
        }
    }

    public void EndPlayerTurn()
    {
        if(actualEnnemy.ToArray().Length == 0)
        {
            cam.target = player.transform;
            StartCoroutine(waitEndTurnNPC());
            indexNbrTurn = 0;
            return;
        }
        NPCcontroller[] npcTab = actualEnnemy.ToArray();
        npcTurn = npcTab[0];
        nbrTurnMax = actualEnnemy.ToArray().Length;
        cam.turnPlayer = false;

        if (npcTurn.dead == true)
        {
            NextTurn();
            return;
        }
        npcTurn.system = GetComponent<SytemTurn>();
        npcTurn.Walk(false);
        cam.target = npcTurn.transform;
        npcTurn.yourTurn = true;
    }

    public void EndNpcTurn()
    {
        indexNbrTurn = 0; 
    }

    public void KillPlayerSystem(NPCcontroller npc)
    {
        player.DontMove();
        cam.turnPlayer = false;
        player.turnPlayer = false;
        cam.target = npc.transform;
    }

    public void Restart()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    private void AutoFreeMode()
    {
        deadIndex++;
        Debug.Log(deadIndex + " " + nbrTurnMax);
        //if (deadIndex == nbrTurnMax)
        //{
        //    player.isOnCombat = false;
        //    player.ChangeModePublic();
        //    deadIndex = 0;
        //    indexNbrTurn = 0;
        //    if (cdInvisibilty)
        //    {
        //        coolDownInv++;
        //        if (coolDownInv == coolDownInvisibility)
        //            cdInvisibilty = false;

        //        if (coolDownInv == player.invisibilityDuration)
        //            player.isInvisble = false;
        //    }
        //    cam.turnPlayer = true;
        //    player.turnPlayer = true;
        //    player.cantMove = false;
        //    //cantMove = false;
        //    player.SettingPathBool = false;
        //    return;
        //}
    }

    public void RefreshSystem()
    {
        nbrTurnMax = 0;
        indexNbrTurn = 0;
        cam.target = player.transform;
        player.turnPlayer = true;
        player.cantMove = false;
        //cantMove = false;
        player.SettingPathBool = false;
    }

    public IEnumerator NPCgetKill(float time)
    {
        yield return new WaitForSeconds(time);
        NextTurn();
    }

    IEnumerator waitEndTurnNPC()
    {
        yield return new WaitForSeconds(2);
        if (cdInvisibilty)
        {
            coolDownInv++;
            if(coolDownInv == coolDownInvisibility)
                cdInvisibilty = false;

            if (coolDownInv == player.invisibilityDuration)
            {
                player.isInvisble = false;
                player.RefreshMat();
            }
        }
        cam.turnPlayer = true;
        player.turnPlayer = true;
        player.cantMove = false;
        //cantMove = false;
        player.SettingPathBool = false;
        //NextTurn();
    }
}
