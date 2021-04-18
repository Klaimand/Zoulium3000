using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_MortEnnnemi : MonoBehaviour
{
    public GameObject ennemiPhysique;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Destroy(gameObject);
            Instantiate(ennemiPhysique, transform.position, transform.rotation);
        }
    } 
}
