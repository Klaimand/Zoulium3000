using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_GravityEventCaller : MonoBehaviour
{
    public void DisableGravity(int _id)
    {
        GameEvents.Instance.DisableGravity(_id);
    }

    public void EnableGravity(int _id)
    {
        GameEvents.Instance.EnableGravity(_id);
    }
}
