using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_BossGroundAttack : MonoBehaviour
{
    [SerializeField] int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.SendMessage("DamagePlayer", damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }
}
