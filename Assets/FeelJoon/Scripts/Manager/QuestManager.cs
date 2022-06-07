using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETeam.FeelJoon
{
    public class QuestManager : Singleton<QuestManager>
    {
        #region Variables
        public QuestDatabase questDatabase;
        public QuestDatabase acceptedQuestDatabase;

        public event Action<QuestObject> OnAcceptedQuest;
        public event Action<QuestObject> OnCompletedQuest;

        public List<QuestObject> acceptedQuestObjects = new List<QuestObject>();

        [Header("Quest Tracker")]
        public Image questTrackerUI;
        public Transform questTracker;

        #endregion Variables

        #region Unity Methods
        void OnDestroy()
        {
            
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