using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_Mastodonte : KLD_Enemy
{
    [Header("Speeds"), SerializeField]
    Vector2 spAcWandering = new Vector2(2f, 4f);
    [SerializeField] Vector2 spAcAlerted = new Vector2(2f, 4f);

    enum MastodonteState { WANDERING, ALERTED, ATTACKING, STUNNED };
    [Header("State"), SerializeField]
    MastodonteState curState = MastodonteState.WANDERING;

    [SerializeField, Header("Following")]
    float followMinDist = 4f;

    [Header("Attack"), SerializeField]
    float attackCooldown = 5f;
    float timeSinceLastAttack;
    [SerializeField] float attackDistance = 5f;
    [SerializeField] float attackDuration = 3.33f;
    float curAttackDuration;

    [SerializeField, Header("Rotation")]
    float maxAngularSpeed = 180f;
    [SerializeField] float timeToReachTargetAngle = 0.4f;

    [Header("Weakness"), SerializeField]
    int dmgOnPulling = 7;
    [SerializeField] GameObject[] anchors;
    [SerializeField] MastodontePart[] parts;
    [SerializeField] float partDestroyTime = 5f;

    [SerializeField, Header("Stun")]
    float stunDuration = 3f;
    float curStunDuration;

    [SerializeField, Header("Death")]
    GameObject deadBody;


    [Header("Animation"), SerializeField]
    float moveThreshold = 0.2f;
    [SerializeField] Animator animator;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SetAgentSpac(spAcWandering);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        UpdateMastodonteState();
        DoMastodonteBehavior();

        DoAttackCooldown();

        SendAnimatorStates();
    }

    protected override void OnDamageTake()
    {
        KLD_AudioManager.Instance.PlaySound("MastodonteDamage");
        //throw new System.NotImplementedException();
    }

    protected override void OnSuperJumpShockwave()
    {
        //throw new System.NotImplementedException();
    }

    protected override void Die()
    {
        KLD_AudioManager.Instance.PlaySound("MastodonteDamage");
        Instantiate(deadBody, transform.position, transform.rotation);
        base.Die();
    }

    void UpdateMastodonteState()
    {
        if (curState == MastodonteState.WANDERING)
        {
            if (IsPlayerInZone() && PlayerDistance() < settings.detectionRange)
            {
                curState = MastodonteState.ALERTED;
                SetAgentSpac(spAcAlerted);
            }
        }
        else if (curState == MastodonteState.ALERTED)
        {
            if (!IsPlayerInZone())
            {
                agent.isStopped = false;
                curState = MastodonteState.WANDERING;
                SetAgentSpac(spAcWandering);
            }
            else if (PlayerDistance() < attackDistance && timeSinceLastAttack >= attackCooldown)
            {
                agent.isStopped = true;
                curAttackDuration = 0f;
                curState = MastodonteState.ATTACKING;
                animator?.SetTrigger("attack");
            }
        }
        else if (curState == MastodonteState.ATTACKING)
        {
            if (curAttackDuration >= attackDuration)
            {
                //agent.isStopped = false;
                //curState = MastodonteState.ALERTED;
                //SetAgentSpac(spAcAlerted);
                //timeSinceLastAttack = 0f;

                curState = MastodonteState.STUNNED;
                curStunDuration = 0f;
                r.materials[1].SetFloat("Stun_Slider_", 1f);
            }
        }
        else if (curState == MastodonteState.STUNNED)
        {
            if (curStunDuration >= stunDuration)
            {
                agent.isStopped = false;
                curState = MastodonteState.ALERTED;
                SetAgentSpac(spAcAlerted);
                timeSinceLastAttack = 0f;
                r.materials[1].SetFloat("Stun_Slider_", 0f);
            }
        }
    }

    void DoMastodonteBehavior()
    {
        switch (curState)
        {

            case MastodonteState.WANDERING:
                Wander();
                break;

            case MastodonteState.ALERTED:
                StickToPlayer();
                break;

            case MastodonteState.ATTACKING:
                curAttackDuration += Time.deltaTime;
                break;

            case MastodonteState.STUNNED:
                curStunDuration += Time.deltaTime;
                break;

            default:
                print("ouie aie aie");
                break;
        }
    }

    void DoAttackCooldown()
    {
        if (curState != MastodonteState.STUNNED)
        {
            timeSinceLastAttack += Time.deltaTime;
        }
    }

    void StickToPlayer()
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

    void DoRotation()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float angle = Vector3.SignedAngle(transform.forward, toPlayer, Vector3.up);

        angle = Mathf.Clamp(angle, -maxAngularSpeed, maxAngularSpeed);
        transform.Rotate(Vector3.up * angle * Time.deltaTime * (1f / timeToReachTargetAngle));
    }

    void SendAnimatorStates()
    {
        bool isMoving = agent.velocity.sqrMagnitude > moveThreshold * moveThreshold;
        animator?.SetBool("isMoving", isMoving);

        animator?.SetInteger("curState", (int)curState);
    }

    public void GetGrabbed()
    {
        foreach (var anchor in anchors)
        {
            anchor.GetComponent<KLD_Anchor>().notSelectable = true;
            Destroy(anchor, 0.25f);
        }

        foreach (var part in parts)
        {
            part.rb.isKinematic = false;
            part.obj.transform.SetParent(null, true);
            Destroy(part.obj, partDestroyTime);
        }

        int dmg = curHealth > dmgOnPulling ? dmgOnPulling : curHealth - 1;

        isInvulnerable = false;
        DamageEnemy(dmg);
    }
}

[System.Serializable]
public class MastodontePart
{
    public GameObject obj;
    public Rigidbody rb;
}
