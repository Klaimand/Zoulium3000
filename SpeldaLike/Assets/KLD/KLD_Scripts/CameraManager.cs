using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] int activeCamPriority = 10;
    [SerializeField] int inactiveCamPriority = 0;
    [SerializeField] CinemachineVirtualCamera startCam;
    CinemachineVirtualCamera curCam;

    public static CameraManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //curCam = startCam;
        //ChangeCamera(startCam);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeCamera(CinemachineVirtualCamera _newCam)
    {
        curCam.m_Priority = inactiveCamPriority;
        _newCam.m_Priority = activeCamPriority;
        curCam = _newCam;
    }

    public void SetFirstCam(CinemachineVirtualCamera _newCam)
    {
        _newCam.Priority = activeCamPriority;
        curCam = _newCam;
    }
}
