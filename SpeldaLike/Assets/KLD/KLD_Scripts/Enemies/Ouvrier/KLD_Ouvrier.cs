using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_Ouvrier : KLD_Enemy
{

    enum OuvrierState { UNALERTED, ALERTED, ATTACKING, STUNNED };

    [SerializeField] OuvrierState curState = OuvrierState.UNALERTED;
    float runThreshold = 0.5f;
    [SerializeField] Animator animator;

    [SerializeField, Header("Follow")]
    float followMinDist = 2f;

    [SerializeField, Header("Speeds")]
    float wanderingSpeed = 3.5f;
    [SerializeField] float wanderingAcceleration = 6f;
    [SerializeField] float alertedSpeed = 6f;
    [SerializeField] float alertedAcceleration = 10f;
    //[SerializeField] float attackSpeed = 20f;
    //[SerializeField] float attackAcceleration = 20f;


    [SerializeField, Header("Attack")] float attackDistance = 2f;
    [SerializeField] float attackCooldown = 2.5f;
    float timeSinceLastAttack = 99f;
    //[SerializeField] float attackDashDistance = 1f;
    [SerializeField] float attackStopTime = 0.5f;
    [SerializeField] float attackDuration = 0.5f;
    [SerializeField] GameObject attackZoneObject;

    [SerializeField, Header("Stun")] float attackStunTime = 1f;
    [SerializeField] float superJumpStunTime = 3f;
    float curStunTime;

    [SerializeField, Header("Death")]
    GameObject explodedPrefab = null;

    [SerializeField, Header("Rotation")]
    float maxAngularSpeed = 180f;
    [SerializeField] float timeToReachTargetAngle = 0.4f;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //print(IsPlayerInZone());
        //print(agent.isStopped);

        UpdateOuvrierState();
        DoOuvrierBehavior();
        SendAnimState();

        if (curState != OuvrierState.STUNNED)
        {
            timeSinceLastAttack += Time.deltaTime;
        }
    }

    void UpdateOuvrierState()
    {
        if (curState == OuvrierState.UNALERTED)
        {
            if (IsPlayerInZone() && PlayerDistance() <= settings.detectionRange)
            {
                curState = OuvrierState.ALERTED;
                agent.speed = alertedSpeed;
                agent.acceleration = alertedAcceleration;
            }
        }
        else if (curState == OuvrierState.ALERTED)
        {
            if (!IsPlayerInZone())
            {
                curState = OuvrierState.UNALERTED;
            }
            else if (PlayerDistance() <= attackDistance && timeSinceLastAttack >= attackCooldown)
            {
                curState = OuvrierState.ATTACKING;
                StartCoroutine(Attack());
            }
        }
        else if (curState == OuvrierState.ATTACKING)
        {

        }
        else if (curState == OuvrierState.STUNNED)
        {
            if (curStunTime <= 0f)
            {
                curState = OuvrierState.UNALERTED;
                r.materials[1].SetFloat("Stun_Slider_", 0f);
                agent.speed = wanderingSpeed;
                agent.acceleration = wanderingAcceleration;
                timeSinceLastAttack = 0f;
                UpdateOuvrierState();
            }
        }
    }

    void DoOuvrierBehavior()
    {
        switch (curState)
        {
            case OuvrierState.UNALERTED:
                Wander();
                break;

            case OuvrierState.ALERTED:
                RushPlayer();
                break;

            case OuvrierState.ATTACKING:
                break;

            case OuvrierState.STUNNED:
                curStunTime -= Time.deltaTime;
                break;

            default:
                break;
        }
    }

    protected override void OnSuperJumpShockwave()
    {
        GetStunned(superJumpStunTime);
    }

    protected override void OnDamageTake()
    {
        GetStunned(attackStunTime);
        KLD_AudioManager.Instance.PlaySound("OuvrierDamage");
    }

    protected override void Die()
    {
        KLD_AudioManager.Instance.PlaySound("OuvrierDeath");
        Instantiate(explodedPrefab, transform.position, transform.rotation);
        base.Die();
    }

    void GetStunned(float _stunTime)
    {
        if (curState == OuvrierState.ATTACKING)
        {
            StopCoroutine(Attack());
        }

        curState = OuvrierState.STUNNED;
        curStunTime = _stunTime;
        r.materials[1].SetFloat("Stun_Slider_", 1f);
    }


    IEnumerator Attack()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(attackStopTime);

        if (curState == OuvrierState.ATTACKING)
        {

            //Vector3 toDash = (player.position - transform.position).normalized * attackDashDistance;
            //agent.isStopped = false;
            //agent.speed = attackSpeed;
            //agent.acceleration = attackAcceleration;
            //agent.SetDestination(transform.position + toDash);
            timeSinceLastAttack = 0f;
            animator?.SetTrigger("attack");
            KLD_AudioManager.Instance.PlaySound("OuvrierAttack");
            //Instantiate(attackZoneObject, transform.position, transform.rotation, transform);

            yield return new WaitForSeconds(attackDuration);

            animator?.SetBool("isAttacking", false);

            curState = OuvrierState.UNALERTED;
            UpdateOuvrierState();

        }
    }

    public void SpawnAttackVFX()
    {
        Instantiate(attackZoneObject, transform.position, transform.rotation, transform);
    }

    void RushPlayer()
    {
        if (PlayerDistance() > followMinDist)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            DoRotation();
        }
    }

    void SendAnimState()
    {
        animator?.SetInteger("enemyState", (int)curState);

        bool isr = agent.velocity.sqrMagnitude > runThreshold * runThreshold;
        animator?.SetBool("isRunning", isr);
    }

    void DoRotation()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float angle = Vector3.SignedAngle(transform.forward, toPlayer, Vector3.up);

        angle = Mathf.Clamp(angle, -maxAngularSpeed, maxAngularSpeed);
        transform.Rotate(Vector3.up * angle * Time.deltaTime * (1f / timeToReachTargetAngle));
    }
}
