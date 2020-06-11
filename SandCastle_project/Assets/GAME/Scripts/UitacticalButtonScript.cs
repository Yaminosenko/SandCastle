using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UitacticalButtonScript : MonoBehaviour
{
    public GameObject shotPanel;
    public GameObject trapPanel;
    public GameObject invisiblePanel;
    public GameObject hackPanel;


    // Start is called before the first frame update
    void Start()
    {
        shotPanel.SetActive(false);
        trapPanel.SetActive(false);
        invisiblePanel.SetActive(false);
        hackPanel.SetActive(false);



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shot()
    {
        shotPanel.SetActive(true);
        trapPanel.SetActive(false);
        invisiblePanel.SetActive(false);
        hackPanel.SetActive(false);
       
    }

    public void Trap()
    {
        shotPanel.SetActive(false);
        trapPanel.SetActive(true);
        invisiblePanel.SetActive(false);
        hackPanel.SetActive(false);
    }

    public void Invisible()
    {
        shotPanel.SetActive(false);
        trapPanel.SetActive(false);
        invisiblePanel.SetActive(true);
        hackPanel.SetActive(false);
    }

    public void Hack()
    {
        shotPanel.SetActive(false);
        trapPanel.SetActive(false);
        invisiblePanel.SetActive(false);
        hackPanel.SetActive(true);
    }

}
