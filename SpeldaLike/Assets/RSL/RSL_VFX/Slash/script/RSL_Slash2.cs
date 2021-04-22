using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_Slash2 : MonoBehaviour
{
    public GameObject slashVFX;
  
    void Salutatouscestsqueezie2 ()
    {
        Instantiate(slashVFX, transform.position, transform.rotation);
    }
}
