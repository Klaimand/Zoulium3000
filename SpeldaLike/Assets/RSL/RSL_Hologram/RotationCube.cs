using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCube : MonoBehaviour
{
    public float x;
    public float y;
    public float z;

    public float amplitude;
    public float frequency;

    Vector3 posOffset = new Vector3();
    Vector3 temPos = new Vector3();

    void Start()
    {
        posOffset = transform.position;
    }
    void Update()
    {
        transform.Rotate(x, y, z * Time.deltaTime);
        temPos = posOffset;
        temPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = temPos;
    }
}
