using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_HealPoint : MonoBehaviour
{

    KLD_PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<KLD_PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerHealth.HealPlayer();
            Destroy(gameObject);
        }
    }
}
