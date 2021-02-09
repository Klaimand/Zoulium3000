using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class KLD_CameraManagerInst : MonoBehaviour
{
    public void ChangeCameraInstance(CinemachineVirtualCamera _newCam)
    {
        CameraManager.Instance.ChangeCamera(_newCam);
    }
}
