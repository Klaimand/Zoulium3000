using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_PopupManager : MonoBehaviour
{
    [SerializeField] float fadeTime = 0.2f;
    [SerializeField] List<PopupEmpty> popups = new List<PopupEmpty>();

    void Start()
    {
        DisableAllPopups();
    }

    void OnValidate()
    {
        foreach (var popup in popups)
        {
            popup.OnValidate(this);
        }
    }

    public void EnablePopup(string _id)
    {
        GetPopup(_id).Enable(fadeTime);
    }

    public void DisablePopup(string _id)
    {
        GetPopup(_id).Disable(fadeTime);
    }

    public void DisableAllPopups()
    {
        for (int i = 0; i < popups.Count; i++)
        {
            popups[i].ForceDisable(0f);
        }
    }

    PopupEmpty GetPopup(string _id)
    {
        for (int i = 0; i < popups.Count; i++)
        {
            if (popups[i].popUpID == _id)
            {
                return popups[i];
            }
        }

        Debug.LogError("No popup found with id : " + _id);
        return null;
    }
}

[System.Serializable]
public class PopupEmpty
{
    public string popUpID = "id";
    [SerializeField] GameObject popUpEmpty;
    CanvasGroup group;
    public bool enabled { get; private set; } = false;

    MonoBehaviour mono;

    public void Enable(float _fadeTime)
    {
        if (!enabled)
        {
            //popUpEmpty.SetActive(true);
            enabled = true;
            mono.StartCoroutine(FadeGroup(group, _fadeTime, true));
        }
    }

    public void Disable(float _fadeTime)
    {
        if (enabled)
        {
            //popUpEmpty.SetActive(false);
            mono.StartCoroutine(FadeGroup(group, _fadeTime, false));
            mono.StartCoroutine(WaitAndChangeEnabling(_fadeTime + 0.05f, false));
        }
    }

    public void ForceDisable(float _fadeTime)
    {
        mono.StartCoroutine(FadeGroup(group, _fadeTime, false));
        mono.StartCoroutine(WaitAndChangeEnabling(_fadeTime + 0.05f, false));
    }

    public void OnValidate(MonoBehaviour _mono)
    {
        if (popUpEmpty != null && group == null)
        {
            try
            {
                group = popUpEmpty.GetComponent<CanvasGroup>();
            }
            catch (System.Exception)
            {
                Debug.LogError("PopUp Empty '" + popUpID + "' need a CanvasGroup component");
                throw;
            }
        }
        if (mono == null)
        {
            mono = _mono;
        }
    }

    IEnumerator WaitAndChangeEnabling(float _fadeTime, bool _enable)
    {
        yield return new WaitForSeconds(_fadeTime);
        enabled = _enable;
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

        _group.alpha = _fadeIn ? 1f : 0f;
    }
}