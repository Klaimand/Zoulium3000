using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLD_QuestInst : MonoBehaviour
{
    KLD_QuestManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("QUEST_Canvas").GetComponent<KLD_QuestManager>();
    }

    public void AddQuest(KLD_Quest _quest)
    {
        manager.AddQuest(_quest);
    }

    public void RemoveQuest(KLD_Quest _quest)
    {
        manager.RemoveQuest(_quest);
    }

    public void RemoveAllQuests()
    {
        manager.RemoveAllQuests();
    }
}
