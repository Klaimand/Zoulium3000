using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApparitionCube : MonoBehaviour {

    public Material hologrameMat;
    //public GameObject vfx;//

    public float speed;
    public float montantDeStep;
    private float targetStepValue = 0f;
    private float currentStepValue = 0f;

    void Start()
    {
  
    }

    void Update()
    {
        currentStepValue = Mathf.Lerp(currentStepValue, targetStepValue, speed * Time.deltaTime);

        hologrameMat.SetFloat("_Dissolve", currentStepValue);

        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            targetStepValue += montantDeStep;
           // Instantiate(vfx, transform.position, Quaternion.identity);//
        }
        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            targetStepValue = 0f;
        }
    }
}