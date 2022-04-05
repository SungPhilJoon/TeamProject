using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacing : MonoBehaviour
{
    #region Variables
    private Camera referenceCamera;
    public bool reverseFace = false;

    public enum Axis
    {
        up, down, left, right, forward, back
    };

    public Axis axis = Axis.up;

    #endregion Variables

    #region Unity Methods
    void Awake()
    {
        if (!referenceCamera)
        {
            referenceCamera = Camera.main;
        }
    }

    void LateUpdate()
    {
        Vector3 targetPos = transform.position + referenceCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back);
        Vector3 targetOrientation = referenceCamera.transform.rotation * GetAxis(axis);

        transform.LookAt(targetPos, targetOrientation);
    }

    #endregion Unity Methods

    #region Helper Methods
    public Vector3 GetAxis(Axis refAxis)
    {
        switch (refAxis)
        {
            case Axis.down:
                return Vector3.down;

            case Axis.forward:
                return Vector3.forward;

            case Axis.back:
                return Vector3.back;

            case Axis.left:
                return Vector3.left;

            case Axis.right:
                return Vector3.right;
        }

        return Vector3.up;
    }

    #endregion Helper Methods
}
