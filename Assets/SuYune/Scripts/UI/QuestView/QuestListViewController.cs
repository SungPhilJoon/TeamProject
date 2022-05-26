using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class QuestListViewController : MonoBehaviour
{
    [SerializeField]
    private ToggleGroup tabGroup;
    [SerializeField]
    private QuestListView activeQuestListView;
    [SerializeField]
    private QuestListView completedQuestListView;

    public IEnumerable<Toggle> Tabs => tabGroup.ActiveToggles();

    /// <summary>
    /// 퀘스트창에 활성화 된 퀘스트 추가
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="onClicked"></param>
    public void AddQuestToActiveListView(Quest quest, UnityAction<bool> onClicked)
        => activeQuestListView.AddElement(quest, onClicked);

    /// <summary>
    /// 퀘스트 창에서 완료된 퀘스트 삭제
    /// </summary>
    /// <param name="quest"></param>
    public void RemoveQuestFromActiveListView(Quest quest)
        => activeQuestListView.RemoveElement(quest);

    /// <summary>
    /// 퀘스트창에서 완료된 퀘스트 확인
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="onClicked"></param>
    public void AddQuestToCompletedListView(Quest quest, UnityAction<bool> onClicked)
        => completedQuestListView.AddElement(quest, onClicked);
}
