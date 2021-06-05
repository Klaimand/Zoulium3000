using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_GameManagerInst : MonoBehaviour
{
    public void LoadScene(string _scene)
    {
        GameManager.Instance.LoadScene(_scene);
    }

    public void RespawnPlayer()
    {
        GameManager.Instance.RespawnPlayer();
    }
}
