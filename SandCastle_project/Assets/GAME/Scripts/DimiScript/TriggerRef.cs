using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRef : MonoBehaviour
{
    public bool isRefresh;
    private SytemTurn system;

    public List<NPCcontroller> npcArea = new List<NPCcontroller>();
    public List<Device> deviceArea = new List<Device>();
    public List<Detector> detectorArea = new List<Detector>();


    private List<NPCcontroller> npclist = new List<NPCcontroller>();
    private List<Device> deviceList = new List<Device>();


    private void OnEnable()
    {
        system = GameObject.Find("SystemTurn").GetComponent<SytemTurn>();
    }


    public void OnTriggerEnter(Collider col)
    {
        if(col.transform.gameObject.layer == 11)
        {
            CharacterControler player = col.GetComponent<CharacterControler>();

            for (int i = 0; i < player.deviceList.ToArray().Length; i++)
            {
                deviceList.Add(player.deviceList.ToArray()[i]);
            }

            player.deviceList.Clear();
            for (int i = 0; i < deviceArea.ToArray().Length; i++)
            {
                player.deviceList.Add(deviceArea.ToArray()[i]);
            }
            for (int i = 0; i < system.actualEnnemy.ToArray().Length; i++)
            {
                npclist.Add(system.actualEnnemy.ToArray()[i]);
                system.actualEnnemy.ToArray()[i].fov._isActive = false;
            }
            system.actualEnnemy.Clear();
            for (int i = 0; i < npcArea.ToArray().Length; i++)
            {
                system.actualEnnemy.Add(npcArea.ToArray()[i]);
                npcArea.ToArray()[i].fov._isActive = true;
            }

            for (int i = 0; i < system.actualEnnemy.ToArray().Length; i++)
            {
                system.actualEnnemy.ToArray()[i].allyNPC.Clear();
                for (int o = 0; o < system.actualEnnemy.ToArray().Length; o++)
                {
                    if(system.actualEnnemy.ToArray()[i] != system.actualEnnemy.ToArray()[o])
                        system.actualEnnemy.ToArray()[i].allyNPC.Add(system.actualEnnemy.ToArray()[o]);
                }
            }

            for (int i = 0; i < detectorArea.ToArray().Length; i++)
            {
                for (int o = 0; o < npcArea.ToArray().Length; o++)
                {
                    detectorArea.ToArray()[i].npcList.Add(npcArea.ToArray()[o]);
                }
            }
        }
    }


    public void OnTriggerExit(Collider col)
    {
        if (col.transform.gameObject.layer == 11 && isRefresh)
        {
            npcArea.Clear();
            deviceArea.Clear();
            for (int i = 0; i < deviceList.ToArray().Length; i++)
            {
                deviceArea.Add(deviceList.ToArray()[i]);
            }

            for (int i = 0; i < npclist.ToArray().Length; i++)
            {
                npcArea.Add(npclist.ToArray()[i]);
            }
        }
    }
}
