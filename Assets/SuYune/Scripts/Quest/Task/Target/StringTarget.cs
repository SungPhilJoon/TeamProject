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
        string targetAsString = target as string;   // target�� string������ ĳ����

        // ĳ���� ���� �� ���� Ÿ���� �ƴϹǷ� false�� return
        if (targetAsString == null)
        {
            return false;
        }
        // ĳ���� ���� �� value ���� ���Ͽ� return
        return value == targetAsString;
    }
}
