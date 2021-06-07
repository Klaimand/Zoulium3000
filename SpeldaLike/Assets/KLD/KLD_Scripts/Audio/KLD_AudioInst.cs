using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_AudioInst : MonoBehaviour
{
    [SerializeField] bool playSoundOnStart = false;
    [SerializeField] string soundName = "sound";

    [SerializeField, Header("FadeOut")]
    float time = 2f;

    void Start()
    {
        if (playSoundOnStart)
        {
            PlaySoundInst(soundName);
        }
    }

    public void PlaySoundInst(string _soundName)
    {
        KLD_AudioManager.Instance.PlaySound(_soundName);
    }

    public void FadeOut(string _soundName)
    {
        KLD_AudioManager.Instance.FadeOutInst(KLD_AudioManager.Instance.GetSound(_soundName).GetSource(), time);
    }

    public void SetReverb(bool value)
    {
        KLD_AudioManager.Instance.SetReverb(value);
    }
}
