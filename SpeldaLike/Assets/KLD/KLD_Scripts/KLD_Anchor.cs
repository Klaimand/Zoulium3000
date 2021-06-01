using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KLD_Anchor : MonoBehaviour
{

    public enum AnchorState
    {
        FREE,
        SELECTED,
        GRABBED
    }

    public AnchorState curState = AnchorState.FREE;

    public bool isOnEnemy = false;

    public UnityEvent onGrab;

    //Color[] colors = { Color.blue, Color.green, Color.red };

    MeshRenderer r;

    void Awake()
    {
        r = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
    }

    void Update()
    {
        //r.material.color = colors[(int)curState];
        float v = curState == AnchorState.FREE ? 0f : 2f;
        r.material.SetFloat("Slider_", v);
    }
}
