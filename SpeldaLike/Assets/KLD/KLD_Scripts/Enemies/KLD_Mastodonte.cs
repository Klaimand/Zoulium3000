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
    float attackDuration = 3.33f;
    float curAttackDuration;

    [SerializeField, Header("Rotation")]
    float maxAngularSpeed = 180f;
    [SerializeField] float timeToReachTargetAngle = 0.4f;

    [Header("Weakness"), SerializeField]
    GameObject[] anchors;
    [SerializeField] MastodontePart[] parts;
    [SerializeField] float partDestroyTime = 5f;


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
        throw new System.NotImplementedException();
    }

    protected override void OnSuperJumpShockwave()
    {
        throw new System.NotImplementedException();
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
                agent.isStopped = false;
                curState = MastodonteState.ALERTED;
                SetAgentSpac(spAcAlerted);
                timeSinceLastAttack = 0f;
            }
        }
        else if (curState == MastodonteState.STUNNED)
        {

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
            Destroy(anchor);
        }

        foreach (var part in parts)
        {
            part.rb.isKinematic = false;
            part.obj.transform.SetParent(null, true);
            Destroy(part.obj, partDestroyTime);
        }
    }
}

[System.Serializable]
public class MastodontePart
{
    public GameObject obj;
    public Rigidbody rb;
}
