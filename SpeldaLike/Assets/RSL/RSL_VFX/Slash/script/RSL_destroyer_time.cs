using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_destroyer_time : MonoBehaviour
{
    public float t;
 
    void Update ()
    {
        Destroy(gameObject, t);
    }
}
