using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Quest/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
    [SerializeField]
    private List<Quest> quests;

    public IReadOnlyList<Quest> Quests => quests;

    public Quest FindQuestBy(string codeName) => quests.FirstOrDefault(x => x.CodeName == codeName);    // ����Ʈ�� ã�ƿ��� �Լ�

#if UNITY_EDITOR 
    [ContextMenu("FindQuests")]
    private void FindQuests()   
    {
        FindQuestsBy<Quest>();
    }

    [ContextMenu("FindAchievements")]
    private void FindAchievements()
    {
        FindQuestsBy<Achievement>();
    }

    private void FindQuestsBy<T>() where T : Quest  // Quest�� ã�ƿ��� ���׸� �Լ�
    {
        quests = new List<Quest>();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");    // ���� ���� �ĺ���(���� ���� ID ����)
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);     // guid�� �̿��� ������ ����Ǿ� �ִ� ��� ��������
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);    // AssetPath�� QuestData �ҷ�����

            if (quest.GetType() == typeof(T))   // �� �� �� ��ü�� Ÿ���� T�� ������ Ȯ�� (����Ʈ ��ü�� ���� ��ü�� �� ã�ƿ��� ������)
                quests.Add(quest);

            EditorUtility.SetDirty(this);   // SetDirty : QuestDatabase ��ü�� ���� Serialize ������ ��ȭ�� �������� Asset�� ������ �� �ݿ��϶�� �ǹ�
            AssetDatabase.SaveAssets();
        }
    }
#endif
}