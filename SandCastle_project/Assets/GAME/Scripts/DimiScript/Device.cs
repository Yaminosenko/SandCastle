using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Device : MonoBehaviour
{
    public int AreaOfEffect;
    public Transform[] npcToDistract;
    public float radiusRange;
    public LayerMask npcMask;
    public bool isHack;
    private CharacterControler player;
    private bool activate;

    private void OnEnable()
    {
        player = GameObject.Find("Character").GetComponentInChildren<CharacterControler>();
    }


    public void Update()
    {
        if (activate)
            ActivateDevice();
    }
    public void HackDevice()
    {
        player.deviceList.Add(this);
        isHack = true;
    }

    public void PreviewRangeDevice()
    {
        //show preview of the range & evry npc on it
    }

    public void ResetPreview()
    {
        //reset preview of the range when activate or change
    }

    public void ActivateDevice()
    {
        Collider[] npcToDistract = Physics.OverlapSphere(transform.position, radiusRange, npcMask);

        for (int i = 0; i < npcToDistract.Length; i++)
        {
            NPCcontroller npc = npcToDistract[i].GetComponent<NPCcontroller>();
            npc.distractPos = transform.position;
            NavMeshHit hit;
            Vector3 randomDirection = Random.insideUnitSphere * 2;
            Vector3 originGround = transform.position;
            //originGround.y = originGround.y + 0.5f;
            originGround += randomDirection;
            NavMesh.SamplePosition(originGround, out hit, 30, 1);
            Vector3 targetPosToFollow = hit.position;
            targetPosToFollow.y = targetPosToFollow.y + 0.5f;
            npc.distractPos = targetPosToFollow;
            npc.HeardSomething();
        }
    }

    public void SecuSound()
    {

    }


    public void OnTriggerStay(Collider col)
    {
        //show UI Use F to Hack
        if (!player.TacticalMode && !isHack)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                HackDevice();
            }
        }
    }

    public void OnTriggerExit(Collider col)
    {
        //delete UI Use F to Hack
        
    }

    IEnumerator spawnSound(float time)
    {
        activate = true;
        yield return new WaitForSeconds(time);
        activate = false;
    }
}
