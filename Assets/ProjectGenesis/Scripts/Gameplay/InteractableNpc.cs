using System;
using System.Collections.Generic;
using ProjectGenesis.Data;
using ProjectGenesis.UI;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    public sealed class InteractableNpc : MonoBehaviour
    {
        [SerializeField] private string displayName = "Староста деревни";
        [SerializeField] private QuestDefinition questDefinition;
        [SerializeField] private QuestDefinition[] followUpQuestDefinitions = Array.Empty<QuestDefinition>();
        [SerializeField] private GameObject selectionRing;

        public string DisplayName => displayName;
        public QuestDefinition QuestDefinition => questDefinition;
        public string GreetingText => questDefinition != null ? questDefinition.OfferText : string.Empty;
        public string ActiveQuestText => questDefinition != null ? questDefinition.ActiveText : string.Empty;
        public string ReadyQuestText => questDefinition != null ? questDefinition.ReadyText : string.Empty;
        public string CompletedQuestText => questDefinition != null ? questDefinition.CompletedText : string.Empty;
        public string QuestId => questDefinition != null ? questDefinition.QuestId : string.Empty;
        public string QuestTitle => questDefinition != null ? questDefinition.DisplayName : string.Empty;
        public string QuestDescription => questDefinition != null ? questDefinition.Description : string.Empty;
        public string QuestObjectiveText => questDefinition != null && questDefinition.Objective != null
            ? questDefinition.Objective.DisplayText
            : string.Empty;
        public string QuestReadyObjectiveText => questDefinition != null && questDefinition.Objective != null
            ? questDefinition.Objective.ReadyText
            : string.Empty;
        public string QuestTargetId => questDefinition != null && questDefinition.Objective != null
            ? questDefinition.Objective.TargetId
            : string.Empty;
        public int RequiredObjectiveCount => questDefinition != null && questDefinition.Objective != null
            ? questDefinition.Objective.RequiredCount
            : 1;
        public int RewardExperience => questDefinition != null && questDefinition.Reward != null
            ? questDefinition.Reward.Experience
            : 0;

        public void ConfigureDisplayName(string npcDisplayName)
        {
            displayName = string.IsNullOrWhiteSpace(npcDisplayName)
                ? "NPC"
                : npcDisplayName.Trim();
        }

        public void ConfigureQuest(QuestDefinition definition)
        {
            ConfigureQuests(definition);
        }

        public IEnumerable<QuestDefinition> QuestDefinitions
        {
            get
            {
                if (questDefinition != null)
                {
                    yield return questDefinition;
                }

                if (followUpQuestDefinitions == null)
                {
                    yield break;
                }

                foreach (QuestDefinition definition in followUpQuestDefinitions)
                {
                    if (definition != null)
                    {
                        yield return definition;
                    }
                }
            }
        }

        public void ConfigureQuests(
            QuestDefinition primaryDefinition,
            params QuestDefinition[] followUpDefinitions)
        {
            questDefinition = primaryDefinition;
            followUpQuestDefinitions = followUpDefinitions ?? Array.Empty<QuestDefinition>();
        }

        public QuestDefinition ResolveQuestDefinition(QuestLog questLog)
        {
            QuestDefinition tracked = FindTrackedQuestDefinition(questLog);
            if (tracked != null)
            {
                return tracked;
            }

            QuestDefinition lastCompleted = null;
            foreach (QuestDefinition definition in QuestDefinitions)
            {
                QuestState state = questLog != null
                    ? questLog.GetQuestState(definition.QuestId)
                    : QuestState.NotStarted;
                if (state == QuestState.Completed)
                {
                    lastCompleted = definition;
                    continue;
                }

                if (state == QuestState.NotStarted && IsUnlocked(definition, questLog))
                {
                    return definition;
                }
            }

            return lastCompleted ?? questDefinition;
        }

        public QuestDefinition FindTrackedQuestDefinition(QuestLog questLog)
        {
            if (questLog == null)
            {
                return null;
            }

            foreach (QuestDefinition definition in QuestDefinitions)
            {
                QuestState state = questLog.GetQuestState(definition.QuestId);
                if (state == QuestState.Active || state == QuestState.ReadyToTurnIn)
                {
                    return definition;
                }
            }

            return null;
        }

        public QuestDefinition FindQuestDefinition(string questId)
        {
            if (string.IsNullOrWhiteSpace(questId))
            {
                return null;
            }

            foreach (QuestDefinition definition in QuestDefinitions)
            {
                if (definition.QuestId == questId)
                {
                    return definition;
                }
            }

            return null;
        }

        private static bool IsUnlocked(QuestDefinition definition, QuestLog questLog)
        {
            return string.IsNullOrWhiteSpace(definition.PrerequisiteQuestId) ||
                   (questLog != null &&
                    questLog.GetQuestState(definition.PrerequisiteQuestId) == QuestState.Completed);
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
