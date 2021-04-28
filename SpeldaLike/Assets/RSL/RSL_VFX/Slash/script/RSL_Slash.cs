using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_Slash : MonoBehaviour
{
    public GameObject slashVFX;
  
    void Salutatouscestsqueezie ()
    {
        Instantiate(slashVFX, transform.position, transform.rotation);
    }
}
