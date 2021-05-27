using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class KLD_PowerGenerator : SerializedMonoBehaviour
{
    [SerializeField, FoldoutGroup("Rendering Event")] UnityEvent OnPlayerHitRendering;

    [SerializeField] UnityEvent OnPlayerHit;

    bool activated = false;

    public void DamageEnemy()//int _damage)
    {
        if (!activated)
        {
            activated = true;
            OnPlayerHitRendering.Invoke();
            OnPlayerHit.Invoke();
            print("activated moteur");
        }
    }

}
