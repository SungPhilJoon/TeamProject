using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ETeam.FeelJoon
{
    public class QuestManager : Singleton<QuestManager>
    {
        #region Variables
        [HideInInspector] public QuestDatabase questDatabase;
        [HideInInspector] public QuestDatabase acceptedQuestDatabase;
        [HideInInspector] public QuestDatabase rewardedQuestDatabase;

        public event Action<QuestObject> OnAcceptedQuest;
        public event Action<QuestObject> OnCompletedQuest;

        public List<QuestObject> acceptedQuestObjects = new List<QuestObject>();
        public List<QuestObject> rewardedQuestObjects = new List<QuestObject>();

        [Header("Quest Tracker")]
        public Image questTrackerUI;
        public Transform questTracker;

        [Header("Quest Reward Popup UI")]
        public QuestRewardPopupUI questRewardPopupUI;

        [Header("Quest Accept Guide Object")]
        public GameObject guideObject;

        #endregion Variables

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();

            acceptedQuestObjects = acceptedQuestDatabase.questObjects.ToList<QuestObject>();
            rewardedQuestObjects = rewardedQuestDatabase.questObjects.ToList<QuestObject>();
        }

        void Start()
        {
            foreach (QuestObject questObject in acceptedQuestObjects)
            {
                questObject.tracker = Instantiate(QuestManager.Instance.questTrackerUI, QuestManager.Instance.questTracker);

                questObject.tracker.transform.GetChild(0).GetComponent<Text>().text = questObject.data.title;
                questObject.tracker.transform.GetChild(1).GetComponent<Text>().text = questObject.data.content +
                    " : " + $"{questObject.data.completedCount} / {questObject.data.count}";
            }
        }

        void OnApplicationQuit()
        {
            acceptedQuestDatabase.questObjects = acceptedQuestObjects.ToArray();
            rewardedQuestDatabase.questObjects = rewardedQuestObjects.ToArray();
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