using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public List<NPCcontroller> npcList = new List<NPCcontroller>();
    public Transform ViewVisualisation;
    public LayerMask groundMask;
    private FieldOfView fov;
    private CharacterControler player;

    private void OnEnable()
    {
        fov = GetComponent<FieldOfView>();
        player = fov.player;
        RaycastHit hit;
        if (Physics.Raycast(ViewVisualisation.position, Vector3.down,out hit, groundMask))
        {
            ViewVisualisation.position = new Vector3(ViewVisualisation.position.x, hit.transform.position.y + 0.2f, ViewVisualisation.position.z);
        }

    }


    public void AlertGuard()
    {
        Debug.Log("did it");
        for (int i = 0; i < npcList.ToArray().Length; i++)
        {
            npcList.ToArray()[i].alertedPos = player.transform.position;
            npcList.ToArray()[i].playerSpotted = true;
            npcList.ToArray()[i].GetAlerted();
        }
    }
}
