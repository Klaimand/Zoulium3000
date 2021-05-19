using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class KLD_Enemy : MonoBehaviour
{
    protected NavMeshAgent agent;
    NavMeshPath path;
    protected Transform player;

    [SerializeField] protected KLD_EnemySettings settings;

    int curHealth;
    bool isInvulnerable = false;

    //Wandering
    float curWanderTimer;
    float wanderTime;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        path = new NavMeshPath();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        curHealth = settings.maxHealth;

        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected abstract void OnSuperJumpShockwave();

    protected abstract void OnDamageTake();

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected float PlayerDistance()
    {
        //return Vector2.Distance(transform.position, player.position + Vector3.up);
        agent.CalculatePath(player.position, path);

        float dist = 0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            dist += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return dist;
    }

    public void DamageEnemy(int _damage)
    {
        if (!isInvulnerable)
        {
            print("took " + _damage + " dmg");

            curHealth = Mathf.Max(0, curHealth - _damage);
            if (CheckDeath())
            {
                Die();
                return;
            }

            isInvulnerable = true;
            //damage shader

            OnDamageTake();

            StartCoroutine(WaitAndDisableInvulnerability());

        }
    }

    bool CheckDeath()
    {
        return curHealth <= 0;
    }

    IEnumerator WaitAndDisableInvulnerability()
    {
        yield return new WaitForSeconds(settings.invulnerabilityTime);
        isInvulnerable = false;
    }

    #region Wandering

    protected void Wander()
    {
        wanderTime += Time.deltaTime;

        if (wanderTime >= curWanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, Random.Range(settings.minMaxWanderRange.x, settings.minMaxWanderRange.y), settings.allowedLayers); //mask was -1
            agent.SetDestination(newPos);
            wanderTime = 0;

            curWanderTimer = Random.Range(settings.minMaxWanderTimer.x, settings.minMaxWanderTimer.y);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection = new Vector3(randDirection.x, randDirection.y, 0f);

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    #endregion
}
