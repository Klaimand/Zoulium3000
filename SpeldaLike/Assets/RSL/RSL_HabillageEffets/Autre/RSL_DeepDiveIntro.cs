using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class RSL_DeepDiveIntro : MonoBehaviour
{
    public UnityEvent OnPlayerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnPlayerEnter.Invoke();
            Debug.Log("salut");
        }
    }
}