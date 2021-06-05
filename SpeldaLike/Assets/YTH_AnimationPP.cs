using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YTH_AnimationPP : MonoBehaviour
{
    [SerializeField]
    Animator PPanim;

    [SerializeField]
    public bool talking;

    private void Start()
    {
        PPanim = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        PPanim.SetBool("talking", talking);
    }

    public void talkTrue()
    {
        talking = true;
    }

    public void talkFalse()
    {
        talking = false;
    }
}
