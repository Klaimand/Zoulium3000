using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_LrAnimInst : MonoBehaviour
{

    [SerializeField] KLD_LaserBoss laser;

    public void StopLaser()
    {
        laser.StopLaser();
    }

    public void EndLaser()
    {
        laser.EndLaser();
    }
}
