using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_DissolveBodyMesh : MonoBehaviour
{
    [SerializeField] MeshRenderer r;

    [SerializeField] float dissolveTime = 2f;
    float t;

    [SerializeField] bool destroyOnFinish = true;

    // Start is called before the first frame update
    void Start()
    {
        t = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        r.material.SetFloat("_Dissolve", 1f - (t / dissolveTime));
        t += Time.deltaTime;

        if (destroyOnFinish && t >= dissolveTime)
        {
            Destroy(gameObject);
        }
    }
}
