using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_Ouvrier : KLD_Enemy
{

    enum OuvrierState { UNALERTED, ALERTED, ATTACKING, STUNNED };

    [SerializeField] OuvrierState curState = OuvrierState.UNALERTED;

    [SerializeField, Header("Follow")]
    float followMinDist = 2f;

    [SerializeField, Header("Speeds")]
    float wanderingSpeed = 3.5f;
    [SerializeField] float wanderingAcceleration = 6f;
    [SerializeField] float alertedSpeed = 6f;
    [SerializeField] float alertedAcceleration = 10f;
    [SerializeField] float attackSpeed = 20f;
    [SerializeField] float attackAcceleration = 20f;


    [SerializeField, Header("Attack")] float attackDistance = 2f;
    [SerializeField] float attackCooldown = 2.5f;
    float timeSinceLastAttack = 99f;
    [SerializeField] float attackDashDistance = 1f;
    [SerializeField] float attackStopTime = 0.5f;
    [SerializeField] float attackDuration = 0.5f;

    [SerializeField, Header("Stun")] float stunTime = 1f;
    float curStunTime;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        UpdateOuvrierState();
        DoOuvrierBehavior();

        if (curState != OuvrierState.STUNNED)
        {
            timeSinceLastAttack += Time.deltaTime;
        }
    }

    void UpdateOuvrierState()
    {
        if (curState == OuvrierState.UNALERTED)
        {
            if (PlayerDistance() <= settings.detectionRange)
            {
                curState = OuvrierState.ALERTED;
                agent.speed = alertedSpeed;
                agent.acceleration = alertedAcceleration;
            }
        }
        else if (curState == OuvrierState.ALERTED)
        {
            if (PlayerDistance() <= attackDistance && timeSinceLastAttack >= attackCooldown)
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
            if (curStunTime >= stunTime)
            {
                curState = OuvrierState.UNALERTED;
                agent.speed = wanderingSpeed;
                agent.acceleration = wanderingAcceleration;
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
                curStunTime += Time.deltaTime;
                break;

            default:
                break;
        }
    }

    protected override void OnSuperJumpShockwave()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnDamageTake()
    {
        if (curState == OuvrierState.ATTACKING)
        {
            StopCoroutine(Attack());
        }

        curState = OuvrierState.STUNNED;
        curStunTime = 0f;
    }


    IEnumerator Attack()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(attackStopTime);

        if (curState == OuvrierState.ATTACKING)
        {

            Vector3 toDash = (player.position - transform.position).normalized * attackDashDistance;
            agent.isStopped = false;
            agent.speed = attackSpeed;
            agent.acceleration = attackAcceleration;
            agent.SetDestination(transform.position + toDash);
            timeSinceLastAttack = 0f;

            yield return new WaitForSeconds(attackDuration);

            curState = OuvrierState.UNALERTED;
            UpdateOuvrierState();

        }
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
        }
    }
}
