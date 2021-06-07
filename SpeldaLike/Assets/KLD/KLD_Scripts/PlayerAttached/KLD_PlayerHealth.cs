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
    [SerializeField] float blinkTime = 0.05f;

    [SerializeField] GameObject healFeedback;

    [SerializeField] Renderer[] renderers;

    [SerializeField] GameObject ragdoll;

    [SerializeField] GameObject playerToDisable;

    bool isDead = false;

    private void Awake()
    {
        controller = GetComponent<KLD_PlayerController>();


        curHealth = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        //lifeBar = GameObject.Find("HUD_Canvas").transform.GetChild(0).GetComponent<Slider>();
        barsParent = GameObject.Find("HUD_Canvas").transform.GetChild(2).transform;
        //StartCoroutine(WaitAndUpdateUI());
    }

    IEnumerator WaitAndUpdateUI()
    {
        yield return new WaitForSeconds(0.3f);
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (barsParent.childCount != maxHealth)
        {
            print(barsParent.childCount + " SSS");
            UpdateUI();
        }
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
            SetRenderersFloat(1f);
            StartCoroutine(WaitAndDisableInvulnerability());
            StartCoroutine(WaitAndUnblink());
            UpdateUI();
            //update UI
            KLD_AudioManager.Instance.PlaySound("TakeDamage");
        }
    }

    IEnumerator WaitAndDisableInvulnerability()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    IEnumerator WaitAndUnblink()
    {
        yield return new WaitForSeconds(blinkTime);
        SetRenderersFloat(0f);
    }

    void Die()
    {
        KLD_AudioManager.Instance.PlaySound("Die");
        controller.Die();
        isDead = true;
        UpdateUI();
        playerToDisable.SetActive(false);
        Instantiate(ragdoll, transform.position, transform.rotation);
        StartCoroutine(WaitAndRespawn());
    }

    IEnumerator WaitAndRespawn()
    {
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.RespawnPlayer();
    }

    void UpdateUI()
    {
        //lifeBar.value = (float)curHealth / maxHealth;
        DoLifeBarUI();
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

    public void AddMaxHP()
    {
        maxHealth++;
        UpdateUI();
        KLD_AudioManager.Instance.PlaySound("BonusHealth");
    }

    public void HealPlayer()
    {
        if (curHealth < maxHealth)
        {
            curHealth++;
            UpdateUI();
            Instantiate(healFeedback, transform.position, Quaternion.Euler(-90f, 0f, 0f), transform);
            KLD_AudioManager.Instance.PlaySound("Heal");
        }
    }

    void SetRenderersFloat(float _value)
    {
        foreach (var r in renderers)
        {
            r.materials[1].SetFloat("HitSlider_", _value);
        }
    }

    [SerializeField] Sprite fullBar;
    [SerializeField] Sprite emptyBar;
    [SerializeField] GameObject barPrefab;
    Transform barsParent;

    Image[] bars;

    int barLenght = 168;
    int spaceLenght = 20;

    void DoLifeBarUI()
    {
        if (barsParent.childCount != maxHealth)
        {
            print("pp");
            List<GameObject> toDestroy = new List<GameObject>();

            for (int i = 0; i < barsParent.childCount; i++)
            {
                toDestroy.Add(barsParent.GetChild(i).gameObject);
                //Destroy(barsParent.GetChild(0).gameObject);
            }

            foreach (var go in toDestroy)
            {
                Destroy(go);
            }


            bars = new Image[maxHealth];
            //int offset = maxHealth % 2 == 0 ? barLenght / 2 : 0;
            int offset = barLenght / 2;
            for (int i = 0; i < maxHealth; i++)
            {
                GameObject obj = Instantiate(barPrefab, Vector3.zero, Quaternion.identity, barsParent);
                bars[i] = obj.GetComponent<Image>();
                obj.GetComponent<RectTransform>().anchoredPosition = Vector3.right *
                (
                    (i * (barLenght + spaceLenght) + offset) - ((maxHealth * (barLenght + spaceLenght)) / 2)
                );
            }
        }

        if (bars == null)
        {
            return;
        }

        for (int i = 0; i < maxHealth; i++)
        {
            bars[i].sprite = i < curHealth ? fullBar : emptyBar;
        }
    }
}
