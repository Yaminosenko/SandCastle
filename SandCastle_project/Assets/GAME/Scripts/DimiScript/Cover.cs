﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cover : MonoBehaviour
{
    public bool little;
    public Image littleCover;
    public Image bigCover;
    public CharacterControler player;
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
        if(player.FreeMode)
            defaultCover.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        defaultCover.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 11 && col.GetComponent<CharacterControler>() != null)
        {
            if (little)
                col.GetComponent<CharacterControler>().isCover = 1;
            else
                col.GetComponent<CharacterControler>().isCover = 2;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == 11 && col.GetComponent<CharacterControler>() != null)
        {
            col.GetComponent<CharacterControler>().isCover = 0;
        }
    }
}
