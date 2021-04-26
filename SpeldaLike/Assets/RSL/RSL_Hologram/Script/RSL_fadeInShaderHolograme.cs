using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            //Ca le fait apparaitre
            targetStepValue += montantDeStep;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            //Ca le fait disparaitre
            targetStepValue = 0f;
        }
    }
}