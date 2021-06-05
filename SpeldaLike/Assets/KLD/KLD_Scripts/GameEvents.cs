using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance = null;

    public event Action<int> onGravityDisable;
    public event Action<int> onGravityEnable;

    public event Action onDialogStart;
    public event Action onDialogEnd;

    public event Action onSceneChange;

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

    public void DisableGravity(int _id)
    {
        onGravityDisable?.Invoke(_id);
    }

    public void EnableGravity(int _id)
    {
        onGravityEnable?.Invoke(_id);
    }


    public void StartDialog()
    {
        onDialogStart?.Invoke();
    }

    public void EndDialog()
    {
        onDialogEnd?.Invoke();
    }

    public void ChangeScene()
    {
        onSceneChange?.Invoke();
    }
}
