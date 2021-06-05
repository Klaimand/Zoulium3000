using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YTH_AnimationPP : MonoBehaviour
{
    [SerializeField]
    Animator PPanim;

    public void talkTrue(bool _talking)
    {
        PPanim?.SetBool("talking", _talking);
    }
}
