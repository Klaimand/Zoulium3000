using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_DEBUG_Inputs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            GameEvents.Instance.DisableGravity(0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            GameEvents.Instance.EnableGravity(0);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            GameEvents.Instance.DisableGravity(1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            GameEvents.Instance.EnableGravity(1);
        }
    }
}
