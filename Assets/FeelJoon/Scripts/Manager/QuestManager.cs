using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UnityChanAdventure.FeelJoon
{
    public class QuestManager : Singleton<QuestManager>
    {
        #region Variables
        [HideInInspector] public QuestDatabase questDatabase;
        [HideInInspector] public QuestDatabase acceptedQuestDatabase;
        [HideInInspector] public QuestDatabase rewardedQuestDatabase;

        public event Action<QuestObject> OnAcceptedQuest;
        public event Action<QuestObject> OnCompletedQuest;

        public Dictionary<QuestObject, GameObject> acceptedQuestObjects = new Dictionary<QuestObject, GameObject>();
        public Dictionary<QuestObject, GameObject> rewardedQuestObjects = new Dictionary<QuestObject, GameObject>();

        [Header("Quest Tracker")]
        public Image questTrackerUI;
        public Transform questTracker;

        [Header("Quest Reward Popup UI")]
        public QuestRewardPopupUI questRewardPopupUI;

        [Header("Quest Accept Guide Object")]
        public GameObject guideObject;

        [Header("Quest View")]
        public GameObject questView;
        public QuestListView acceptedQuestListView;
        public QuestListView rewardedQuestListView;

        #endregion Variables

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();

            // acceptedQuestObjects = acceptedQuestDatabase.questObjects.ToList<QuestObject>();
            // rewardedQuestObjects = rewardedQuestDatabase.questObjects.ToList<QuestObject>();

            for (int i = 0; i < acceptedQuestDatabase.questObjects.Length; i++)
            {
                acceptedQuestListView.AddElement(acceptedQuestDatabase.questObjects[i]);
            }

            for (int i = 0; i < rewardedQuestDatabase.questObjects.Length; i++)
            {
                rewardedQuestListView.AddElement(rewardedQuestDatabase.questObjects[i]);
            }
        }

        void Start()
        {
            foreach (QuestObject questObject in acceptedQuestListView.elementsByQuest.Keys)
            {
                questObject.tracker = Instantiate(QuestManager.Instance.questTrackerUI, QuestManager.Instance.questTracker);

                questObject.tracker.transform.GetChild(0).GetComponent<Text>().text = questObject.data.title;
                questObject.tracker.transform.GetChild(1).GetComponent<Text>().text = questObject.data.content +
                    " : " + $"{questObject.data.completedCount} / {questObject.data.count}";
            }

            questView.SetActive(false);
        }

        void OnApplicationQuit()
        {
            acceptedQuestDatabase.questObjects = acceptedQuestListView.elementsByQuest.Keys.ToArray();
            rewardedQuestDatabase.questObjects = rewardedQuestListView.elementsByQuest.Keys.ToArray();
        }

        #endregion Unity Methods

        #region Helper Methods
        public void ProcessQuest(QuestType type, int targetID)
        {
            foreach (QuestObject questObject in questDatabase.questObjects)
            {
                if (questObject.status == QuestStatus.Accepted && questObject.data.type == type
                    && questObject.data.targetID == targetID)
                {
                    questObject.data.completedCount++;
                    OnAcceptedQuest?.Invoke(questObject);

                    if (questObject.data.completedCount >= questObject.data.count)
                    {
                        questObject.status = QuestStatus.Completed;
                        OnCompletedQuest?.Invoke(questObject);
                    }
                }
            }
        }

        #endregion Helper Methods
    }
}