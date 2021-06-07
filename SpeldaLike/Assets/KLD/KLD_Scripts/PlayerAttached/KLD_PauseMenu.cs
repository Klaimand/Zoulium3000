using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_PauseMenu : MonoBehaviour
{

    bool paused = false;

    GameObject obj = null;

    // Start is called before the first frame update
    void Start()
    {
        obj = transform.GetChild(0).gameObject;
    }

    public void PressedKey()
    {
        if (!paused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    public void Pause()
    {
        if (!paused)
        {
            obj.SetActive(true);
            paused = true;
            Time.timeScale = 0f;
        }
    }

    public void Unpause()
    {
        if (paused)
        {
            obj.SetActive(false);
            paused = false;
            Time.timeScale = 1f;
        }
    }
}
