using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salutcestdiablox9 : MonoBehaviour
{
    public GameObject slashvfx;
    public GameObject slashvfx2;
    public GameObject slashvfx3;
    void Salutcestdiablox9_1()
    {
        Instantiate(slashvfx, transform.position, transform.rotation);
    }

    void Salutcestdiablox9_2()
    {
        Instantiate(slashvfx2, transform.position, transform.rotation);
    }
    void Salutcestdiablox9_3()
    {
        Instantiate(slashvfx3, transform.position, transform.rotation);
    }
}
