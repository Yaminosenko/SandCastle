using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Device : MonoBehaviour
{
    public int AreaOfEffect;
    public Transform[] npcToDistract;
    public float radiusRange;
    public LayerMask npcMask;
    public bool isHack;
    private CharacterControler player;
    private bool activate;
    public Image preview;

    private void OnEnable()
    {
        player = GameObject.Find("Character").GetComponentInChildren<CharacterControler>();

        preview.rectTransform.localScale = new Vector3(radiusRange * 3, radiusRange * 3, radiusRange * 3);
        preview.gameObject.SetActive(false);
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
        preview.gameObject.SetActive(true);
    }

    public void ResetPreview()
    {
        preview.gameObject.SetActive(false);
    }

    public void ActivateDevice()
    {
        Vector3 pos = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z + 0.5f);
        Collider[] npcToDistract = Physics.OverlapSphere(pos, radiusRange, npcMask);


        for (int i = 0; i < npcToDistract.Length; i++)
        {
            NPCcontroller npc = npcToDistract[i].GetComponent<NPCcontroller>();
            //npc.distractPos = transform.position;
            NavMeshHit hit;
            Vector3 randomDirection = Random.insideUnitSphere * 2;
            Vector3 originGround = pos;
            //originGround.y = originGround.y + 0.5f;
            originGround += randomDirection;
            NavMesh.SamplePosition(originGround, out hit, 30, 1);
            Vector3 targetPosToFollow = hit.position;
            targetPosToFollow.y = targetPosToFollow.y + 0.5f;
            Debug.Log(targetPosToFollow);
            npc.distractPos = targetPosToFollow;
            npc.HeardSomething();
        }
    }

    public void SecuSound()
    {
        StartCoroutine(spawnSound(0.5f));
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
        //Debug.Log(activate);
        yield return new WaitForSeconds(time);
        activate = false;
    }

    #region Handles 
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 pos = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z + 0.5f);
        
        Handles.color = Color.magenta;
        Handles.RadiusHandle(Quaternion.identity, pos, radiusRange);
           
        
        //Handles.RadiusHandle(Quaternion.identity, radiusKillCenter.position, radiusKillSphere);

        //Handles.SphereHandleCap(-1, crosshair.transform.position, Quaternion.identity, 0.5f, EventType.Repaint);
    }
#endif
    #endregion
}
