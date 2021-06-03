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
    //__________________________________________

    [SerializeField, Header("Life")]
    int health = 5;
    int curHealth;
    //[SerializeField] GameObject shieldObj;
    bool isVulnerable = false;
    bool dead;
    [SerializeField] UnityEvent onBossNoShield;
    [SerializeField] UnityEvent onBossDefeat;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = health;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(w());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator w()
    {
        yield return new WaitForSeconds(5f);
        LaserAttack();
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


}
