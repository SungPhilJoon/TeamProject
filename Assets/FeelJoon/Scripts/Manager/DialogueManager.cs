using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UnityChanAdventure.FeelJoon
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        #region Variables
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dialogueText;

        public Animator animator;

        private Queue<string> sentences = new Queue<string>();

        public event Action OnStartDialogue;
        public event Action OnEndDialogue;

        [SerializeField] private Text btnContext;

        private readonly int hashIsOpen = Animator.StringToHash("IsOpen");

        private readonly string nextBtnContext = "다음";
        private readonly string completedBtnContext = "확인";

        #endregion Variables

        #region Helper Methods
        public void StartDialogue(Dialogue dialogue)
        {
            OnStartDialogue?.Invoke();

            animator?.SetBool(hashIsOpen, true);

            nameText.text = dialogue.name;

            sentences.Clear();

            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            btnContext.text = nextBtnContext;

            DisplayNextSentence();
        }

        public void DisplayNextSentence()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            if (sentences.Count.Equals(0))
            {
                EndDialogue();
                return;
            }

            if (sentences.Count.Equals(1))
            {
                btnContext.text = completedBtnContext;
            }

            string sentence = sentences.Dequeue();
            StopAllCoroutines();

            StartCoroutine(TypeSentence(sentence));
        }

        private IEnumerator TypeSentence(string sentence)
        {
            dialogueText.text = string.Empty;

            yield return new WaitForSeconds(0.25f);

            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return null;
            }
        }

        private void EndDialogue()
        {
            animator?.SetBool(hashIsOpen, false);

            OnEndDialogue?.Invoke();
        }

        public void CloseDialogue()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            EndDialogue();
        }

        #endregion Helper Methods
    }
}