using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityChanAdventure.FeelJoon
{
    public class QuestListView : MonoBehaviour
    {
        #region Variables
        public Image toggleElement;

        public Dictionary<QuestObject, GameObject> elementsByQuest = new Dictionary<QuestObject, GameObject>();
        private ToggleGroup toggleGroup;

        public QuestDisplayGroup displayGroup;

        #endregion Variables

        #region Unity Methods
        void Awake()
        {
            toggleGroup = GetComponent<ToggleGroup>();
        }

        void Start()
        {
            for (int i = 0; i < QuestManager.Instance.acceptedQuestObjects.Count; i++)
            {
                var element = Instantiate(toggleElement, transform);
            }
        }

        #endregion Unity Methods

        #region Helper Methods
        public void AddElement(QuestObject questObject)
        {
            var element = Instantiate(toggleElement, transform);
            Text text = element.GetComponentInChildren<Text>();
            text.text = questObject.data.title;

            QuestToggleContext questToggleContext = element.GetComponent<QuestToggleContext>();

            questToggleContext.questDisplayGroup = displayGroup;
            questToggleContext.questObject = questObject;

            var toggle = element.GetComponent<Toggle>();
            toggle.group = toggleGroup;
            toggle.onValueChanged.AddListener(questToggleContext.OnClick);

            elementsByQuest.Add(questObject, element.gameObject);
        }

        public void RemoveElement(QuestObject questObject)
        {
            Destroy(elementsByQuest[questObject]);
            elementsByQuest.Remove(questObject);
        }

        public void OnClickQuestTab(bool isOn)
        {
            displayGroup.gameObject.SetActive(false);

            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");
        }

        #endregion Helper Methods
    }
}