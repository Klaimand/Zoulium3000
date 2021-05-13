using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class KLD_DEBUG_AnimLauncher : SerializedMonoBehaviour
{

    [SerializeField] Animator animator;

    [SerializeField] bool loopAnim = false;
    [SerializeField, Range(1, 3), ShowIf("loopAnim")] int animToLoop = 0;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (loopAnim)
            animator?.SetInteger("playerState", animToLoop);


    }

    [Button]
    public void LaunchCombo()
    {
        animator?.SetTrigger("Combo");
        animator?.SetInteger("playerState", 7);
    }


    [Button]
    public void LaunchAnim(int _animToLaunch)
    {
        animator?.SetInteger("playerState", _animToLaunch);
        StartCoroutine(WaitAndIdle());
    }

    IEnumerator WaitAndIdle()
    {
        yield return new WaitForSeconds(1.2f);
        animator?.SetInteger("playerState", 0);
    }
}
