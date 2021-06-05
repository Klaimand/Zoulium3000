using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class KLD_FirstCamSetter : MonoBehaviour
{

    [SerializeField] CinemachineVirtualCamera firstCam;
    // Start is called before the first frame update
    void Start()
    {
        CameraManager.Instance.SetFirstCam(firstCam);
    }
}
