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

    public Quest FindQuestBy(string codeName) => quests.FirstOrDefault(x => x.CodeName == codeName);    // 퀘스트를 찾아오는 함수

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

    private void FindQuestsBy<T>() where T : Quest  // Quest를 찾아오는 제네릭 함수
    {
        quests = new List<Quest>();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");    // 전역 고유 식별자(에셋 고유 ID 정보)
        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);     // guid를 이용해 에셋이 저장되어 있는 경로 가져오기
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);    // AssetPath로 QuestData 불러오기

            if (quest.GetType() == typeof(T))   // 한 번 더 객체의 타입이 T와 같은지 확인 (퀘스트 객체와 업적 객체를 다 찾아오기 때문에)
                quests.Add(quest);

            EditorUtility.SetDirty(this);   // SetDirty : QuestDatabase 객체가 가진 Serialize 변수가 변화가 생겼으니 Asset을 저장할 때 반영하라는 의미
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
