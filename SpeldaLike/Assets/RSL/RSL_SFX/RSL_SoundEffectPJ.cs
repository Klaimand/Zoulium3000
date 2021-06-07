using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class RSL_SoundEffectPJ : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClip;
    public AudioClip Slash1SFX;
    public AudioClip Slash2SFX;
    public AudioClip Slash3SFX;
    public AudioClip JumpSFX;
    public AudioClip Jump2SFX;
    public AudioClip MegaJumpChargeSFX;
    public AudioClip JumpDecolageSFX;
    public AudioClip MegaJumpFallSFX;
    public AudioClip GrappleSFX;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        int index = Random.Range(0, audioClip.Length);
        return audioClip[index];
    }

    public void Slash1()
    {
        audioSource.PlayOneShot(Slash1SFX);
    }
    public void Slash2()
    {
        audioSource.PlayOneShot(Slash2SFX);
    }
    public void Slash3()
    {
        audioSource.PlayOneShot(Slash3SFX);
    }
    public void Jump()
    {
        audioSource.PlayOneShot(JumpSFX);
    }
    public void Jump2()
    {
        audioSource.PlayOneShot(Jump2SFX);
    }

    public void MegaJumpCharge()
    {
        audioSource.PlayOneShot(MegaJumpChargeSFX);
    }
    public void MegaJumpFall()
    {
        audioSource.PlayOneShot(MegaJumpFallSFX);
    }
    public void JumpDecolage()
    {
        audioSource.PlayOneShot(JumpDecolageSFX);
    }

    public void Grapple()
    {
        audioSource.PlayOneShot(GrappleSFX);
    }
}
