using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class KLD_DialogManager : SerializedMonoBehaviour
{

    KLD_PlayerController controller;

    CanvasGroup group;
    [SerializeField, FoldoutGroup("UI References")] Text uiText;
    [SerializeField, FoldoutGroup("UI References")] Text uiCharaText;
    [SerializeField, FoldoutGroup("UI References")] Image uiCharaImage;

    [Header("Dialog Settings"), SerializeField] float minPanelTime = 0.1f;
    [SerializeField] float timePerChar = 0.05f;
    [SerializeField] float timePerSpecialChar = 0.15f;
    List<char> specialChars = new List<char> { '.', ',', ';', ':', '!', '?' };
    [SerializeField] bool fadeInAndOut = false;
    [SerializeField, ShowIf("fadeInAndOut")] float fadeTime = 0.1f;

    bool dialoging = false;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SetActive(false);
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<KLD_PlayerController>();
    }

    public void StartDialog(KLD_Dialog _dialog, KLD_DialogStarter _starter)
    {
        if (!dialoging && (controller.GetPlayerState() == 0 || controller.GetPlayerState() == 1))
        {
            dialoging = true;
            StartCoroutine(IStartDialog(_dialog, _starter));
        }
    }

    IEnumerator IStartDialog(KLD_Dialog _dialog, KLD_DialogStarter _starter)
    {
        GameEvents.Instance.StartDialog();
        _starter.onDialogStart.Invoke();
        uiCharaText.text = _dialog.characterName;
        uiCharaImage.sprite = _dialog.characterSprite;
        uiText.text = "";

        if (fadeInAndOut)
        {
            StartCoroutine(FadeGroup(group, fadeTime, true));
            yield return new WaitForSeconds(fadeTime);
        }

        group.alpha = 1f;


        foreach (var panel in _dialog.panels)
        {
            uiText.text = "";
            yield return null;
            bool skipChars = false;
            foreach (char c in panel.text)
            {
                float ct = 0f;

                uiText.text += c;

                float curCharTime = specialChars.Contains(c) ? timePerSpecialChar : timePerChar;
                while (!skipChars && ct < curCharTime)
                {
                    if (Input.GetButtonDown("Interact") || Input.GetButtonDown("Attack") || Input.GetButtonDown("Jump"))
                    {
                        skipChars = true;
                    }

                    ct += Time.deltaTime;
                    yield return null;
                }
            }

            uiText.text = panel.text;

            yield return new WaitForSeconds(minPanelTime);

            while (!Input.GetButtonDown("Interact") && !Input.GetButtonDown("Attack") && !Input.GetButtonDown("Jump"))
            {
                yield return null;
            }
        }

        _starter.onDialogEnd.Invoke();
        GameEvents.Instance.EndDialog();

        if (fadeInAndOut)
        {
            StartCoroutine(FadeGroup(group, fadeTime, false));
            yield return new WaitForSeconds(fadeTime);
        }

        group.alpha = 0f;
        yield return new WaitForSeconds(0.05f);
        dialoging = false;
    }


    IEnumerator FadeGroup(CanvasGroup _group, float _fadeTime, bool _fadeIn)
    {
        float t = 0f;

        while (t < _fadeTime)
        {
            _group.alpha = !_fadeIn ? 1f - (t / _fadeTime) : t / _fadeTime;

            t += Time.deltaTime;
            yield return null;
        }
    }

}
