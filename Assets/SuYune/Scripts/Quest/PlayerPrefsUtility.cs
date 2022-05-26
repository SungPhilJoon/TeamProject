using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsUtility : MonoBehaviour
{
    [ContextMenu("DeleteSaveData")]
    private void DeleteSaveData()   // 저장 데이터 삭제
    {
        PlayerPrefs.DeleteAll();
    }
}