using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;     // json�� �̿��ϱ� ����
using UnityEngine;

public class QuestSystem : MonoBehaviour    // �̱������� ���
{
    #region Save Path
    // k�� key�� ���Ӹ�
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
    private static bool isApplicationQuitting = false;  // �� ����

    public static QuestSystem Instance
    {
        get
        {
            if (!isApplicationQuitting || instance == null)
            {
                instance = FindObjectOfType<QuestSystem>();
                if (instance == null)
                {
                    instance = new GameObject("Quest System").AddComponent<QuestSystem>();  // �ν��Ͻ��� ���ٸ� ���� ����� ��
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
        questDatatabase = Resources.Load<QuestDatabase>("QuestDatabase");       // Quest Data �ε�
        // achievementDatabase = Resources.Load<QuestDatabase>("AchievementDatabase"); // Achievement Data �ε�

        // �ε忡 �������� �� (����)
        // if (!Load())
        // {
        //     foreach (var achievement in achievementDatabase.Quests)
        //     {
        //         Register(achievement);
        //     }
        // }
    }

    // �� ���� �� ���� (������ ���׷� ���� ���� �Ǿ��� ���� ������ �ȵ�)
    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
        // Save();
    }

    public Quest Register(Quest quest)
    {
        var newQuest = quest.Clone();   // ����Ʈ ���纻

        newQuest.onCompleted += OnQuestCompleted;
        newQuest.onCanceled += OnQuestCanceled;

        activeQuests.Add(newQuest);

        newQuest.OnRegister();
        onQuestRegistered?.Invoke(newQuest);

        // if (newQuest is Achievement) // �����̶��
        // {
        //     newQuest.onCompleted += OnAchievementCompleted;
        // 
        //     activeAchievements.Add(newQuest);
        // 
        //     newQuest.OnRegister();
        //     onAchievementRegistered?.Invoke(newQuest);
        // }
        // else     // ����Ʈ���
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
    /// �ܺο��� ����� ����޴� �Լ�
    /// </summary>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    public void ReceiveReport(string category, object target, int successCount)
    {
        ReceiveReport(activeQuests, category, target, successCount);
        // ReceiveReport(activeAchievements, category, target, successCount);
    }

    // ���� �Լ��� ���Ǽ��� ���� overload �߰�
    public void ReceiveReport(Category category, TaskTarget target, int successCount)
        => ReceiveReport(category.CodeName, target.Value, successCount);

    /// <summary>
    /// ���ο��� ����� ����޴� �Լ�
    /// </summary>
    /// <param name="quests"></param>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <param name="successCount"></param>
    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        // ToArray�� List�� �纻�� ���� for���� ������ ���� : for���� ���ư��� ���߿� Quest�� Complete�Ǿ� ��Ͽ��� ���� �� �ֱ� ������
        foreach (var quest in quests.ToArray())
            quest.ReceiveReport(category, target, successCount);
    }

    /// <summary>
    /// ����Ʈ�� ��Ͽ� �ִ��� Ȯ��
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
            var root = JObject.Parse(PlayerPrefs.GetString(kSaveRootPath));     // ������ �����͸� json���� ��ȣȭ�� ��

            LoadSaveDatas(root[kActiveQuestsSavePath], questDatatabase, LoadActiveQuest);
            LoadSaveDatas(root[kCompletedQuestsSavePath], questDatatabase, LoadCompletedQuest);

            // LoadSaveDatas(root[kActiveAchievementsSavePath], achievementDatabase, LoadActiveQuest);
            // LoadSaveDatas(root[kCompletedAchievementsSavePath], achievementDatabase, LoadCompletedQuest);

            return true;    // Load�� ����
        }
        else    // ����� �����Ͱ� ���ٸ�
            return false;   // Load�� ����
    }

    /// <summary>
    /// ���̺굥���͸� ����� �Լ� (����Ʈ ����) 
    /// </summary>
    /// <param name="quests"></param>
    /// <returns>saveDatas</returns>
    private JArray CreateSaveDatas(IReadOnlyList<Quest> quests)
    {
        var saveDatas = new JArray();
        foreach (var quest in quests)
        {
            // if (quest.IsSavable)     // ���������� ���̺갡 ��
                saveDatas.Add(JObject.FromObject(quest.ToSaveData()));  // savaData�� json���·� ��ȯ �� jsonArray�� �־��ش�.
        }
        return saveDatas;
    }

    /// <summary>
    /// ���̺굥���͸� �ҷ����� �Լ� (����Ʈ �ҷ�����) 
    /// - DatasToken : CreateSaveDatas�� ����� ������� SavaData�� ����Ǿ��ٰ� Load �ÿ� datasToken �Լ��� ������ ��
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

    // �ҷ��� Quest���� ó���� �Լ�
    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);     // �ҷ��� ����Ʈ ���
        newQuest.LoadFrom(saveData);        // ����� ����Ʈ�� ����� ������ �ֱ�
    }

    private void LoadCompletedQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone();
        newQuest.LoadFrom(saveData);

        completedQuests.Add(newQuest);

        // if (newQuest is Achievement)    // �����̶��
        // {
        //     completedAchievements.Add(newQuest); // �Ϸ�� ������ �߰�
        // }
        // else    // ������ �ƴ϶��(����Ʈ���)
        // {
        //     completedQuests.Add(newQuest);   // �Ϸ�� ����Ʈ�� �߰�
        // }
    }
    #endregion

    #region Callback
    // �Ʒ��� ���� �Լ����� �̺�Ʈ �ֵ� ���α׷����̶� �Ҹ���.
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
