using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GateDestination
{
    Village,
    HuntingGround,
    BossGround
}

public class GateController : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform arrivalPoint;

    private CharacterController controller;

    public GateDestination gateDestination;

    #endregion Variables

    #region Unity Methods
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller = other.GetComponent<CharacterController>();

            controller.enabled = false;
            LoadingUIManager.Instance.LoadScene();
            other.transform.position = arrivalPoint.position;
            controller.enabled = true;

            if (gateDestination.Equals(GateDestination.BossGround) && !GameManager.Instance.isCharacterEnterBossGround)
            {
                GameManager.Instance.isCharacterEnterBossGround = true;
                return;
            }
            GameManager.Instance.isCharacterEnterBossGround = false;
        }
    }

    #endregion Unity Methods
}
