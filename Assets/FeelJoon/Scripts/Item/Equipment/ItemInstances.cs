using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInstances : MonoBehaviour
{
    #region Variables
    public List<Transform> itemTransforms = new List<Transform>();

    #endregion Variables

    #region Helper Methods
    private void OnDestroy()
    {
        foreach(Transform item in itemTransforms)
        {
            Destroy(item.gameObject);
        }
    }

    #endregion Helper Methods
}
