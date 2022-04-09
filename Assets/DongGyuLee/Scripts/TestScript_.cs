using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestScript_ : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    private void Update()
    {
        transform.position = target.position;

        transform.eulerAngles =
            new Vector3(Mathf.Clamp(transform.eulerAngles.x + Mouse.current.delta.y.ReadValue(), 10, 80),
                transform.eulerAngles.y + (-Mouse.current.delta.x.ReadValue()), 0);
    }

}
