using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform arrivalPoint;

    private CharacterController controller;

    #endregion Variables

    #region Unity Methods
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller = other.GetComponent<CharacterController>();

            controller.enabled = false;
            other.transform.position = arrivalPoint.position;
            controller.enabled = true;
        }
    }

    #endregion Unity Methods
}
