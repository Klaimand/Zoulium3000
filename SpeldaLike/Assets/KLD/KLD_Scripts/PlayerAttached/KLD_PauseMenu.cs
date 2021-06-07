using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KLD_PauseMenu : MonoBehaviour
{

    bool paused = false;

    GameObject obj = null;

    [SerializeField] Selectable toSelect;

    // Start is called before the first frame update
    void Start()
    {
        obj = transform.GetChild(0).gameObject;
        obj.SetActive(false);
    }

    void OnEnable()
    {
        GameEvents.Instance.onSceneChange += Unpause;
    }

    void OnDisable()
    {
        GameEvents.Instance.onSceneChange -= Unpause;
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
            toSelect.Select();
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
