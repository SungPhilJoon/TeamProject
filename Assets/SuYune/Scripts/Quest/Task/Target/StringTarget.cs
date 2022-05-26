using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/String", fileName = "Target_")]
public class StringTarget : TaskTarget
{
    [SerializeField]
    private string value;

    public override object Value => value;
    public override bool IsEqual(object target)
    {
        string targetAsString = target as string;   // target을 string형으로 캐스팅

        // 캐스팅 실패 시 같은 타입이 아니므로 false로 return
        if (targetAsString == null)
        {
            return false;
        }
        // 캐스팅 성공 시 value 값과 비교하여 return
        return value == targetAsString;
    }
}
