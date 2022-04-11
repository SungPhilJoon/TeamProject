using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float moveSpeed = 0f;

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);   
    }
}
