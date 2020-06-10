using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cover : MonoBehaviour
{
    public bool little;
    public Image littleCover;
    public Image bigCover;
    public CharacterControler player;
    public GameObject offsetLocalPos;
    private GameObject defaultCover;


    void Start()
    {
        if (little)
            defaultCover = littleCover.gameObject;
        else
            defaultCover = bigCover.gameObject;

        defaultCover.gameObject.SetActive(false);
    }

    private void Update()
    {
        
    }

    private void OnMouseOver()
    {
        if (player.TacticalMode)
        {
            defaultCover.gameObject.SetActive(true);
            RaycastHit hit;
            if(Physics.Raycast(transform.position,Vector3.down,out hit))
            {
                if(hit.transform.gameObject.layer == 8)
                {
                    hit.transform.gameObject.GetComponent<Block>().Over();
                }
            }
        }
    }

    private void OnMouseExit()
    {
        defaultCover.gameObject.SetActive(false);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.transform.gameObject.layer == 8)
            {
                hit.transform.gameObject.GetComponent<Block>().Exit();
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 11 && col.GetComponent<CharacterControler>() != null)
        {
            if (little)
                col.GetComponent<CharacterControler>().isCover = 1;
            else
                col.GetComponent<CharacterControler>().isCover = 2;

            //col.GetComponent<CharacterControler>().CrouchIdle(true);
        }
        else if (col.gameObject.layer == 12 && col.GetComponent<NPCcontroller>() != null)
        {
            col.GetComponent<NPCcontroller>().isOnCover = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == 11 && col.GetComponent<CharacterControler>() != null)
        {
            col.GetComponent<CharacterControler>().isCover = 0;
            //col.GetComponent<CharacterControler>().CrouchIdle(false);
        }
        else if (col.gameObject.layer == 12 && col.GetComponent<NPCcontroller>() != null)
        {
            col.GetComponent<NPCcontroller>().isOnCover = false;
        }
    }
}
