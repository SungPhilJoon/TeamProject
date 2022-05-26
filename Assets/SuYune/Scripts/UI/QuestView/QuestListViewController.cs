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
    /// ����Ʈâ�� Ȱ��ȭ �� ����Ʈ �߰�
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="onClicked"></param>
    public void AddQuestToActiveListView(Quest quest, UnityAction<bool> onClicked)
        => activeQuestListView.AddElement(quest, onClicked);

    /// <summary>
    /// ����Ʈ â���� �Ϸ�� ����Ʈ ����
    /// </summary>
    /// <param name="quest"></param>
    public void RemoveQuestFromActiveListView(Quest quest)
        => activeQuestListView.RemoveElement(quest);

    /// <summary>
    /// ����Ʈâ���� �Ϸ�� ����Ʈ Ȯ��
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="onClicked"></param>
    public void AddQuestToCompletedListView(Quest quest, UnityAction<bool> onClicked)
        => completedQuestListView.AddElement(quest, onClicked);
}
