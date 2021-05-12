using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_Anchor : MonoBehaviour
{

    public enum AnchorState
    {
        FREE,
        SELECTED,
        GRABBED
    }

    public AnchorState curState = AnchorState.FREE;

    Color[] colors = { Color.blue, Color.green, Color.red };

    MeshRenderer r;

    void Awake()
    {
        r = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        r.material.color = colors[(int)curState];
    }
}
