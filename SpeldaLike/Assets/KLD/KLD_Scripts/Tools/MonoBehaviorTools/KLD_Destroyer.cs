using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_Destroyer : MonoBehaviour
{

    [SerializeField] float time = 2f;
    [SerializeField] bool dontDestroyAtStart = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!dontDestroyAtStart)
        {
            Destroy(gameObject, time);
        }
    }

    public void Destroy(float _t)
    {
        Destroy(gameObject, _t);
    }
}
