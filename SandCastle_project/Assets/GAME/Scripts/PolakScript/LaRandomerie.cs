using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaRandomerie : MonoBehaviour
{
    public AudioClip[] PlatformFS;
    public AudioClip[] SandFS;
    public AudioSource lasource; 
    public bool soundsGood = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        SoundRandom();
    }
    // Update is called once per frame
    void SoundRandom()
    {
        if ((soundsGood == true && Input.GetKey(KeyCode.Z)))
        {
            int StepPlat = Random.Range(0, 6);

            for (int i = 0; i < PlatformFS.Length; i++)
            {
                if (StepPlat == i)
                {
                    lasource.clip = PlatformFS[i];
                    lasource.Play();
                    soundsGood = false;
                    StartCoroutine(CooldownStep());
                }
            }
        }
    }

    IEnumerator CooldownStep()
    {
        yield return new WaitForSeconds(0.6f);
        
           soundsGood = true;
        
    }
}
