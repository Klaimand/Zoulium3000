using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YTH_Door : MonoBehaviour
{
    [SerializeField]
    Animator door;

    public void OpenDoor(bool _open)
    {
        door?.SetBool("open", _open);
    }
}
