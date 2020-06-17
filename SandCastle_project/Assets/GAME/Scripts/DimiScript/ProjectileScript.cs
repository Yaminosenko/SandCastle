using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public Vector3 destination;
    public GameObject impact;
    public GameObject himself;
    private bool reach;
    private GameObject instanciat;



    private void Start()
    {
        transform.LookAt(destination);

    }

    private void Update()
    {
        if (!reach)
        {
            transform.Translate(0, 0, 100 * Time.deltaTime, Space.Self);

            float dist = Vector3.Distance(transform.position, destination);
            if (dist <= 1f)
            {

                reach = true;
                StartCoroutine(ImpactDelay());
                //Debug.Log("touché");
                //Destroy(gameObject);
            }
        }

        
    }

    IEnumerator ImpactDelay()
    {
        GameObject impactInst = Instantiate(impact, transform.position, Quaternion.identity);
        instanciat = impactInst;
        yield return new WaitForSeconds(0.1f);
        //instanciat.SetActive(false);
        Destroy(gameObject);
        //Destroy(instanciat);
    }
    

}
