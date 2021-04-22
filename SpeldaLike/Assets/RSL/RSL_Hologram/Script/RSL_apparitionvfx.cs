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
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Instantiate(vfx, transform.position, Quaternion.identity);
        }
    }
}
