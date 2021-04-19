using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_destroyer : MonoBehaviour
{
    public float t;
    void Update()
    {
        if (Input.GetKeyDown("Numpad3"))
        {
            Destroy(gameObject);
        }
    }
}
