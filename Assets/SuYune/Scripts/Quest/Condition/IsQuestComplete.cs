using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Acception Condition에 사용할 함수 (이전 퀘스트를 깼을 때만 받을 수 있게)
[CreateAssetMenu(menuName = "Quest/Condition/IsQuestComplete", fileName = "IsQuestComplete_")]
public class IsQuestComplete : Condition
{
    [SerializeField]
    private Quest target;

    public override bool IsPass(Quest quest)
        => QuestSystem.Instance.ContainsInCompleteQuests(target);
}