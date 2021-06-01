using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_PlayerAttackInst : MonoBehaviour
{

    KLD_PlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = transform.parent.GetComponent<KLD_PlayerController>();
    }

    public void InstAttackZone(int _attackIndex)
    {
        controller.InstantiateAttack(_attackIndex);
    }

    public void InstAttackVFX(int _attackIndex)
    {
        controller.InstantiateAttackVFX(_attackIndex);
    }

    public void PullMastodonte()
    {
        controller.InvokeMastodonteAnchorEvent();
    }
}
