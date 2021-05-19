using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_Rotate : MonoBehaviour
{

    [SerializeField] Vector3 rotation = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }
}
