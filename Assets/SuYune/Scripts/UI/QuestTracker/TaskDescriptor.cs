using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    // TextMesh Pro : 유니티에서 글자를 출력하게 하는 네임스페이스

public class TaskDescriptor : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color taskCompletionColor;
    [SerializeField]
    private Color taskSuccessCountColor;
    [SerializeField]
    private Color strikeThroughColor;

    public void UpdateText(string text)
    {
        this.text.fontStyle = FontStyles.Normal;
        this.text.text = text;
    }

    public void UpdateText(Task task)
    {
        text.fontStyle = FontStyles.Normal;

        if (task.IsComplete)    // 완료했다면
        {
            var colorCode = ColorUtility.ToHtmlStringRGB(taskCompletionColor);
            text.text = BuildText(task, colorCode, colorCode);
        }
        else    // 진행 중이라면
            text.text = BuildText(task, ColorUtility.ToHtmlStringRGB(normalColor), ColorUtility.ToHtmlStringRGB(taskSuccessCountColor));
    }

    public void UpdateTextUsingStrikeThrough(Task task)     // StrikeThrough : 취소선, 착선
    {
        var colorCode = ColorUtility.ToHtmlStringRGB(strikeThroughColor);
        text.fontStyle = FontStyles.Strikethrough;
        text.text = BuildText(task, colorCode, colorCode);
    }

    private string BuildText(Task task, string textColorCode, string successCountColorCode)
    {
        return $"<color=#{textColorCode}>● {task.Description} <color=#{successCountColorCode}>{task.CurrentSuccess}</color>/{task.NeedSuccessToComplete}</color>";
    }
}
