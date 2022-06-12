using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    Vector3 originPosition;

    void Start()
    {
        originPosition = transform.position;
    }

    void Update()
    {
        transform.position = originPosition + new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0f) * 0.1f;
    }
}
