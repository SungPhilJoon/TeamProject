using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBarUI : MonoBehaviour
{
    public Image hpBar;
    public Text bossHP;

    private BossController bossController;

    private void Awake()
    {
        bossController = GameObject.FindObjectOfType<BossController>();
    }

    public void Refresh()
    {
        
    }
}
