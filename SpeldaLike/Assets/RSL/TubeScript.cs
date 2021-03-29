using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeScript : MonoBehaviour
{
    void Start()
    {
        GetComponent<HingeJoint>().connectedBody = transform.parent.GetComponent<Rigidbody>();

    }
}