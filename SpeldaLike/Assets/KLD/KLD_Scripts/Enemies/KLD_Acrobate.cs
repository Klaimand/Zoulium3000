using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_Acrobate : KLD_Enemy
{
    [SerializeField] GameObject explodedPrefab;
    [SerializeField] Animator animator;
    [Header("Speeds"), SerializeField] Vector2 spAcWandering = new Vector2(4f, 6f);
    [SerializeField] Vector2 spAcAlerted = new Vector2(6f, 10f);
    [SerializeField] Vector2 spAcBlocking = new Vector2(6f, 10f);

    [SerializeField] float blockingDistanceToPlayer = 2f;
    [SerializeField] float blockingDistance = 4f;
    [SerializeField] float blockTimeToAttack = 2f;
    float curBlockTimeWithoutAttack;

    [SerializeField] float attackDuration = 0.5f;
    float curAttackTime;

    [SerializeField] float stunDuration = 3f;
    float curStunTime;

    [SerializeField] GameObject shieldObj;

    [SerializeField, Header("Rotation")]
    float maxAngularSpeed = 180f;
    [SerializeField] float timeToReachTargetAngle = 0.4f;

    enum AcrobateState { WANDERING, ALERTED, BLOCKING, ATTACKING, STUNNED };
    [SerializeField] AcrobateState curState = AcrobateState.WANDERING;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        isInvulnerable = true;
        SetAgentSpac(spAcWandering);
        //shieldObj.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        UpdateAcrobateState();
        DoAcrobateBehavior();
    }

    void UpdateAcrobateState()
    {
        if (curState == AcrobateState.WANDERING)
        {
            if (IsPlayerInZone() && PlayerDistance() <= settings.detectionRange)
            {
                curState = AcrobateState.ALERTED;
                SetAgentSpac(spAcAlerted);
            }
        }
        else if (curState == AcrobateState.ALERTED)
        {
            if (!IsPlayerInZone())
            {
                curState = AcrobateState.WANDERING;
                SetAgentSpac(spAcWandering);
            }
            else if (PlayerDistance() < blockingDistance)
            {
                curState = AcrobateState.BLOCKING;
                SetAgentSpac(spAcBlocking);
            }
        }
        else if (curState == AcrobateState.BLOCKING)
        {
            if (!IsPlayerInZone())
            {
                curState = AcrobateState.WANDERING;
                SetAgentSpac(spAcWandering);
            }
            else if (PlayerDistance() > blockingDistance)
            {
                curState = AcrobateState.ALERTED;
                SetAgentSpac(spAcAlerted);
            }
            else if (curBlockTimeWithoutAttack >= blockTimeToAttack)
            {
                KLD_AudioManager.Instance.PlaySound("AcrobateAttack");
                animator.SetTrigger("attack");
                curState = AcrobateState.ATTACKING;
                //isInvulnerable = false;
                //shieldObj.SetActive(false);
            }
        }
        else if (curState == AcrobateState.ATTACKING)
        {
            if (curAttackTime >= attackDuration)
            {
                curState = AcrobateState.BLOCKING;
                //isInvulnerable = true;
                //shieldObj.SetActive(true);
                curBlockTimeWithoutAttack = 0f;
            }
        }
        else if (curState == AcrobateState.STUNNED)
        {
            if (curStunTime >= stunDuration)
            {
                curState = AcrobateState.WANDERING;
                r.materials[1].SetFloat("Stun_Slider_", 0f);
                isInvulnerable = true;
                shieldObj.SetActive(true);
            }
        }
    }

    void DoAcrobateBehavior()
    {
        switch (curState)
        {
            case AcrobateState.WANDERING:
                Wander();
                break;

            case AcrobateState.ALERTED:
                StickToPlayer();
                break;

            case AcrobateState.BLOCKING:
                curBlockTimeWithoutAttack += Time.deltaTime;
                StickToPlayer();
                break;

            case AcrobateState.ATTACKING:
                curAttackTime += Time.deltaTime;
                DoRotation();
                break;

            case AcrobateState.STUNNED:
                curStunTime += Time.deltaTime;
                break;

            default:
                print("........::...");
                break;
        }
    }

    protected override void OnDamageTake()
    {
        KLD_AudioManager.Instance.PlaySound("AcrobateDamage");
    }

    protected override void OnSuperJumpShockwave()
    {

    }

    public override void DamageEnemySuperJump(int _damage)
    {
        //base.DamageEnemySuperJump(_damage);
        curState = AcrobateState.STUNNED;
        curStunTime = 0f;
        isInvulnerable = false;
        shieldObj.SetActive(false);
        r.materials[1].SetFloat("Stun_Slider_", 1f);
    }

    void StickToPlayer()
    {
        if (PlayerDistance() > blockingDistanceToPlayer)
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

    void DoRotation()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float angle = Vector3.SignedAngle(transform.forward, toPlayer, Vector3.up);

        angle = Mathf.Clamp(angle, -maxAngularSpeed, maxAngularSpeed);
        transform.Rotate(Vector3.up * angle * Time.deltaTime * (1f / timeToReachTargetAngle));
    }

    protected override void Die()
    {
        KLD_AudioManager.Instance.PlaySound("AcrobateDamage");
        Instantiate(explodedPrefab, transform.position, transform.rotation);
        base.Die();
    }
}
