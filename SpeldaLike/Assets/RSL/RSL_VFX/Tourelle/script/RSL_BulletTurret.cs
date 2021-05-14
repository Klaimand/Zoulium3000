using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_BulletTurret : MonoBehaviour
{
    public Vector3 shootDir;
    public float moveSpeed;

    void Update()
    {
        transform.position += shootDir * moveSpeed * Time.deltaTime;
    }
}
