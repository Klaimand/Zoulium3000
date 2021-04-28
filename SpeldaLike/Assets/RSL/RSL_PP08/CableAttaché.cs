using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableAttaché : MonoBehaviour
{
    public Transform attache;

    void Update()
    {
        transform.position = attache.position;
        
    }
}
