using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class KLD_Boss : MonoBehaviour
{
    Transform player;

    //____________________________________________ATTACKS
    [Header("Energy Wave"), SerializeField]
    Animator groundAnimator;
    [SerializeField] float damageDelay = 4f;
    [SerializeField] GameObject damageZone;

    [SerializeField, Header("Energy Ball")]
    GameObject energyBallPrefab;
    [SerializeField] int numberOfBalls = 10;
    [SerializeField] float ballMaxDelay = 8f;
    [SerializeField] Transform[] spawnPoints;

    [SerializeField, Header("Laser")]
    GameObject laserBoss;
    float laserInstOffset = 1.5f;

    [SerializeField, Header("Launch")]
    float groundAttackThreshold = 0.5f;
    [SerializeField] float ballAttackThreshold = 0.8f;
    [SerializeField] Vector2 minMaxAttackLaunchTime = new Vector2(5f, 10f);

    //enum AttackType { GROUND_ATTACK, BALL_ATTACK, LASER_ATTACK };
    int[] lastAttacks = new int[2];
    //__________________________________________

    [SerializeField, Header("Life")]
    int health = 5;
    int curHealth;
    //[SerializeField] GameObject shieldObj;
    bool isVulnerable = false;
    bool dead;
    bool canAttack = true;
    [SerializeField] UnityEvent onBossNoShield;
    [SerializeField] UnityEvent onBossDefeat;

    // Start is called before the first frame update
    void Start()
    {
        canAttack = true;
        curHealth = health;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //StartCoroutine(WaitAndStartBoss());
    }

    // Update is called once per frame
    //void Update()
    //{

    //}


    IEnumerator WaitAndStartBoss()
    {
        yield return new WaitForSeconds(5f);
        StartBoss();
    }

    public void StartBoss()
    {
        canAttack = true;
        StartCoroutine(BossLoop());
    }

    IEnumerator BossLoop()
    {
        while (canAttack)
        {
            bool isValid = false;
            int attackIndex = 0;
            do
            {
                float rd = Random.value;

                if (rd < groundAttackThreshold)
                {
                    attackIndex = 0;
                }
                else if (rd < ballAttackThreshold)
                {
                    attackIndex = 1;
                }
                else
                {
                    attackIndex = 2;
                }

                if (lastAttacks[0] != lastAttacks[1])
                {
                    isValid = true;
                }
                else if (attackIndex != lastAttacks[0])
                {
                    isValid = true;
                }

            } while (!isValid);

            if (attackIndex == 0)
            {
                EnergyWaveAttack();
            }
            else if (attackIndex == 1)
            {
                EnergyBallAttack();
            }
            else if (attackIndex == 2)
            {
                LaserAttack();
            }

            lastAttacks[1] = lastAttacks[0];
            lastAttacks[0] = attackIndex;

            float rdt = Random.Range(minMaxAttackLaunchTime.x, minMaxAttackLaunchTime.y);
            yield return new WaitForSeconds(4f + rdt);
        }
    }

    #region Attacks

    void EnergyWaveAttack()
    {
        groundAnimator.SetTrigger("attack");
        StartCoroutine(IEnergyAttack());
    }

    IEnumerator IEnergyAttack()
    {
        yield return new WaitForSeconds(damageDelay);
        Instantiate(damageZone, transform.position, transform.rotation);
    }

    void EnergyBallAttack()
    {
        Vector3[] positions = GetSpawnPoints();
        foreach (var pos in positions)
        {
            StartCoroutine(WaitAndInstanciate(pos, Random.value * ballMaxDelay));
        }
    }

    IEnumerator WaitAndInstanciate(Vector3 _position, float _time)
    {
        yield return new WaitForSeconds(_time);
        Instantiate(energyBallPrefab, _position, Quaternion.identity);
    }

    Vector3[] GetSpawnPoints()
    {
        List<Vector3> availiblePositions = new List<Vector3>();

        foreach (var spawnPoint in spawnPoints)
        {
            availiblePositions.Add(spawnPoint.position);
        }

        List<Vector3> finalPositions = new List<Vector3>();

        int nbOfBalls = spawnPoints.Length - 1 < numberOfBalls ? spawnPoints.Length - 1 : numberOfBalls;

        for (int i = 0; i < nbOfBalls; i++)
        {
            int rdIndex = Random.Range(0, availiblePositions.Count);
            finalPositions.Add(availiblePositions[rdIndex]);
            availiblePositions.RemoveAt(rdIndex);
        }

        return finalPositions.ToArray();
    }

    void LaserAttack()
    {
        Instantiate(laserBoss, transform.position + Vector3.up * laserInstOffset, Quaternion.identity);
    }

    #endregion


    public void LaunchAttack(int _attackIndex)
    {
        if (_attackIndex == 0)
        {
            EnergyWaveAttack();
        }
        else if (_attackIndex == 1)
        {
            EnergyBallAttack();
        }
        else if (_attackIndex == 2)
        {
            LaserAttack();
        }
    }

    public void StopBoss()
    {
        canAttack = false;
        StopCoroutine(BossLoop());
    }

    public void TakeDamage()
    {
        curHealth--;

        if (curHealth <= 0)
        {
            onBossNoShield.Invoke();
            isVulnerable = true;
            //shieldObj.SetActive(false);
        }
    }

    public void Die()
    {
        if (isVulnerable && !dead)
        {
            onBossDefeat.Invoke();
        }
    }

    public void LoadMainMenu()
    {
        AudioSource _source = KLD_AudioManager.Instance.GetSound("musique2").GetSource();
        KLD_AudioManager.Instance.FadeOutInst(_source, 2f);
        KLD_AudioManager.Instance.PlaySound("musique1");
        KLD_AudioManager.Instance.SetReverb(false);
        GameManager.Instance.LoadMainMenu("MainMenu");
        print("laod main menu");
    }
}
