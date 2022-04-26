using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;
using ETeam.FeelJoon;

public class GameManager : Singleton<GameManager>
{
    #region Variables
    private GameObject player = GameObject.FindGameObjectWithTag("Player");

    #endregion Variables

    #region Properties
    public GameObject Player => player;

    public bool IsPlayerDead => !player.gameObject.activeSelf;

    #endregion Properties

}
