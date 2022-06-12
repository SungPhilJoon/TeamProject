using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    [CreateAssetMenu(fileName ="New Quest Database", menuName ="Quest System/Quests/New Quest Database")]
    public class QuestDatabase : ScriptableObject
    {
        public QuestObject[] questObjects;

        private void OnValidate()
        {
            for (int i = 0; i < questObjects.Length; i++)
            {
                questObjects[i].data.id = i;
            }
        }
    }
}