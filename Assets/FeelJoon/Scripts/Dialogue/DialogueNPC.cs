using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public class DialogueNPC : MonoBehaviour, IInteractable
    {
        #region Variables
        [SerializeField] Dialogue dialogue;

        private bool isStartDialogue = false;

        private GameObject interactGO;

        #endregion Variables

        #region IInteractable Interface
        private float distance = 2.0f;

        public float Distance => distance;

        public bool Interact(GameObject other)
        {
            float calcDistance = Vector3.Distance(other.transform.position, transform.position);

            if (calcDistance > distance)
            {
                return false;
            }

            if (isStartDialogue)
            {
                return false;
            }

            interactGO = other;

            DialogueManager.Instance.OnEndDialogue -= OnEndDialogue;
            DialogueManager.Instance.OnEndDialogue += OnEndDialogue;
            isStartDialogue = true;

            DialogueManager.Instance.StartDialogue(dialogue);

            return true;
        }

        public void StopInteract(GameObject other)
        {
            isStartDialogue = false;
        }

        private void OnEndDialogue()
        {
            StopInteract(interactGO);
        }

        #endregion IInteractable Interface
    }
}