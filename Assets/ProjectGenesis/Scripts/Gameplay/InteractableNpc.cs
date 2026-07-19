using ProjectGenesis.UI;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    public sealed class InteractableNpc : MonoBehaviour
    {
        [SerializeField] private string displayName = "Староста деревни";
        [SerializeField] private string greetingText = "Путник, волков за северными воротами стало слишком много. Принеси мне пять волчьих хвостов.";
        [SerializeField] private string activeQuestText = "Хвост выпадает не с каждого волка. Продолжай охоту за северными воротами.";
        [SerializeField] private string readyQuestText = "Пяти хвостов достаточно. Теперь мы знаем, насколько велика стая.";
        [SerializeField] private string completedQuestText = "Спасибо за помощь. Северная дорога теперь безопаснее.";
        [SerializeField] private string questId = "wolves-near-the-road";
        [SerializeField] private string questTitle = "Волчьи трофеи";
        [SerializeField] private string questDescription = "Стая у северной дороги угрожает деревне. Принесите старосте доказательства охоты.";
        [SerializeField] private string questObjectiveText = "Волчьи хвосты";
        [SerializeField] private string questTargetId = "wolf_tail";
        [SerializeField, Min(1)] private int requiredObjectiveCount = 5;
        [SerializeField, Min(0)] private int rewardExperience = 80;
        [SerializeField] private GameObject selectionRing;

        public string DisplayName => displayName;
        public string GreetingText => greetingText;
        public string ActiveQuestText => activeQuestText;
        public string ReadyQuestText => readyQuestText;
        public string CompletedQuestText => completedQuestText;
        public string QuestId => questId;
        public string QuestTitle => questTitle;
        public string QuestDescription => questDescription;
        public string QuestObjectiveText => questObjectiveText;
        public string QuestTargetId => questTargetId;
        public int RequiredObjectiveCount => requiredObjectiveCount;
        public int RewardExperience => rewardExperience;

        public void ConfigureQuest(
            string id,
            string title,
            string description,
            string objectiveText,
            string targetId,
            int requiredCount,
            int experienceReward)
        {
            questId = id;
            questTitle = title;
            questDescription = description;
            questObjectiveText = objectiveText;
            questTargetId = targetId;
            requiredObjectiveCount = Mathf.Max(1, requiredCount);
            rewardExperience = Mathf.Max(0, experienceReward);
        }

        public void SetSelectionRing(GameObject ring)
        {
            selectionRing = ring;
            SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            if (selectionRing != null)
            {
                selectionRing.SetActive(selected);
            }
        }

        public void Interact(QuestLog questLog, DialogueWindow dialogueWindow, System.Func<InteractableNpc, bool> canInteract = null)
        {
            if (dialogueWindow != null)
            {
                dialogueWindow.Show(this, questLog, canInteract);
            }
        }
    }
}
