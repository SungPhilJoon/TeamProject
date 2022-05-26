using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCView : MonoBehaviour
{
    public NPC npc;
    // public DialogueSystem dialogueSystem;

    public GameObject questGiverView;

    [SerializeField]
    public TextMeshProUGUI npcNameText;
    [SerializeField]
    public TextMeshProUGUI npcTalkText;

    private void Update()
    {
        Show();
    }

    public void Show()
    {
        npcNameText.text = npc.npcName;
        npcTalkText.text = npc.npcDialog;
    }

    public void UpdateText(Quest quest)
    {
        if (quest.IsComplatable)    // 완료했다면
        {
            npcTalkText.text = npc.npcDialog;
            
        }
        else    // 진행 중이라면
            npcTalkText.text = npc.npcDialog;
    }

}
