using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class KLD_PowerGenerator : SerializedMonoBehaviour
{
    [SerializeField, FoldoutGroup("Rendering Event")] UnityEvent OnPlayerHitRenderingOnce;
    [SerializeField, FoldoutGroup("Rendering Event")] UnityEvent OnPlayerHitRenderingEach;

    [SerializeField] UnityEvent OnPlayerHit;

    bool activated = false;

    public void DamageEnemy(int _damage)
    {
        if (!activated)
        {
            activated = true;
            OnPlayerHitRenderingOnce.Invoke();
            OnPlayerHit.Invoke();
        }
        OnPlayerHitRenderingEach.Invoke();
    }

    public void PlayStretch(Animator a)
    {
        a.SetTrigger("attacked");
    }

}
