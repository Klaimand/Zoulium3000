using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAt : MonoBehaviour
{
    public Transform target;
    void Update()
    {
        Vector3 relativPos = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(relativPos);
    }
}
