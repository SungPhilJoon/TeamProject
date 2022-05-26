using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;   // ��Ʈ����Ʈ�� Debugging�� �ϱ� ���� ([Conditional("UNITY_EDITOR")] ����Ϸ���)
using System.Linq;
using UnityEngine;

using Debug = UnityEngine.Debug;    // Debug class�� ����Ƽ�� Debug class�� ����ϰڴ�

public enum QuestState
{
    Inactive,   // ����Ʈ ��Ȱ��ȭ
    Running,    // ����Ʈ ���� ��
    Complete,   // ����Ʈ�� �ڵ����� �Ϸ�Ǵ� ����
    Cancel,     // ����Ʈ ���
    WaitingForCompletion    // ������ ����Ʈ �ϷḦ ���ֱ� ��ٸ��� ����
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
    private bool useAutoComplete;   // �ڵ��ϷḦ ����� ������
    [SerializeField]
    private bool isCancelable;  // ������ �� �ִ��� ( ���� ����Ʈ�� ���� ������ �� ���� ����Ʈ�� �ִ� ��춧���� ����)
    [SerializeField]
    private bool isSavable;     // ������ �������� ���� ���� (������� �ʴ� ��ȸ�� ����Ʈ Ʃ�丮�� ����Ʈ ���� ��)

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
    public bool IsCancelable => isCancelable && cancelConditions.All(x => x.IsPass(this));  // ���� ��� // public bool IsCancelable => isCancelable;
    public bool IsAcceptable => acceptionConditions.All(x => x.IsPass(this));
    public virtual bool IsSavable => isSavable;     // virtual �� ���� ������ ������ ���̺긦 �� �ؾ��ϱ� ����
    #endregion

    public event TaskSuccessChangedHandler onTaskSuccessChanged;
    public event CompletedHandler onCompleted;
    public event CanceledHandler onCanceled;
    public event NewTaskGroupHandler onNewTaskGroup;

    /// <summary>
    /// Awake ������ �Լ� (Quest�� System�� ��ϵǾ����� ����)
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
    /// ���� ���� �� ����� �Լ�
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

        // AllComplete ���
        if (CurrentTaskGroup.IsAllTaskComplete)
        {
            // ���� taskGroup�� ���ٸ�
            if (currentTaskGroupIndex + 1 == taskGroups.Length)
            {
                State = QuestState.WaitingForCompletion;
                // autoComplete�� Ȱ��ȭ �Ǿ��ִٸ�
                if (useAutoComplete)
                {
                    Complete();
                }
            }
            // ���� taskGroup�� �����Ѵٸ�
            else
            {
                var prevTaskGroup = taskGroups[currentTaskGroupIndex++];
                prevTaskGroup.End();
                CurrentTaskGroup.Start();
                onNewTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
            }
        }
        // AllComplete�� �ƴ϶��
        else
        {
            State = QuestState.Running;
        }
    }

    /// <summary>
    /// ����Ʈ�� �Ϸ��ϴ� �Լ�
    /// </summary>
    public void Complete()
    {
        // CheckIsRunning();

        foreach (var taskGroup in taskGroups)
        {
            taskGroup.Complete();
        }

        State = QuestState.Complete;

        // �Ϸ� �� ����ȹ��
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
    /// ����Ʈ�� �����ϴ� �Լ�
    /// </summary>
    public void Cancel()
    {
        // CheckIsRunning();
        // Debug.Assert(IsCancelable, "This quest can't be canceled.");
        State = QuestState.Cancel;
        onCanceled?.Invoke(this);
    }

    /// <summary>
    /// Cloning �ϱ� ���� �Լ�
    /// </summary>
    /// <returns></returns>
    public Quest Clone()
    {
        var clone = Instantiate(this);
        clone.taskGroups = taskGroups.Select(x => new TaskGroup(x)).ToArray();

        return clone;
    }

    /// <summary>
    /// ���� ������ �����
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
    /// ���� ������ �ҷ�����
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
/// onTaskSuccessChanged �̺�Ʈ�� Task�� �̺�Ʈ�� ����ϱ� ���� ���� Callback �Լ�
/// </summary>
public void OnSuccessChanged(Task task, int currentSuccess, int prevSuccess)
        => onTaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);

    // [Conditional("UNITY_EDITOR")]   // UNITY_EDTOR�� ����Ƽ���� ���ο� ����Ǿ� �ִ� �ɺ�
    // private void CheckIsRunning()   // Assertó�� ������ �Լ��� ����
    // {
    //     Debug.Assert(!IsRegistered, "This quest has already been registered.");
    //     Debug.Assert(!IsCancel, "This quest has been canceled.");
    //     Debug.Assert(!IsComplatable, "This quest has already been completed.");
    // }
}
