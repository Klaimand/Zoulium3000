using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_ColliderAttack : MonoBehaviour
{

    [SerializeField] int damage = 1;

    enum AttackType { PLAYER_NORMAL, PLAYER_SUPERJUMP, ENEMY_NORMAL };

    [SerializeField] AttackType attackType = AttackType.PLAYER_NORMAL;

    [SerializeField, Header("Player")]
    LayerMask collisionLayer;
    [SerializeField] bool instImpact = false;
    [SerializeField] GameObject[] playerHitImpact;
    [SerializeField] int impactIndex = 0;

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
        string message = "";

        switch (attackType)
        {
            case AttackType.PLAYER_NORMAL:
                message = "DamageEnemy";
                break;

            case AttackType.PLAYER_SUPERJUMP:
                message = "DamageEnemySuperJump";
                break;

            case AttackType.ENEMY_NORMAL:
                message = "DamagePlayer";
                break;

            default:
                break;
        }

        if (instImpact && (attackType == AttackType.PLAYER_NORMAL || attackType == AttackType.PLAYER_SUPERJUMP))
        {
            if (collisionLayer == (collisionLayer | (1 << other.gameObject.layer)))
            {
                Instantiate(playerHitImpact[impactIndex], transform.position, transform.rotation);
            }
        }



        other.gameObject.SendMessage(message, damage, SendMessageOptions.DontRequireReceiver);
    }
}
