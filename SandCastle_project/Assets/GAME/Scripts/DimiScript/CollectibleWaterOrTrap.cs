using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleWaterOrTrap : MonoBehaviour
{
    public bool water;
    public int value;
    private bool once;

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.layer == 11 && !once)
        {
            CharacterControler player = col.GetComponent<CharacterControler>();

            if (water)
            {
                if(player.Water >= 100)
                {

                }
                else if(100 - player.Water  < value)
                {
                    player.Water += 100 - player.Water;
                    once = true;
                }
                else
                {
                    once = true;
                    player.Water += value;
                }
                Mathf.Clamp(player.Water, 0, 100);
            }
            else
            {
                player.Trap++;
                once = true;
            }
        }
    }
}
