using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainImageController : MonoBehaviour
{
    #region Variables
    public float delay;

    public GameObject bossObject;

    private List<GameObject> mainImages = new List<GameObject>();

    #endregion Variables

    #region Unity Methods
    void Start()
    {
        foreach (Transform mainImage in this.transform)
        {
            mainImages.Add(mainImage.gameObject);
        }

        StartCoroutine(ChangeMainImageWithDelay(delay));
    }

    #endregion Unity Methods

    #region Helper Methods
    private IEnumerator ChangeMainImageWithDelay(float delay)
    {
        int index = 0;
        int count = mainImages.Count;

        while (true)
        {
            if ((index % count).Equals(4))
            {
                bossObject.SetActive(true);
            }

            mainImages[index % count].SetActive(true);

            yield return new WaitForSeconds(delay);

            mainImages[index % count].SetActive(false);
            ++index;

            if (bossObject.activeSelf == true)
            {
                bossObject.SetActive(false);
            }
        }
    }

    #endregion Helper Methods
}
