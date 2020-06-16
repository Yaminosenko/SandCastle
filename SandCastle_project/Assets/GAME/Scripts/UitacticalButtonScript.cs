using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UitacticalButtonScript : MonoBehaviour
{
    public GameObject shotPanel;
    public GameObject trapPanel;
    public GameObject invisiblePanel;
    public GameObject hackPanel;

    public GameObject shotButton;
    public GameObject trapButton;
    public GameObject invisibeButton;
    public GameObject hackButton;

    public GameObject passButton;
    public GameObject tacticalButton;

    public Image water;
    public TextMeshProUGUI trapNumber;
    public TextMeshProUGUI targetNumber;
    public CharacterControler player;
    private CameraController cam;


    // Start is called before the first frame update
    void Start()
    {
        Refresh();
        player = GameObject.Find("Character").GetComponentInChildren<CharacterControler>();
        cam = Camera.main.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        string trapCount = player.Trap.ToString();
        trapNumber.SetText(trapCount);
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


    public void PassTurn()
    {
        player.PassTurn();
    }

    public void CameraRotateRight()
    {

    }

    public void Refresh()
    {
        shotPanel.SetActive(false);
        trapPanel.SetActive(false);
        invisiblePanel.SetActive(false);
        hackPanel.SetActive(false);
    }

    public void Tactical(bool b)
    {
        Refresh();

        shotButton.SetActive(b);
        trapButton.SetActive(b);
        invisibeButton.SetActive(b);
        hackButton.SetActive(b);

        passButton.SetActive(b);
        tacticalButton.SetActive(b);
    }

}
