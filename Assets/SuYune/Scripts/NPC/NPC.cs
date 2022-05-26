using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField]
    public string npcName;
    [SerializeField]
    public string npcDialog;

    private NPCView npcView;
    public GameObject questGiverView;

    public string NpcName => npcName;
    public string NpcDialog => npcDialog;


    public void OnTriggerEnter(Collider other)
    {
        questGiverView.SetActive(true);
    }

    public void OnTriggerExit(Collider other)
    {
        questGiverView.SetActive(false);
    }
}
