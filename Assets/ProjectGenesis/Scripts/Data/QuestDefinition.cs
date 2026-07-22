using System;
using UnityEngine;

namespace ProjectGenesis.Data
{
    public enum QuestObjectiveType
    {
        DefeatTarget
    }

    [Serializable]
    public sealed class QuestObjectiveDefinition
    {
        [SerializeField] private QuestObjectiveType objectiveType = QuestObjectiveType.DefeatTarget;
        [SerializeField] private string targetId = "enemy.target";
        [SerializeField] private string displayText = "Defeat targets";
        [SerializeField] private string readyText = "Return to the quest giver";
        [SerializeField, Min(1)] private int requiredCount = 1;

        public QuestObjectiveType ObjectiveType => objectiveType;
        public string TargetId => targetId;
        public string DisplayText => displayText;
        public string ReadyText => readyText;
        public int RequiredCount => requiredCount;

        public void Configure(
            QuestObjectiveType type,
            string objectiveTargetId,
            string objectiveDisplayText,
            string completedObjectiveText,
            int count)
        {
            objectiveType = type;
            targetId = objectiveTargetId?.Trim() ?? string.Empty;
            displayText = objectiveDisplayText?.Trim() ?? string.Empty;
            readyText = completedObjectiveText?.Trim() ?? string.Empty;
            requiredCount = Mathf.Max(1, count);
        }

        public bool TryValidate(out string error)
        {
            if (string.IsNullOrWhiteSpace(targetId))
            {
                error = "Objective target id is empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(displayText))
            {
                error = "Objective display text is empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(readyText))
            {
                error = "Objective ready text is empty.";
                return false;
            }

            if (requiredCount <= 0)
            {
                error = "Objective required count must be positive.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }

    [Serializable]
    public sealed class QuestRewardDefinition
    {
        [SerializeField, Min(0)] private int experience;

        public int Experience => experience;

        public void Configure(int experienceReward)
        {
            experience = Mathf.Max(0, experienceReward);
        }
    }

    [CreateAssetMenu(
        fileName = "SO_Quest_NewQuest",
        menuName = "Project Genesis/Quest Definition")]
    public sealed class QuestDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string questId = "quest.new";
        [SerializeField] private string displayName = "New Quest";
        [SerializeField, TextArea] private string description = "Quest description.";
        [SerializeField] private string prerequisiteQuestId;

        [Header("Dialogue")]
        [SerializeField, TextArea] private string offerText = "Quest offer.";
        [SerializeField, TextArea] private string activeText = "Quest is active.";
        [SerializeField, TextArea] private string readyText = "Quest is ready.";
        [SerializeField, TextArea] private string completedText = "Quest completed.";

        [Header("Objective And Reward")]
        [SerializeField] private QuestObjectiveDefinition objective = new();
        [SerializeField] private QuestRewardDefinition reward = new();

        public string QuestId => questId;
        public string DisplayName => displayName;
        public string Description => description;
        public string PrerequisiteQuestId => prerequisiteQuestId;
        public string OfferText => offerText;
        public string ActiveText => activeText;
        public string ReadyText => readyText;
        public string CompletedText => completedText;
        public QuestObjectiveDefinition Objective => objective;
        public QuestRewardDefinition Reward => reward;

        public bool IsValid => TryValidate(out _);

        public void Configure(
            string id,
            string questName,
            string questDescription,
            string questOfferText,
            string questActiveText,
            string questReadyText,
            string questCompletedText,
            QuestObjectiveType objectiveType,
            string targetId,
            string objectiveText,
            string objectiveReadyText,
            int requiredCount,
            int rewardExperience,
            string requiredQuestId = "")
        {
            questId = id?.Trim() ?? string.Empty;
            displayName = questName?.Trim() ?? string.Empty;
            description = questDescription?.Trim() ?? string.Empty;
            prerequisiteQuestId = requiredQuestId?.Trim() ?? string.Empty;
            offerText = questOfferText?.Trim() ?? string.Empty;
            activeText = questActiveText?.Trim() ?? string.Empty;
            readyText = questReadyText?.Trim() ?? string.Empty;
            completedText = questCompletedText?.Trim() ?? string.Empty;
            objective ??= new QuestObjectiveDefinition();
            objective.Configure(
                objectiveType,
                targetId,
                objectiveText,
                objectiveReadyText,
                requiredCount);
            reward ??= new QuestRewardDefinition();
            reward.Configure(rewardExperience);
        }

        public bool TryValidate(out string error)
        {
            if (string.IsNullOrWhiteSpace(questId))
            {
                error = "Quest id is empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(displayName) ||
                string.IsNullOrWhiteSpace(description))
            {
                error = "Quest name or description is empty.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(offerText) ||
                string.IsNullOrWhiteSpace(activeText) ||
                string.IsNullOrWhiteSpace(readyText) ||
                string.IsNullOrWhiteSpace(completedText))
            {
                error = "One or more quest dialogue texts are empty.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(prerequisiteQuestId) &&
                prerequisiteQuestId == questId)
            {
                error = "Quest cannot require itself.";
                return false;
            }

            if (objective == null)
            {
                error = "Quest objective is missing.";
                return false;
            }

            if (!objective.TryValidate(out error))
            {
                return false;
            }

            if (reward == null)
            {
                error = "Quest reward is missing.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
