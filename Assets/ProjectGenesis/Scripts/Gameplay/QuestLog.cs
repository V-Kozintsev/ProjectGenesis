using System.Collections.Generic;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    public sealed class QuestLog : MonoBehaviour
    {
        private readonly Dictionary<string, QuestState> questStates = new();

        public QuestState GetQuestState(string questId)
        {
            if (string.IsNullOrWhiteSpace(questId))
            {
                return QuestState.NotStarted;
            }

            return questStates.TryGetValue(questId, out QuestState state) ? state : QuestState.NotStarted;
        }

        public bool TryAcceptQuest(string questId)
        {
            if (string.IsNullOrWhiteSpace(questId) || GetQuestState(questId) != QuestState.NotStarted)
            {
                return false;
            }

            questStates[questId] = QuestState.Active;
            return true;
        }
    }
}
