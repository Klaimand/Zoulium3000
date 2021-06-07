using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_GameManagerInst : MonoBehaviour
{
    public void LoadSceneFromMainMenu(string _scene)
    {
        GameManager.Instance.LoadSceneFromMainMenu(_scene);
    }

    public void LoadScene(string _scene)
    {
        GameManager.Instance.LoadScene(_scene);
    }

    public void RespawnPlayer()
    {
        GameManager.Instance.RespawnPlayer();
    }

    public void LoadMainMenu(string _scene)
    {
        GameManager.Instance.LoadMainMenu(_scene);
    }
}
