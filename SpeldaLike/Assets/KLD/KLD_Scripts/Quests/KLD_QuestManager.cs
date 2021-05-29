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

    [SerializeField] List<KLD_Quest> curQuests = new List<KLD_Quest>();

    [SerializeField] List<RectTransform> transforms = new List<RectTransform>();

    [SerializeField, Header("DEBUG")] KLD_Quest testQuest;

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
            l.GetChild(0).GetComponent<Text>().text = _quest.questTitle;

            curQuests.Add(_quest);
            transforms.Add(l);
        }
    }

    public void RemoveQuest(KLD_Quest _quest)
    {
        if (curQuests.Contains(_quest))
        {
            int j = GetQuestIndex(_quest);

            Destroy(transforms[j].gameObject);
            transforms.RemoveAt(j);

            if (transforms.Count > j)
            {
                for (int i = j; i < transforms.Count; i++)
                {
                    transforms[i].localPosition += Vector3.up * lineHeight;
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

    [Button]
    public void AddTestQuest()
    {
        AddQuest(testQuest);
    }

    [Button]
    public void RemoveTestQuest()
    {
        RemoveQuest(testQuest);
    }
}