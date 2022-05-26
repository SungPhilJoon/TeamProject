using System.Collections;
using System.Collections.Generic;
using System.Linq;  // public bool IsTarget(object target) => targets.Any(x => x.IsEqual(target)); 을 사용하기 위함
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
    // State가 변할 때마다 알려주는 이벤트(Task의 상태를 Update에서 계속 추적하지 않아도 상태가 바뀌면 알아서 UI가 Update됨)
    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);

    // CurrentSuccess 값이 변할 때 알려주는 이벤트 (다른 곳에서 계속 Update로 추적하지 않아도 되게 하기 위함)
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
    private int needSuccessToComplete;  // 퀘스트 완료에 필요한 횟수
    [SerializeField]
    private bool canReceiveReportsDuringCompletion; // Task가 완료되었어도 "계속 성공횟수를 보고 받을 것이냐?"는 옵션

    private TaskState state;
    private int currentSuccess = 0;

    public event StateChangedHandler onStateChanged;
    public event SuccessChangedHandler onSuccessChanged;
    #endregion

    #region Properties
    public int CurrentSuccess   // 성공한 횟수
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
    public Quest Owner { get; private set; }    // 누가 퀘스트를 가지고 있는지 확인하기 위함
    #endregion

    #region Helper Methods
    /// <summary>
    /// Awake 역할을 하는 함수
    /// </summary>
    /// <param name="owner"></param>
    public void Setup(Quest owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// Task가 시작됐을 때 실행하는 함수
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
    /// Task가 완전히 끝났을 때 실행하는 함수
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
    /// Task를 즉시 완료할 수 있는 함수
    /// </summary>
    public void Complete()
    {
        CurrentSuccess = needSuccessToComplete;
    }

    /// <summary>
    /// Task가 성공 횟수를 보고 받을 대상인지 확인하는 함수
    /// </summary>
    /// <param name="category"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsTarget(string category, object target)
        => Category == category &&  // Category == Category
        targets.Any(x => x.IsEqual(target)) &&    // 세팅해놓은 target들 중 해당하는 target이 있으면 ture 없으면 false
        (!IsComplete || (IsComplete && canReceiveReportsDuringCompletion)); // !IsComplete;
    #endregion
}
