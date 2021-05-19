using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_ColliderAttack : MonoBehaviour
{

    [SerializeField] int damage = 1;

    [SerializeField] bool playerAttack = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SendMessage(playerAttack ? "DamageEnemy" : "DamagePlayer", damage, SendMessageOptions.DontRequireReceiver);
        print(other.gameObject.name);
    }
}
