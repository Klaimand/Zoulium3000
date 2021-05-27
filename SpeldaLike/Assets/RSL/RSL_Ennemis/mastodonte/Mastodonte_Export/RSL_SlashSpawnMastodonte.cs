using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSL_SlashSpawnMastodonte : MonoBehaviour
{

    [SerializeField] GameObject slash1;
    [SerializeField] GameObject slash2;
    [SerializeField] GameObject slash3;
    [SerializeField] GameObject slam;
    void Salutatouscestsqueezie()
    {
        Instantiate(slash1, transform.position, transform.rotation);
    }
    void Salutatouscestsqueezie2()
    {
        Instantiate(slash2, transform.position, transform.rotation);
    }
    void Salutatouscestsqueezie3()
    {
        Instantiate(slash3, transform.position, transform.rotation);
    }
    void SlamAttaque()
    {
        Instantiate(slam, transform.position, transform.rotation);
    }
}
