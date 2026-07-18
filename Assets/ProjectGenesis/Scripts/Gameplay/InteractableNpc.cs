using ProjectGenesis.UI;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    public sealed class InteractableNpc : MonoBehaviour
    {
        [SerializeField] private string displayName = "Староста деревни";
        [SerializeField] private string greetingText = "Путник, северная дорога стала опасной. Волки подходят всё ближе к деревне. Сможешь помочь нам?";
        [SerializeField] private string activeQuestText = "Волк всё ещё бродит за северными воротами. Возвращайся, когда справишься с ним.";
        [SerializeField] private string readyQuestText = "Ты справился. Деревня снова может дышать спокойнее.";
        [SerializeField] private string completedQuestText = "Спасибо за помощь. Северная дорога теперь безопаснее.";
        [SerializeField] private string questId = "wolves-near-the-road";
        [SerializeField] private string questTitle = "Волки у дороги";
        [SerializeField] private string questObjectiveText = "Победить молодого волка за северными воротами.";
        [SerializeField] private string questTargetId = "wolf";
        [SerializeField, Min(1)] private int requiredKillCount = 1;
        [SerializeField, Min(0)] private int rewardExperience = 80;
        [SerializeField] private GameObject selectionRing;

        public string DisplayName => displayName;
        public string GreetingText => greetingText;
        public string ActiveQuestText => activeQuestText;
        public string ReadyQuestText => readyQuestText;
        public string CompletedQuestText => completedQuestText;
        public string QuestId => questId;
        public string QuestTitle => questTitle;
        public string QuestObjectiveText => questObjectiveText;
        public string QuestTargetId => questTargetId;
        public int RequiredKillCount => requiredKillCount;
        public int RewardExperience => rewardExperience;

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
