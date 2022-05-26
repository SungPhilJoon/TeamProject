using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/PositiveCount", fileName = "PositiveCount")]
public class PositiveCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        // �������� 0���� ũ�ٸ� current���� �������� �����ְ� �ƴ϶�� ���簪�� ��ȯ
        return successCount > 0 ? currentSuccess + successCount : currentSuccess;   
    }
}
