using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class casuitpp08issou : MonoBehaviour
{
    [SerializeField] Transform cibleTransform = null;
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, cibleTransform.position.z);
    }
}
