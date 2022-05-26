/// <summary>
/// 퀘스트의 저장 데이터를 담을 구조체
/// </summary>
public struct QuestSaveData
{
    public string codeName;
    public QuestState state;
    public int taskGroupIndex;
    public int[] taskSuccessCounts;
}
