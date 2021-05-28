using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_OuvrierAttackEventInst : MonoBehaviour
{
    KLD_Ouvrier ouvrier;

    void Start()
    {
        ouvrier = transform.parent.GetComponent<KLD_Ouvrier>();
    }

    public void SpawnAttackZoneInst()
    {
        ouvrier.SpawnAttackVFX();
    }
}
