using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YTH_AnimationPP : MonoBehaviour
{
    [SerializeField]
    Animator PPanim;

    public void talk(bool _talking)
    {
        PPanim?.SetBool("talking", _talking);
    }

    public void wake(bool _wake)
    {
        PPanim?.SetBool("wake", _wake);
    }
}
