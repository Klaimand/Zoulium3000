using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMort : MonoBehaviour
{
    public GameObject laVersionQuiFaitBOOM;
    public float t = 10f;

    void Start()
    {
        StartCoroutine(delayedStart());
    }
    IEnumerator delayedStart()
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
        Instantiate(laVersionQuiFaitBOOM, transform.position, transform.rotation);
    }   
}