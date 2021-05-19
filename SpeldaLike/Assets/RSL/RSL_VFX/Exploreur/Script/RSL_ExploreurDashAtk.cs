using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_ExploreurDashAtk : MonoBehaviour
{
    public GameObject vfx;

    void SalutatouscestGuzzDxencompagniedePorto()
    {
        Instantiate(vfx, transform.position, transform.rotation);
    }
}
