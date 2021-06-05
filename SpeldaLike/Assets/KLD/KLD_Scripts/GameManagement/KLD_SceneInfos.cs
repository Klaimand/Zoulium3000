using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName = "newSceneInfo", menuName = "KLD/Obsolete/KLD_SceneInfo", order = 50)]
public class KLD_SceneInfos : ScriptableObject
{
    public string sceneName;
    public CinemachineBlenderSettings customBlend;
}
