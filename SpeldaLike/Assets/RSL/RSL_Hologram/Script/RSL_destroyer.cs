using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RSL_destroyer : MonoBehaviour
{
    public float t;
    void Update()
    {
        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            Destroy(gameObject);
        }
    }
}
