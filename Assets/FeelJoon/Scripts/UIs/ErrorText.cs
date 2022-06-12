using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class ErrorText : MonoBehaviour
    {
        #region Variables
        public float delay;

        private Animator animator;

        protected readonly int hashIsError = Animator.StringToHash("IsError");
        protected readonly int hashError = Animator.StringToHash("Error");

        #endregion Variables

        #region Unity Methods
        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        void OnEnable()
        {
            // animator.SetBool(hashIsError, enabled);
            animator.SetTrigger(hashError);
            StartCoroutine(SetOffText(delay));
        }

        #endregion Unity Methods

        #region Helper Methods
        private IEnumerator SetOffText(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }

        #endregion Helper Methods
    }
}