using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleScript : MonoBehaviour
{


    public CharacterControler player;
    public UitacticalButtonScript UI;
    private bool go;
    private float value;

    private void Start()
    {
        player = GameObject.Find("Character").GetComponentInChildren<CharacterControler>();
        UI = player.UI;
    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 11)
        {
            player.dead = true;
            StartCoroutine(blackScreen());
            player.offsetCamShoulder.transform.position = new Vector3(player.offsetCamShoulder.transform.position.x, player.offsetCamShoulder.transform.position.y + 1, player.offsetCamShoulder.transform.position.z);
            player.offsetCamShoulder.transform.rotation = Quaternion.Euler(player.offsetCamShoulder.transform.rotation.x+20, player.offsetCamShoulder.transform.rotation.y, player.offsetCamShoulder.transform.rotation.z);
            player.InstanciateCamera();
        }
    }

    private void Update()
    {
        transEffect();
    }

    private void transEffect()
    {
        
            Image trans = UI.blackScreen;

        if (go)
        {
            value = Mathf.Lerp(value, 1, 1 * Time.deltaTime);
        }

        Debug.Log(value);
        trans.color = new Color(trans.color.r, trans.color.g, trans.color.b, value);
    }

    IEnumerator blackScreen()
    {
        yield return new WaitForSeconds(2);
        go = true;
        yield return new WaitForSeconds(2);
        for (int i = 0; i < UI.video.Length; i++)
        {
            UI.video[i].SetActive(true);
        }

    }
}
