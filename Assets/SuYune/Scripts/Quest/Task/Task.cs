using System.Collections;
using System.Collections.Generic;
using System.Linq;  // public bool IsTarget(object target) => targets.Any(x => x.IsEqual(target)); �� ����ϱ� ����
using UnityEngine;

public enum TaskState
{
    Inactive,
    Running,
    Complete
}

[CreateAssetMenu(menuName = "Quest/Task/Task", fileName = "Task_")]
public class Task : ScriptableObject
{
    #region Vriables

    #region Events
    // State�� ���� ������ �˷��ִ� �̺�Ʈ(Task�� ���¸� Update���� ��� �������� �ʾƵ� ���°� �ٲ�� �˾Ƽ� UI�� Update��)
    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);

    // CurrentSuccess ���� ���� �� �˷��ִ� �̺�Ʈ (�ٸ� ������ ��� Update�� �������� �ʾƵ� �ǰ� �ϱ� ����)
    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    #endregion

    [SerializeField]
    private Category category;

    [Header("Text")]
    [SerializeField]
    private string codeName;
    [SerializeField]
    private string description;

    [Header("Action")]
    [SerializeField]
    private TaskAction action;

    [Header("Target")]
    [SerializeField]
    private TaskTarget[] targets;

    [Header("Setting")]
    [SerializeField]
    private InitialSuccessValue initialSuccessValue;
    [SerializeField]
    private int needSuccessToComplete;  // ����Ʈ �Ϸῡ �ʿ��� Ƚ��
    [SerializeField]
    private bool canReceiveReportsDuringCompletion; // Task�� �Ϸ�Ǿ�� "��� ����Ƚ���� ���� ���� ���̳�?"�� �ɼ�

    private TaskState state;
    private int currentSuccess = 0;

    public event StateChangedHandler onStateChanged;
    public event SuccessChangedHandler onSuccessChanged;
    #endregion

    #region Properties
    public int CurrentSuccess   // ������ Ƚ��
    {
        get => currentSuccess;
        set
        {
            int prevSuccess = currentSuccess;
            currentSuccess = Mathf.Clamp(value, 0, needSuccessToComplete);
            if (currentSuccess != prevSuccess)
            {
                State = currentSuccess == needSuccessToComplete ? TaskState.Complete : TaskState.Running;
                onSuccessChanged?.Invoke(this, currentSuccess, prevSuccess);
            }
        }
    }
    public Category Category => category;

    public string CodeName => codeName;
    public string Description => description;
    public int NeedSuccessToComplete => needSuccessToComplete;
    public TaskState State
    {
        get => state;
        set
        {
            var prevState = state;
            state = value;
            onStateChanged?.Invoke(this, state, prevState);
        }
    }
    public bool IsComplete => State == TaskState.Complete;
    public Quest Owner { get; private set; }    // ���� ����Ʈ�� ������ �ִ��� Ȯ���ϱ� ����
    #endregion

    #region Helper Methods
    /// <summary>
    /// Awake ������ �ϴ� �Լ�
    /// </summary>
    /// <param name="owner"></param>
    public void Setup(Quest owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// Task�� ���۵��� �� �����ϴ� �Լ�
    /// </summary>
    public void Start()
    {
        State = TaskState.Running;
        if (initialSuccessValue)
        {
            CurrentSuccess = initialSuccessValue.GetValue(this);
        }
    }

    /// <summary>
    /// Task�� ������ ������ �� �����ϴ� �Լ�
    /// </summary>
    public void End()
    {
        onStateChanged = null;
        onSuccessChanged = null;
    }
    public void ReceiveReport(int successCount)
    {
        CurrentSuccess = action.Run(this, CurrentSuccess, successCount);
    }

    /// <summary>
    /// Task�� ��� �Ϸ��� �� �ִ� �Լ�
    /// </summary>
    public void Complete()
    {
        CurrentSuccess = needSuccessToComplete;
    }

    /// <summary>
    /// Task�� ���� Ƚ���� ���� ���� ������� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsTarget(string category, object target)
        => Category == category &&  // Category == Category
        targets.Any(x => x.IsEqual(target)) &&    // �����س��� target�� �� �ش��ϴ� target�� ������ ture ������ false
        (!IsComplete || (IsComplete && canReceiveReportsDuringCompletion)); // !IsComplete;
    #endregion
}
