using ProjectGenesis.UI;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    public sealed class InteractableNpc : MonoBehaviour
    {
        [SerializeField] private string displayName = "Староста деревни";
        [SerializeField] private string greetingText = "Путник, северная дорога стала опасной. Волки подходят всё ближе к деревне. Сможешь помочь нам?";
        [SerializeField] private string activeQuestText = "Спасибо. Пока просто запомни поручение: волков и бой мы добавим в следующем спринте.";
        [SerializeField] private string questId = "wolves-near-the-road";
        [SerializeField] private string questTitle = "Волки у дороги";
        [SerializeField] private string questObjectiveText = "Будущая цель: победить 3 волков у старой северной дороги.";
        [SerializeField] private GameObject selectionRing;

        public string DisplayName => displayName;
        public string GreetingText => greetingText;
        public string ActiveQuestText => activeQuestText;
        public string QuestId => questId;
        public string QuestTitle => questTitle;
        public string QuestObjectiveText => questObjectiveText;

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
