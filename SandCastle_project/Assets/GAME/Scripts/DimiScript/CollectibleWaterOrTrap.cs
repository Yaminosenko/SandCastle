using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleWaterOrTrap : MonoBehaviour
{
    public bool water;
    public int value;


    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 11)
        {
            CharacterControler player = col.GetComponent<CharacterControler>();

            if (water)
            {
                player.Water += value;
            }
            else
            {
                player.Trap++;
            }
        }
    }
}
