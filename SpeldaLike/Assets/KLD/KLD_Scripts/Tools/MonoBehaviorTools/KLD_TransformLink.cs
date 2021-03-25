using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KLD_TransformLink : SerializedMonoBehaviour
{
    [SerializeField]
    Transform linkTo;

    [SerializeField]
    bool lockRotation = false;
    [SerializeField, ShowIf("lockRotation")]
    bool lockX = false, lockY = false, lockZ = false;

    // Update is called once per frame
    void Update()
    {
        transform.position = linkTo.position;
        if (lockRotation)
        {
            //transform.rotation = linkTo.rotation;
            Vector3 eulerAngles = new Vector3(
                lockX ? linkTo.rotation.eulerAngles.x : transform.rotation.eulerAngles.x,
                lockY ? linkTo.rotation.eulerAngles.y : transform.rotation.eulerAngles.y,
                lockZ ? linkTo.rotation.eulerAngles.z : transform.rotation.eulerAngles.z
            );
            transform.rotation = Quaternion.Euler(eulerAngles);
        }
    }
}
