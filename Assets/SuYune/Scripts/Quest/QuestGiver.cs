using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour
{
    // [SerializeField]
    // public Text npcName;
    [SerializeField]
    private Quest[] quests;

    public TaskGroup taskGroups;

    private void Start()
    {
        // 시작시 퀘스트 등록
        // foreach (var quest in quests)
        // {
        //     if (quest.IsAcceptable && !QuestSystem.Instance.ContainsInCompleteQuests(quest))
        //     {
        //         QuestSystem.Instance.Register(quest);
        //     }
        // }
    }


    public void OnClickedAcceptBtn()
    {
        foreach (var quest in quests)
        {
            if (quest.IsAcceptable && !QuestSystem.Instance.ContainsInCompleteQuests(quest))
            {
                QuestSystem.Instance.Register(quest);
            }
        }

    }

    public void OnClickedCompleteBtn()
    {
        foreach (var quest in quests)
        {
            if (taskGroups.IsAllTaskComplete && QuestSystem.Instance.ContainsInActiveQuests(quest))
            {
                QuestSystem.Instance.CompleteWaitingQuests();
            }
        }
    }
}