using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;   // 어트리뷰트로 Debugging을 하기 위함 ([Conditional("UNITY_EDITOR")] 사용하려고)
using System.Linq;
using UnityEngine;

using Debug = UnityEngine.Debug;    // Debug class를 유니티의 Debug class를 사용하겠다

public enum QuestState
{
    Inactive,   // 퀘스트 비활성화
    Running,    // 퀘스트 진행 중
    Complete,   // 퀘스트가 자동으로 완료되는 상태
    Cancel,     // 퀘스트 취소
    WaitingForCompletion    // 유저가 퀘스트 완료를 해주길 기다리는 상태
}

[CreateAssetMenu(menuName = "Quest/Quest", fileName = "Quest_")]
public class Quest : ScriptableObject
{
    #region Events
    public delegate void TaskSuccessChangedHandler(Quest quest, Task task, int currentSuccess, int prevSuccess);
    public delegate void CompletedHandler(Quest quest);
    public delegate void CanceledHandler(Quest quest);
    public delegate void NewTaskGroupHandler(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup);
    #endregion

    [SerializeField]
    private Category category;
    [SerializeField]
    private Sprite icon;

    [Header("Text")]
    [SerializeField]
    private string codeName;
    [SerializeField]
    private string displayName;
    [SerializeField, TextArea]
    private string description;

    [Header("Task")]
    [SerializeField]
    private TaskGroup[] taskGroups;

    [Header("Rewaed")]
    [SerializeField]
    private Reward[] rewards;

    [Header("Option")]
    [SerializeField]
    private bool useAutoComplete;   // 자동완료를 사용할 것인지
    [SerializeField]
    private bool isCancelable;  // 삭제할 수 있는지 ( 메인 퀘스트와 같이 삭제할 수 없는 퀘스트가 있는 경우때문에 만듬)
    [SerializeField]
    private bool isSavable;     // 저장할 것인지에 대한 변수 (저장되지 않는 일회용 퀘스트 튜토리얼 퀘스트 같은 것)

    [Header("Condotion")]
    [SerializeField]
    private Condition[] acceptionConditions;
    [SerializeField]
    private Condition[] cancelConditions;

    private int currentTaskGroupIndex;

    #region Properties
    public Category Category => category;
    public Sprite Icon => icon;
    public string CodeName => codeName;
    public string DisplayName => displayName;
    public string Description => description;
    public QuestState State { get; private set; }
    public TaskGroup CurrentTaskGroup => taskGroups[currentTaskGroupIndex];
    public IReadOnlyList<TaskGroup> TaskGroups => taskGroups;
    public IReadOnlyList<Reward> Rewards => rewards;
    public bool IsRegistered => State != QuestState.Inactive;
    public bool IsComplatable => State == QuestState.WaitingForCompletion;
    public bool IsComplete => State == QuestState.Complete;
    public bool IsCancel => State == QuestState.Cancel;
    public bool IsCancelable => isCancelable && cancelConditions.All(x => x.IsPass(this));  // 조건 취소 // public bool IsCancelable => isCancelable;
    public bool IsAcceptable => acceptionConditions.All(x => x.IsPass(this));
    public virtual bool IsSavable => isSavable;     // virtual 을 넣은 이유는 업적은 세이브를 꼭 해야하기 때문
    #endregion

    public event TaskSuccessChangedHandler onTaskSuccessChanged;
    public event CompletedHandler onCompleted;
    public event CanceledHandler onCanceled;
    public event NewTaskGroupHandler onNewTaskGroup;

    /// <summary>
    /// Awake 역할의 함수 (Quest가 System에 등록되었을때 실행)
    /// </summary>
    public void OnRegister()
    {
        Debug.Assert(!IsRegistered, "This quest has already been registered.");

        foreach (var taskGroup in taskGroups)
        {
            taskGroup.SetUp(this);
            foreach(var task in taskGroup.Tasks)
            {
                task.onSuccessChanged += OnSuccessChanged;
            }
        }

        State = QuestState.Running;
        CurrentTaskGroup.Start();
    }

    /// <summary>
    /// 보고를 받을 때 사용할 함수
    /// </summary>
    public void ReceiveReport(string category, object target, int successCount)
    {
        // Debug.Assert(!IsRegistered, "This quest has already been registered.");
        // Debug.Assert(!IsCancel, "This quest has been canceled.");

        if (IsComplete)
        {
            return;
        }

        CurrentTaskGroup.ReceiveReport(category, target, successCount);

        // AllComplete 라면
        if (CurrentTaskGroup.IsAllTaskComplete)
        {
            // 다음 taskGroup이 없다면
            if (currentTaskGroupIndex + 1 == taskGroups.Length)
            {
                State = QuestState.WaitingForCompletion;
                // autoComplete가 활성화 되어있다면
                if (useAutoComplete)
                {
                    Complete();
                }
            }
            // 다음 taskGroup이 존재한다면
            else
            {
                var prevTaskGroup = taskGroups[currentTaskGroupIndex++];
                prevTaskGroup.End();
                CurrentTaskGroup.Start();
                onNewTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
            }
        }
        // AllComplete가 아니라면
        else
        {
            State = QuestState.Running;
        }
    }

    /// <summary>
    /// 퀘스트를 완료하는 함수
    /// </summary>
    public void Complete()
    {
        // CheckIsRunning();

        foreach (var taskGroup in taskGroups)
        {
            taskGroup.Complete();
        }

        State = QuestState.Complete;

        // 완료 시 보상획득
        foreach (var reward in rewards)
        {
            reward.Give(this);
        }

        onCompleted?.Invoke(this);

        onTaskSuccessChanged = null;
        onCompleted = null;
        onCanceled = null;
        onNewTaskGroup = null;
    }

    /// <summary>
    /// 퀘스트를 삭제하는 함수
    /// </summary>
    public void Cancel()
    {
        // CheckIsRunning();
        // Debug.Assert(IsCancelable, "This quest can't be canceled.");
        State = QuestState.Cancel;
        onCanceled?.Invoke(this);
    }

    /// <summary>
    /// Cloning 하기 위한 함수
    /// </summary>
    /// <returns></returns>
    public Quest Clone()
    {
        var clone = Instantiate(this);
        clone.taskGroups = taskGroups.Select(x => new TaskGroup(x)).ToArray();

        return clone;
    }

    /// <summary>
    /// 저장 데이터 만들기
    /// </summary>
    /// <returns></returns>
    public QuestSaveData ToSaveData()
    {
        return new QuestSaveData
        {
            codeName = codeName,
            state = State,
            taskGroupIndex = currentTaskGroupIndex,
            taskSuccessCounts = CurrentTaskGroup.Tasks.Select(x => x.CurrentSuccess).ToArray()
        };
    }

    /// <summary>
    /// 저장 데이터 불러오기
    /// </summary>
    /// <param name="saveData"></param>
    public void LoadFrom(QuestSaveData saveData)
    {
        State = saveData.state;
        currentTaskGroupIndex = saveData.taskGroupIndex;

        for (int i = 0; i < currentTaskGroupIndex; i++)
        {
            var taskGroup = taskGroups[i];
            taskGroup.Start();
            taskGroup.Complete();
        }

        for (int i = 0; i < saveData.taskSuccessCounts.Length; i++)
        {
            CurrentTaskGroup.Start();
            CurrentTaskGroup.Tasks[i].CurrentSuccess = saveData.taskSuccessCounts[i];
        }
    }


/// <summary>
/// onTaskSuccessChanged 이벤트를 Task의 이벤트에 등록하기 위해 만든 Callback 함수
/// </summary>
public void OnSuccessChanged(Task task, int currentSuccess, int prevSuccess)
        => onTaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);

    // [Conditional("UNITY_EDITOR")]   // UNITY_EDTOR는 유니티엔진 내부에 선언되어 있는 심볼
    // private void CheckIsRunning()   // Assert처럼 디버깅용 함수를 만듬
    // {
    //     Debug.Assert(!IsRegistered, "This quest has already been registered.");
    //     Debug.Assert(!IsCancel, "This quest has been canceled.");
    //     Debug.Assert(!IsComplatable, "This quest has already been completed.");
    // }
}
