using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_SlamAttaque : MonoBehaviour
{
    public GameObject slamVFX;
    void SlamAttaque()
    {
        Instantiate(slamVFX, transform.position, transform.rotation);
    }
}
