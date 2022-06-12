using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public enum QuestType
    {
        DestroyEnemy,
        AcquireItem,
    }

    [Serializable]
    public class Quest
    {
        #region Variables
        public int id;

        public QuestType type;
        public int targetID;

        public int count;
        public int completedCount;

        public int rewardExp;
        public int rewardGold;
        public int rewardItemId;
        public ItemObjectDatabase rewardItemObjectDatabase;

        public string title;
        public string description;
        public string content;

        #endregion Variables
    }
}