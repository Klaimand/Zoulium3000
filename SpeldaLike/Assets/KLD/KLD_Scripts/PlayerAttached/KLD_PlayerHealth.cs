using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class KLD_PlayerHealth : SerializedMonoBehaviour
{

    KLD_PlayerController controller;
    Slider lifeBar;

    [SerializeField, Range(1, 10)] int maxHealth = 5;
    [SerializeField, ReadOnly] int curHealth;

    bool isInvulnerable = false;
    [SerializeField] float invulnerabilityTime = 0.2f;

    bool isDead = false;

    private void Awake()
    {
        controller = GetComponent<KLD_PlayerController>();


        curHealth = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        lifeBar = GameObject.Find("HUD_Canvas").transform.GetChild(0).GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void DamagePlayer(int _damage)
    {
        if (!isInvulnerable && !isDead)
        {
            for (int i = 0; i < _damage; i++)
            {
                curHealth--;

                if (curHealth <= 0)
                {
                    Die();
                    return;
                }
            }

            isInvulnerable = true;
            StartCoroutine(WaitAndDisableInvulnerability());
            UpdateUI();
            //update UI
        }
    }

    IEnumerator WaitAndDisableInvulnerability()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    void Die()
    {
        //controller.die
        isDead = true;
        UpdateUI();
    }

    void UpdateUI()
    {
        lifeBar.value = (float)curHealth / maxHealth;
    }

    public Vector2Int GetHealth()
    {
        return new Vector2Int(maxHealth, curHealth);
    }

    public void SetHealth(int _maxHealth, int _curHealth)
    {
        maxHealth = _maxHealth;
        curHealth = _curHealth;
        UpdateUI();
    }
}
