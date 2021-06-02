using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_MaterialPropertyChanger : MonoBehaviour
{
    [SerializeField] Renderer r;
    [SerializeField] int materialIndex = 0;
    [SerializeField] string reference = "_reference";
    [SerializeField] Vector2 fromTo = new Vector2(0f, 1f);

    public void ChangeProperty(float _fadeTime)
    {
        StartCoroutine(IChangeProperty(_fadeTime));
    }

    IEnumerator IChangeProperty(float _fadeTime)
    {
        float t = 0;

        while (t < _fadeTime)
        {
            float n = Mathf.Lerp(fromTo.x, fromTo.y, t / _fadeTime);
            r.materials[materialIndex].SetFloat(reference, n);
            t += Time.deltaTime;
            yield return null;
        }
        r.materials[materialIndex].SetFloat(reference, fromTo.y);
    }
}
