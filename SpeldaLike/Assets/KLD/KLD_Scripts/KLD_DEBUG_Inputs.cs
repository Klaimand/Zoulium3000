using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KLD_DEBUG_Inputs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.isPressed)
        {
            GameEvents.Instance.DisableGravity(0);
        }
        if (Keyboard.current.digit2Key.isPressed)
        {
            GameEvents.Instance.EnableGravity(0);
        }

        if (Keyboard.current.digit3Key.isPressed)
        {
            GameEvents.Instance.DisableGravity(1);
        }
        if (Keyboard.current.digit4Key.isPressed)
        {
            GameEvents.Instance.EnableGravity(1);
        }
    }
}
