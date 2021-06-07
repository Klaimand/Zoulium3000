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

    private void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        int index = Random.Range(0, audioClip.Length - 1);
        return audioClip[index];
    }

   private void Slash1()
    {
        audioSource.PlayOneShot(Slash1SFX);
    }
    private void Slash2()
    {
        audioSource.PlayOneShot(Slash2SFX);
    }
    private void Slash3()
    {
        audioSource.PlayOneShot(Slash3SFX);
    }
    private void Jump()
    {
        audioSource.PlayOneShot(JumpSFX);
    }
    private void Jump2()
    {
        audioSource.PlayOneShot(Jump2SFX);
    }

    private void MegaJumpCharge()
    {
        audioSource.PlayOneShot(MegaJumpChargeSFX);
    }
    private void MegaJumpFall()
    {
        audioSource.PlayOneShot(MegaJumpFallSFX);
    }
    private void JumpDecolage()
    {
        audioSource.PlayOneShot(JumpDecolageSFX);
    }

    private void Grapple()
    {
        audioSource.PlayOneShot(GrappleSFX);
    }
}
