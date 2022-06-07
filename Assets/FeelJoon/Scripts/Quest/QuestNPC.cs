using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class QuestNPC : MonoBehaviour, IInteractable
    {
        #region Variables
        public QuestObject questObject;

        public Dialogue readyDialogue;
        public Dialogue acceptedDialogue;
        public Dialogue completedDialogue;

        private bool isStartQuestDialogue = false;
        private GameObject interactGO = null;

        #endregion Variables

        #region Unity Methods
        void Start()
        {
            QuestManager.Instance.OnCompletedQuest -= OnCompletedQuest;
            QuestManager.Instance.OnCompletedQuest += OnCompletedQuest;
        }

        #endregion Unity Methods

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

            if (isStartQuestDialogue)
            {
                return false;
            }

            this.interactGO = other;

            DialogueManager.Instance.OnEndDialogue -= OnEndDialogue;
            DialogueManager.Instance.OnEndDialogue += OnEndDialogue;
            isStartQuestDialogue = true;

            if (questObject.status == QuestStatus.None)
            {
                readyDialogue.sentences[0] = questObject.data.description;
                DialogueManager.Instance.StartDialogue(readyDialogue);
                questObject.status = QuestStatus.Accepted;

                QuestManager.Instance.acceptedQuestObjects.Add(questObject);

                questObject.tracker = Instantiate(QuestManager.Instance.questTrackerUI, QuestManager.Instance.questTracker);

                questObject.tracker.transform.GetChild(0).GetComponent<Text>().text = questObject.data.title;
                questObject.tracker.transform.GetChild(1).GetComponent<Text>().text = questObject.data.content +
                    " : " + $"{questObject.data.completedCount} / {questObject.data.count}";

                QuestManager.Instance.OnAcceptedQuest -= OnAcceptedQuest;
                QuestManager.Instance.OnAcceptedQuest += OnAcceptedQuest;
            }
            else if (questObject.status == QuestStatus.Accepted)
            {
                DialogueManager.Instance.StartDialogue(acceptedDialogue);
            }
            else if (questObject.status == QuestStatus.Completed)
            {
                DialogueManager.Instance.StartDialogue(completedDialogue);
                QuestManager.Instance.acceptedQuestObjects.Remove(questObject);
                Destroy(questObject.tracker.gameObject);

                // process reward
                if (other.TryGetComponent<MainPlayerController>(out MainPlayerController mainPlayerController))
                {
                    mainPlayerController.playerStats.AddExp(questObject.data.rewardExp);
                    mainPlayerController.gold += questObject.data.rewardGold;
                    mainPlayerController.inventory.AddItem
                        (new Item(questObject.SearchRewardItemObjectWithID(questObject.data.rewardItemId)), 1);
                }

                questObject.status = QuestStatus.Rewarded;
                this.questObject = QuestManager.Instance.questDatabase.questObjects[this.questObject.data.id + 1];
            }

            return true;
        }

        public void StopInteract(GameObject other)
        {
            isStartQuestDialogue = false;
        }

        #endregion IInteractable Interface

        #region Helper Methods
        private void OnEndDialogue()
        {
            StopInteract(interactGO);
        }

        private void OnCompletedQuest(QuestObject questObject)
        {
            if (questObject.data.id == this.questObject.data.id
                && questObject.status == QuestStatus.Completed)
            {
                // Process npc effect
            }
        }

        private void OnAcceptedQuest(QuestObject questObject)
        {
                questObject.tracker.transform.GetChild(1).GetComponent<Text>().text = questObject.data.content +
                    " : " + $"{questObject.data.completedCount} / {questObject.data.count}";
        }

        #endregion Helper Methods
    }
}