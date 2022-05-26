using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/PositiveCount", fileName = "PositiveCount")]
public class PositiveCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        // 성공값이 0보다 크다면 current값에 성공값을 더해주고 아니라면 현재값을 반환
        return successCount > 0 ? currentSuccess + successCount : currentSuccess;   
    }
}
