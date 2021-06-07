using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemySettings", menuName = "KLD/EnemySettings", order = 0)]
public class KLD_EnemySettings : ScriptableObject
{
    public int maxHealth = 5;
    public float invulnerabilityTime = 0.1f;

    public float detectionRange = 10f;

    public float healPointLootChance = 0.4f;
    public GameObject healPointObj = null;

    [Header("Wandering")]
    public Vector2 minMaxWanderRange = new Vector2(3f, 5f);
    public Vector2 minMaxWanderTimer = new Vector2(2f, 3f);
    public LayerMask allowedLayers;

    [Header("Debug")]
    public float maxErrorDistance = 1f;
}
