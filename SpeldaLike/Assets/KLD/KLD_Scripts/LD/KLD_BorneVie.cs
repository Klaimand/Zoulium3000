using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_BorneVie : MonoBehaviour
{

    bool gaveLife = false;

    float healTime = 2f;
    float t;

    KLD_PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<KLD_PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gaveLife && other.gameObject.CompareTag("Player"))
        {
            playerHealth.AddMaxHP();
            gaveLife = true;
            t = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (t > healTime && other.gameObject.CompareTag("Player"))
        {
            other.gameObject.SendMessage("HealPlayer");
            t = 0f;
        }
    }

}
