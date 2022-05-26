using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpin : MonoBehaviour
{
    public float spinSpeed = 360f;

    private void Update()
    {
        transform.Rotate(-Vector3.forward * spinSpeed * Time.deltaTime);
    }
}
