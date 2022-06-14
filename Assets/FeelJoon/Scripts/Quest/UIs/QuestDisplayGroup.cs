using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityChanAdventure.FeelJoon
{
    public class QuestDisplayGroup : MonoBehaviour
    {
        #region Variables
        public Text questName;
        public Text description;
        public Text taskDescription;
        public Text rewardDescription;

        #endregion Variables

        #region Helper Methods
        public void Show(QuestObject questObject, bool isOn)
        {
            if (isOn)
            {
                questName.text = questObject.data.title;
                description.text = questObject.data.description;
                taskDescription.text = questObject.data.content +
                        " : " + $"{questObject.data.completedCount} / {questObject.data.count}";
                rewardDescription.text = $"EXP : {questObject.data.rewardExp}\n" +
                    $"Gold : {questObject.data.rewardGold}\n" +
                    $"Item : {questObject.SearchRewardItemObjectWithID(questObject.data.rewardItemId).data.name}";

            }
        }
    
        #endregion Helper Methods
    }
}