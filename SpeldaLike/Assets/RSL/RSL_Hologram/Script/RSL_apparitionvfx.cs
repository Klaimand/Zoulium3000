using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_apparitionvfx : MonoBehaviour
{
    public GameObject vfx;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Instantiate(vfx, transform.position, Quaternion.identity);
        }
    }
}
