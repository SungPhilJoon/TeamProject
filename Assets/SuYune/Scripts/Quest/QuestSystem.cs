using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;     // json을 이용하기 위함
using UnityEngine;

public class QuestSystem : MonoBehaviour    // 싱글톤으로 사용
{
    #region Save Path
    // k는 key의 줄임말
    private const string kSaveRootPath = "questSystem";
    private const string kActiveQuestsSavePath = "activeQuests";
    private const string kCompletedQuestsSavePath = "completedQuests";
    // private const string kActiveAchievementsSavePath = "activeAchievement";
    // private const string kCompletedAchievementsSavePath = "completedAchievement";
    #endregion

    #region Events
    public delegate void QuestRegisteredHandler(Quest newQuest);
    public delegate void QuestCompletedHandler(Quest quest);
    public delegate void QuestCanceledHandler(Quest quest);
    #endregion

    private static QuestSystem instance;
    private static bool isApplicationQuitting = false;  // 앱 종료

    public static QuestSystem Instance
    {
        get
        {
            if (!isApplicationQuitting || instance == null)
            {
                instance = FindObjectOfType<QuestSystem>();
                if (instance == null)
                {
                    instance = new GameObject("Quest System").AddComponent<QuestSystem>();  // 인스턴스가 없다면 새로 만들어 줌
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
            return instance;
        }
    }

    private List<Quest> activeQuests = new List<Quest>();
    [SerializeField]
    private List<Quest> completedQuests = new List<Quest>();

    // private List<Quest> activeAchievements = new List<Quest>();
    // private List<Quest> completedAchievements = new List<Quest>();

    private QuestDatabase questDatatabase;
    // private QuestDatabase achievementDatabase;

    public event QuestRegisteredHandler onQuestRegistered;
    public event QuestCompletedHandler onQuestCompleted;
    public event QuestCanceledHandler onQuestCanceled;

    // public event QuestRegisteredHandler onAchievementRegistered;
    // public event QuestCompletedHandler onAchievementCompleted;

    public IReadOnlyList<Quest> ActiveQuests => activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => completedQuests;
    // public IReadOnlyList<Quest> ActiveAchievements => activeAchievements;
    // public IReadOnlyList<Quest> CompletedAchievements => completedAchievements;

    private void Awake()
    {
        questDatatabase = Resources.Load<QuestDatabase>("QuestDatabase");       // Quest Data 로드
        // achievementDatabase = Resources.Load<QuestDatabase>("AchievementDatabase"); // Achievement Data 로드

        // 로드에 실패했을 시 (업적)
        // if (!Load())
        // {
        //     foreach (var achievement in achievementDatabase.Quests)
        //     {
        //         Register(achievement);
        //     }
        // }
    }

    // 앱 종료 시 저장 (오류나 버그로 인해 강종 되었을 때는 저장이 안됨)
    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
        // Save();
    }

    public Quest Register(Quest quest)
    {
        var newQuest = quest.Clone();   // 퀘스트 복사본

        newQuest.onCompleted += OnQuestCompleted;
        newQuest.onCanceled += OnQuestCanceled;

        activeQuests.Add(newQuest);

        newQuest.OnRegister();
        onQuestRegistered?.Invoke(newQuest);

        // if (newQuest is Achievement) // 업적이라면
        // {
        //     newQuest.onCompleted += OnAchievementCompleted;
        // 
        //     activeAchievements.Add(newQuest);
        // 
        //     newQuest.OnRegister();
        //     onAchievementRegistered?.Invoke(newQuest);
        // }
        // else     // 퀘스트라면
        // {
        //     newQuest.onCompleted += OnQuestCompleted;
        //     newQuest.onCanceled += OnQuestCanceled;
        // 
        //     activeQuests.Add(newQuest);
        // 
        //     newQuest.OnRegister();
        //     onQuestRegistered?.Invoke(newQuest);
        // }

        return newQuest;
    }

    /// <summary>
    /// 외부에서 사용할 보고받는 함수
    /// </summary>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    public void ReceiveReport(string category, object target, int successCount)
    {
        ReceiveReport(activeQuests, category, target, successCount);
        // ReceiveReport(activeAchievements, category, target, successCount);
    }

    // 위의 함수의 편의성을 위해 overload 추가
    public void ReceiveReport(Category category, TaskTarget target, int successCount)
        => ReceiveReport(category.CodeName, target.Value, successCount);

    /// <summary>
    /// 내부에서 사용할 보고받는 함수
    /// </summary>
    /// <param name="quests"></param>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        // ToArray로 List의 사본을 만들어서 for문을 돌리는 이유 : for문이 돌아가는 와중에 Quest가 Complete되어 목록에서 빠질 수 있기 때문에
        foreach (var quest in quests.ToArray())
            quest.ReceiveReport(category, target, successCount);
    }

    /// <summary>
    /// 퀘스트가 목록에 있는지 확인
    /// </summary>
    /// <param name="quest"></param>
    /// <returns></returns>
    public bool ContainsInActiveQuests(Quest quest) => activeQuests.Any(x => x.CodeName == quest.CodeName);

    public bool ContainsInCompleteQuests(Quest quest) => completedQuests.Any(x => x.CodeName == quest.CodeName);

    // public bool ContainsInActiveAchievements(Quest quest) => activeAchievements.Any(x => x.CodeName == quest.CodeName);

    // public bool ContainsInCompletedAchievements(Quest quest) => completedAchievements.Any(x => x.CodeName == quest.CodeName);

    public void CompleteWaitingQuests()
    {
        foreach (var quest in activeQuests.ToList())
        {
            if (quest.IsComplatable)
                quest.Complete();
        }
    }

    #region Json Methods
    public void Save()
    {
        var root = new JObject();
        root.Add(kActiveQuestsSavePath, CreateSaveDatas(activeQuests));
        root.Add(kCompletedQuestsSavePath, CreateSaveDatas(completedQuests));
        // root.Add(kActiveAchievementsSavePath, CreateSaveDatas(activeAchievements));
        // root.Add(kCompletedAchievementsSavePath, CreateSaveDatas(activeAchievements));

        PlayerPrefs.SetString(kSaveRootPath, root.ToString());
        PlayerPrefs.Save();
    }

    public bool Load()
    {
        if (PlayerPrefs.HasKey(kSaveRootPath))
        {
            var root = JObject.Parse(PlayerPrefs.GetString(kSaveRootPath));     // 저장한 데이터를 json으로 복호화한 뒤

            LoadSaveDatas(root[kActiveQuestsSavePath], questDatatabase, LoadActiveQuest);
            LoadSaveDatas(root[kCompletedQuestsSavePath], questDatatabase, LoadCompletedQuest);

            // LoadSaveDatas(root[kActiveAchievementsSavePath], achievementDatabase, LoadActiveQuest);
            // LoadSaveDatas(root[kCompletedAchievementsSavePath], achievementDatabase, LoadCompletedQuest);

            return true;    // Load에 성공
        }
        else    // 저장된 데이터가 없다면
            return false;   // Load에 실패
    }

    /// <summary>
    /// 세이브데이터를 만드는 함수 (퀘스트 저장) 
    /// </summary>
    /// <param name="quests"></param>
    /// <returns>saveDatas</returns>
    private JArray CreateSaveDatas(IReadOnlyList<Quest> quests)
    {
        var saveDatas = new JArray();
        foreach (var quest in quests)
        {
            // if (quest.IsSavable)     // 선택적으로 세이브가 됨
                saveDatas.Add(JObject.FromObject(quest.ToSaveData()));  // savaData를 json형태로 변환 후 jsonArray에 넣어준다.
        }
        return saveDatas;
    }

    /// <summary>
    /// 세이브데이터를 불러오는 함수 (퀘스트 불러오기) 
    /// - DatasToken : CreateSaveDatas의 결과로 만들어진 SavaData가 저장되었다가 Load 시에 datasToken 함수로 들어오게 됨
    /// </summary>
    /// <param name="dataToken"></param>
    /// <param name="database"></param>
    /// <param name="onSuccess"></param>
    private void LoadSaveDatas(JToken datasToken, QuestDatabase database, System.Action<QuestSaveData, Quest> onSuccess)
    {
        var datas = datasToken as JArray;
        foreach (var data in datas)
        {
            var saveData = data.ToObject<QuestSaveData>();
            var quest = database.FindQuestBy(saveData.codeName);
            onSuccess.Invoke(saveData, quest);
        }
    }

    // 불러온 Quest들을 처리할 함수
    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);     // 불러온 퀘스트 등록
        newQuest.LoadFrom(saveData);        // 등록한 퀘스트에 저장된 데이터 넣기
    }

    private void LoadCompletedQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone();
        newQuest.LoadFrom(saveData);

        completedQuests.Add(newQuest);

        // if (newQuest is Achievement)    // 업적이라면
        // {
        //     completedAchievements.Add(newQuest); // 완료된 업적에 추가
        // }
        // else    // 업적이 아니라면(퀘스트라면)
        // {
        //     completedQuests.Add(newQuest);   // 완료된 퀘스트에 추가
        // }
    }
    #endregion

    #region Callback
    // 아래와 같은 함수들은 이벤트 주도 프로그래밍이라 불린다.
    private void OnQuestCompleted(Quest quest)
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        onQuestCompleted?.Invoke(quest);
    }

    private void OnQuestCanceled(Quest quest)
    {
        activeQuests.Remove(quest);
        onQuestCanceled?.Invoke(quest);

        Destroy(quest, Time.deltaTime);
    }

    // private void OnAchievementCompleted(Quest achievement)
    // {
    //     activeAchievements.Remove(achievement);
    //     completedAchievements.Add(achievement);
    // 
    //     onAchievementCompleted?.Invoke(achievement);
    // }
    #endregion
}
