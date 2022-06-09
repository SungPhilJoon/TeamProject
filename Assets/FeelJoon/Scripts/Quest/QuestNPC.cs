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

        private Animator animator;

        private Vector3 originalRotation;

        #endregion Variables

        #region Unity Methods
        void Start()
        {
            QuestManager.Instance.OnCompletedQuest -= OnCompletedQuest;
            QuestManager.Instance.OnCompletedQuest += OnCompletedQuest;

            animator = GetComponentInChildren<Animator>();

            originalRotation = transform.rotation.eulerAngles;

            if (QuestManager.Instance.questDatabase.questObjects.Length.Equals(0))
            {
                return;
            }

            foreach (QuestObject questObject in QuestManager.Instance.questDatabase.questObjects)
            {
                if (questObject.status.Equals(QuestStatus.None) || 
                    questObject.status.Equals(QuestStatus.Accepted) || 
                    questObject.status.Equals(QuestStatus.Completed))
                {
                    this.questObject = questObject;

                    QuestManager.Instance.OnAcceptedQuest -= OnAcceptedQuest;
                    QuestManager.Instance.OnAcceptedQuest += OnAcceptedQuest;

                    break;
                }
            }
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

            transform.LookAt(interactGO.transform.position + Vector3.up * 0.5f);

            DialogueManager.Instance.OnEndDialogue -= OnEndDialogue;
            DialogueManager.Instance.OnEndDialogue += OnEndDialogue;
            isStartQuestDialogue = true;

            if (questObject.status == QuestStatus.None)
            {
                readyDialogue.sentences[0] = questObject.data.description;
                DialogueManager.Instance.StartDialogue(readyDialogue);
                // questObject.status = QuestStatus.Accepted;

                // QuestManager.Instance.acceptedQuestObjects.Add(questObject);

                animator?.SetBool("IsQuestNone", true);
            }
            else if (questObject.status == QuestStatus.Accepted)
            {
                DialogueManager.Instance.StartDialogue(acceptedDialogue);

                animator?.SetBool("IsQuestAccepted", true);
            }
            else if (questObject.status == QuestStatus.Completed)
            {
                DialogueManager.Instance.StartDialogue(completedDialogue);

                QuestRewardPopupUI questRewardPopupUI = QuestManager.Instance.questRewardPopupUI;

                questRewardPopupUI.gameObject.SetActive(true);

                questRewardPopupUI.expText.text = questObject.data.rewardExp.ToString("#,###");
                questRewardPopupUI.goldText.text = questObject.data.rewardGold.ToString("#,###");
                questRewardPopupUI.itemIcon.sprite = questObject.SearchRewardItemObjectWithID(questObject.data.rewardItemId).icon;
                //QuestManager.Instance.acceptedQuestObjects.Remove(questObject);
                //if (questObject.tracker != null)
                //{
                //    Destroy(questObject.tracker?.gameObject);
                //}

                //if (QuestManager.Instance.guideObject.activeSelf)
                //{
                //    QuestManager.Instance.guideObject.SetActive(false);
                //}

                animator?.SetBool("IsQuestCompleted", true);

                // process reward
                //if (other.TryGetComponent<MainPlayerController>(out MainPlayerController mainPlayerController))
                //{
                //    mainPlayerController.playerStats.AddExp(questObject.data.rewardExp);
                //    mainPlayerController.gold += questObject.data.rewardGold;
                //    mainPlayerController.inventory.AddItem
                //        (new Item(questObject.SearchRewardItemObjectWithID(questObject.data.rewardItemId)), 1);
                //}

                //questObject.status = QuestStatus.Rewarded;
                //QuestManager.Instance.rewardedQuestObjects.Add(questObject);
                //foreach (QuestObject questObject in QuestManager.Instance.questDatabase.questObjects)
                //{
                //    if (questObject.status.Equals(QuestStatus.Accepted))
                //    {
                //        this.questObject = questObject;
                //        break;
                //    }

                //    if (questObject.status.Equals(QuestStatus.None))
                //    {
                //        this.questObject = questObject;
                //        break;
                //    }
                //}
            }

            return true;
        }

        public void StopInteract(GameObject other)
        {
            isStartQuestDialogue = false;

            if (questObject.status.Equals(QuestStatus.None))
            {
                questObject.status = QuestStatus.Accepted;

                QuestManager.Instance.acceptedQuestObjects.Add(questObject);

                animator?.SetBool("IsQuestNone", false);

                QuestManager.Instance.guideObject.SetActive(true);

                questObject.tracker = Instantiate(QuestManager.Instance.questTrackerUI, QuestManager.Instance.questTracker);

                questObject.tracker.transform.GetChild(0).GetComponent<Text>().text = questObject.data.title;
                questObject.tracker.transform.GetChild(1).GetComponent<Text>().text = questObject.data.content +
                    " : " + $"{questObject.data.completedCount} / {questObject.data.count}";
            }
            else if (questObject.status.Equals(QuestStatus.Accepted))
            {
                animator?.SetBool("IsQuestAccepted", false);
            }
            else if (questObject.status.Equals(QuestStatus.Completed))
            {
                QuestManager.Instance.acceptedQuestObjects.Remove(questObject);
                if (questObject.tracker != null)
                {
                    Destroy(questObject.tracker?.gameObject);
                }

                if (QuestManager.Instance.guideObject.activeSelf)
                {
                    QuestManager.Instance.guideObject.SetActive(false);
                }

                animator?.SetBool("IsQuestCompleted", false);

                if (other.TryGetComponent<MainPlayerController>(out MainPlayerController mainPlayerController))
                {
                    mainPlayerController.playerStats.AddExp(questObject.data.rewardExp);
                    mainPlayerController.gold += questObject.data.rewardGold;
                    mainPlayerController.inventory.AddItem
                        (new Item(questObject.SearchRewardItemObjectWithID(questObject.data.rewardItemId)), 1);
                }

                questObject.status = QuestStatus.Rewarded;
                QuestManager.Instance.rewardedQuestObjects.Add(questObject);
                foreach (QuestObject questObject in QuestManager.Instance.questDatabase.questObjects)
                {
                    if (questObject.status.Equals(QuestStatus.Accepted))
                    {
                        this.questObject = questObject;
                        break;
                    }

                    if (questObject.status.Equals(QuestStatus.None))
                    {
                        this.questObject = questObject;
                        break;
                    }
                }

                QuestManager.Instance.questRewardPopupUI.gameObject.SetActive(false);
            }

            transform.rotation = Quaternion.Euler(originalRotation);
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