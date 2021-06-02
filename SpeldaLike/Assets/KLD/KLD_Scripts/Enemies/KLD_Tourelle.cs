using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_Tourelle : MonoBehaviour
{
    [SerializeField] MeshRenderer r, r2;

    [SerializeField] float damageBlinkTime = 0.1f;

    [SerializeField] KLD_EnemySettings settings;

    int curHealth;
    bool isInvulnerable = false;

    Transform player;

    //____________________________________________
    [SerializeField] float playerOffset = 1f;
    Vector3 targetPosition;

    float sqrPlayerDist;

    [SerializeField, Header("Shooting")]
    int shootPerShooting = 3;
    int curShot;
    [SerializeField] float shotInterval = 1f;
    float curShotInterval;
    [SerializeField] float shootingInterval = 3f;
    float curShootingInterval;

    [SerializeField, Header("Aiming")]
    Transform head;
    [SerializeField] Transform refTarget;
    [SerializeField] float maxAngularSpeed = 30f;

    enum TourelleState { UNALERTED, SHOOTING, WAITING };
    [SerializeField] TourelleState curState;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = settings.maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //base.Update();
        targetPosition = player.position + (Vector3.up * playerOffset);
        sqrPlayerDist = (targetPosition - transform.position).sqrMagnitude;

        UpdateTourelleState();
        DoTourelleBehavior();



    }

    void UpdateTourelleState()
    {
        if (curState == TourelleState.UNALERTED)
        {
            if (sqrPlayerDist < settings.detectionRange * settings.detectionRange)
            {
                if (curShootingInterval >= shootingInterval)
                {
                    curState = TourelleState.SHOOTING;
                    curShot = 0;
                }
                else
                {
                    curState = TourelleState.WAITING;
                }
            }
        }
        else if (curState == TourelleState.SHOOTING)
        {
            if (curShot >= shootPerShooting - 1)
            {
                curState = TourelleState.WAITING;
                curShootingInterval = 0f;
            }
        }
        else if (curState == TourelleState.WAITING)
        {
            if (sqrPlayerDist > settings.detectionRange * settings.detectionRange)
            {
                curState = TourelleState.UNALERTED;
            }
            else if (curShootingInterval >= shootingInterval)
            {
                curState = TourelleState.SHOOTING;
                curShot = 0;
            }
        }
    }

    void DoTourelleBehavior()
    {
        switch (curState)
        {
            case TourelleState.UNALERTED:
                curShootingInterval += Time.deltaTime;
                DoRotation(refTarget.position);
                break;

            case TourelleState.SHOOTING:
                if (curShotInterval >= shotInterval)
                {
                    //shoot
                    print("shot");
                    curShot++;
                }
                DoRotation(targetPosition);
                curShotInterval += Time.deltaTime;
                break;

            case TourelleState.WAITING:
                DoRotation(targetPosition);
                curShootingInterval += Time.deltaTime;
                break;

            default:
                break;

        }
    }

    void DoRotation(Vector3 _target)
    {
        //Vector3 vectorToTarget = _target.position - _looker.position;
        //float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        //Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        //_looker.rotation = Quaternion.RotateTowards(_looker.rotation, q, Time.deltaTime * _maxRotateSpeed);

        Vector3 toTarget = _target - head.position;

        Quaternion q = Quaternion.LookRotation(toTarget, Vector3.up);

        head.rotation = Quaternion.RotateTowards(head.rotation, q, maxAngularSpeed * Time.deltaTime);



    }

    #region Life & death

    public virtual void DamageEnemy(int _damage)
    {
        if (!isInvulnerable)
        {
            if (TakeDamage(_damage))
            {

            }
        }
    }

    bool TakeDamage(int _damage)
    {
        //print("took " + _damage + " dmg");

        curHealth = Mathf.Max(0, curHealth - _damage);
        if (CheckDeath())
        {
            Die();
            return false;
        }

        isInvulnerable = true;

        StartCoroutine(WaitAndDisableInvulnerability());

        r.materials[2].SetFloat("HitSlider_", 1f);
        r2.materials[2].SetFloat("HitSlider_", 1f);
        StartCoroutine(WaitAndDisableBlinkShader());

        return true;
    }

    bool CheckDeath()
    {
        return curHealth <= 0;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator WaitAndDisableInvulnerability()
    {
        yield return new WaitForSeconds(settings.invulnerabilityTime);
        isInvulnerable = false;
    }

    IEnumerator WaitAndDisableBlinkShader()
    {
        yield return new WaitForSeconds(damageBlinkTime);
        r.materials[2].SetFloat("HitSlider_", 0f);
        r2.materials[2].SetFloat("HitSlider_", 0f);
    }



    #endregion
}
