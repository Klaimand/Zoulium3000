using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_BossShield : MonoBehaviour
{
    [SerializeField] Animator a;

    public void FadeOut()
    {
        a.SetTrigger("FadeOut");
    }
}
