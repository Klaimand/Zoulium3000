using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemySettings", menuName = "KLD/EnemySettings", order = 0)]
public class KLD_EnemySettings : ScriptableObject
{
    public int maxHealth = 5;
    public float invulnerabilityTime = 0.1f;

    public float detectionRange = 10f;


    [Header("Wandering")]
    public Vector2 minMaxWanderRange = new Vector2(3f, 5f);
    public Vector2 minMaxWanderTimer = new Vector2(2f, 3f);
    public LayerMask allowedLayers;
}
