using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class KLD_Debug_InputsBinder : SerializedMonoBehaviour
{
    [FoldoutGroup("Num1"), SerializeField] UnityEvent num1;
    [FoldoutGroup("Num2"), SerializeField] UnityEvent num2;
    [FoldoutGroup("Num3"), SerializeField] UnityEvent num3;
    [FoldoutGroup("Num4"), SerializeField] UnityEvent num4;
    [FoldoutGroup("Num5"), SerializeField] UnityEvent num5;
    [FoldoutGroup("Num6"), SerializeField] UnityEvent num6;

    private void Start()
    {
        Debug.LogWarning("KLD_Debug_InputsBinder is in use in the scene on the gameobject : " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            num1.Invoke();
        }
        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            num2.Invoke();
        }
        if (Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            num3.Invoke();
        }
        if (Keyboard.current.numpad4Key.wasPressedThisFrame)
        {
            num4.Invoke();
        }
        if (Keyboard.current.numpad5Key.wasPressedThisFrame)
        {
            num5.Invoke();
        }
        if (Keyboard.current.numpad6Key.wasPressedThisFrame)
        {
            num6.Invoke();
        }
    }
}
