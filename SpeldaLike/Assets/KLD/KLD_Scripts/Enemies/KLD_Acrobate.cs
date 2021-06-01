using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_Acrobate : KLD_Enemy
{

    [Header("Speeds"), SerializeField] Vector2 spAcWandering = new Vector2(4f, 6f);
    [SerializeField] Vector2 spAcAlerted = new Vector2(6f, 10f);
    [SerializeField] Vector2 spAcBlocking = new Vector2(6f, 10f);

    [SerializeField] float blockingDistance = 4f;
    [SerializeField] float parryTime = 0.3f;
    float curParryTime;
    [SerializeField] float blockTimeToAttack = 2f;
    float curBlockTimeWithoutAttack;

    [SerializeField] float attackDuration = 0.5f;

    [SerializeField] float stunDuration = 3f;
    float curStunTime;

    enum AcrobateState { WANDERING, ALERTED, BLOCKING, PARRYING, ATTACKING, STUNNED };
    AcrobateState curState = AcrobateState.WANDERING;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
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
            }
        }
        else if (curState == AcrobateState.BLOCKING)
        {
            if (!IsPlayerInZone())
            {
                curState = AcrobateState.WANDERING;
            }
            else if (PlayerDistance() > blockingDistance)
            {
                curState = AcrobateState.ALERTED;
            }
            else if (curBlockTimeWithoutAttack >= blockTimeToAttack)
            {
                //attack
                curState = AcrobateState.ATTACKING;
            }
            //if take damage => parry / reset curBlockTime / reset curParryTime
        }
        else if (curState == AcrobateState.PARRYING)
        {
            if (curParryTime >= parryTime)
            {
                curState = AcrobateState.BLOCKING;
            }
        }
        else if (curState == AcrobateState.ATTACKING)
        {

        }
        else if (curState == AcrobateState.STUNNED)
        {
            if (curStunTime >= stunDuration)
            {
                curState = AcrobateState.WANDERING;

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
                break;

            case AcrobateState.BLOCKING:
                break;

            case AcrobateState.PARRYING:
                break;

            case AcrobateState.ATTACKING:
                break;

            case AcrobateState.STUNNED:
                break;

            default:
                print("........::...");
                break;
        }
    }

    protected override void OnDamageTake()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnSuperJumpShockwave()
    {
        throw new System.NotImplementedException();
    }

    void SetAgentSpac(Vector2 _spac)
    {
        agent.speed = _spac.x;
        agent.acceleration = _spac.y;
    }

    void StickToPlayer()
    {

    }
}
