using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_TurretFiring : MonoBehaviour
{
    public GameObject MuzzleFlashVfx;
    public GameObject ProjectileVfx;
    public GameObject ImpactVFX;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            Instantiate(MuzzleFlashVfx, transform.position, transform.rotation);
            Instantiate(ProjectileVfx, transform.position, transform.rotation);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            Instantiate(ImpactVFX, transform.position, transform.rotation);
        }
    }
}