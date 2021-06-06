using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class KLD_QuestManager : SerializedMonoBehaviour
{
    Transform child;

    [SerializeField] float lineHeight = 90f;
    [SerializeField] GameObject questLine = null;

    [SerializeField, Header("Anims")] float lerpTime = 0.2f;
    [SerializeField] float questFadeTime = 0.2f;

    List<KLD_Quest> curQuests = new List<KLD_Quest>();
    List<RectTransform> transforms = new List<RectTransform>();

    //[SerializeField, Header("DEBUG")] KLD_Quest testQuest;

    // Start is called before the first frame update
    void Start()
    {
        child = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void AddQuest(KLD_Quest _quest)
    {
        if (!curQuests.Contains(_quest))
        {

            RectTransform l = Instantiate(questLine).GetComponent<RectTransform>();
            l.SetParent(child);
            l.localPosition = Vector3.down * lineHeight * curQuests.Count;
            l.GetChild(1).GetComponent<Text>().text = _quest.questTitle;

            StartCoroutine(FadeGroup(l.gameObject.GetComponent<CanvasGroup>(), questFadeTime, true));

            curQuests.Add(_quest);
            transforms.Add(l);
        }
    }

    public void RemoveQuest(KLD_Quest _quest)
    {
        if (curQuests.Contains(_quest))
        {
            int j = GetQuestIndex(_quest);

            StartCoroutine(FadeGroup(transforms[j].gameObject.GetComponent<CanvasGroup>(), questFadeTime, false));
            Destroy(transforms[j].gameObject, questFadeTime + 0.1f);
            transforms.RemoveAt(j);

            if (transforms.Count > j)
            {
                for (int i = j; i < transforms.Count; i++)
                {
                    //transforms[i].localPosition += Vector3.up * lineHeight;
                    StartCoroutine(LerpUp(transforms[i], lineHeight, lerpTime));
                }
            }


            curQuests.Remove(_quest);
        }
    }

    int GetQuestIndex(KLD_Quest _quest)
    {
        for (int i = 0; i < curQuests.Count; i++)
        {
            if (_quest == curQuests[i])
            {
                return i;
            }
        }
        return 0;
    }

    IEnumerator LerpUp(RectTransform _transform, float _height, float _time)
    {
        float t = 0f;
        float startPos = _transform.localPosition.y;
        float endPos = startPos + _height;

        while (t < _time)
        {
            _transform.localPosition = Vector3.up * Mathf.Lerp(startPos, endPos, t / _time);

            t += Time.deltaTime;
            yield return null;
        }
        _transform.localPosition = Vector3.up * endPos;
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

    //[Button]
    //public void AddTestQuest()
    //{
    //AddQuest(testQuest);
    //}

    //[Button]
    //public void RemoveTestQuest()
    //{
    //RemoveQuest(testQuest);
    //}
}