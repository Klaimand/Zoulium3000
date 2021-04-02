using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RSL_fadeInShaderHolograme: MonoBehaviour

{

    public Material hologrameMat;

    public float speed;
    public float montantDeStep;
    private float targetStepValue = 0f;
    private float currentStepValue = 0f;


    void Update()
    {
        currentStepValue = Mathf.Lerp(currentStepValue, targetStepValue, speed * Time.deltaTime); //Ce qui fait slider le slider

        hologrameMat.SetFloat("_Dissolve", currentStepValue);

        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            //Ca le fait apparaitre
            targetStepValue += montantDeStep;
        }
        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            //Ca le fait disparaitre
            targetStepValue = 0f;
        }
    }
}