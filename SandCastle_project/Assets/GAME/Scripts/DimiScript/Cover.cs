using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cover : MonoBehaviour
{
    public bool little;
    public Image littleCover;
    public Image bigCover;
    private GameObject defaultCover;

    void Start()
    {
        if (little)
            defaultCover = littleCover.gameObject;
        else
            defaultCover = bigCover.gameObject;

        //defaultCover.gameObject.SetActive(true);
    }


    private void OnMouseOver()
    {
        defaultCover.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        defaultCover.gameObject.SetActive(false);
    }
}
