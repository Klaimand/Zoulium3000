using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_Slash3 : MonoBehaviour
{
    public GameObject slashVFX;
  
    void Salutatouscestsqueezie3 ()
    {
        Instantiate(slashVFX, transform.position, transform.rotation);
    }
}
