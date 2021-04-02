using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RSL_apparitionvfx : MonoBehaviour
{
    public GameObject vfx;

    void Start()
    {
        
    }

    void Update()
    {
        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            Instantiate(vfx, transform.position, Quaternion.identity);
        }
    }
}
