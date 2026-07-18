using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [Serializable]
    public sealed class QuestProgressData
    {
        public string QuestId;
        public string TargetId;
        public QuestState State;
        public int RequiredCount;
        public int CurrentCount;
        public int RewardExperience;

        public QuestProgressData Clone()
        {
            return new QuestProgressData
            {
                QuestId = QuestId,
                TargetId = TargetId,
                State = State,
                RequiredCount = RequiredCount,
                CurrentCount = CurrentCount,
                RewardExperience = RewardExperience
            };
        }
    }

    public sealed class QuestLog : MonoBehaviour
    {
        private readonly Dictionary<string, QuestProgressData> quests = new();
        private PlayerProgression progression;

        public event Action<QuestLog> Changed;

        private void Awake()
        {
            progression = GetComponent<PlayerProgression>();
        }

        public QuestState GetQuestState(string questId)
        {
            QuestProgressData progress = GetQuestProgress(questId);
            return progress != null ? progress.State : QuestState.NotStarted;
        }

        public QuestProgressData GetQuestProgress(string questId)
        {
            if (string.IsNullOrWhiteSpace(questId))
            {
                return null;
            }

            return quests.TryGetValue(questId, out QuestProgressData progress) ? progress : null;
        }

        public bool TryAcceptQuest(
            string questId,
            string targetId,
            int requiredCount,
            int rewardExperience)
        {
            if (string.IsNullOrWhiteSpace(questId) || GetQuestState(questId) != QuestState.NotStarted)
            {
                return false;
            }

            quests[questId] = new QuestProgressData
            {
                QuestId = questId,
                TargetId = targetId,
                State = QuestState.Active,
                RequiredCount = Mathf.Max(1, requiredCount),
                CurrentCount = 0,
                RewardExperience = Mathf.Max(0, rewardExperience)
            };
            Changed?.Invoke(this);
            return true;
        }

        public void ReportEnemyDefeated(string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
            {
                return;
            }

            bool changed = false;
            foreach (QuestProgressData progress in quests.Values)
            {
                if (progress.State != QuestState.Active || progress.TargetId != targetId)
                {
                    continue;
                }

                progress.CurrentCount = Mathf.Min(progress.RequiredCount, progress.CurrentCount + 1);
                if (progress.CurrentCount >= progress.RequiredCount)
                {
                    progress.State = QuestState.ReadyToTurnIn;
                }

                changed = true;
            }

            if (changed)
            {
                Changed?.Invoke(this);
            }
        }

        public bool TryTurnInQuest(string questId)
        {
            QuestProgressData progress = GetQuestProgress(questId);
            if (progress == null || progress.State != QuestState.ReadyToTurnIn)
            {
                return false;
            }

            progress.State = QuestState.Completed;
            progression?.AddExperience(progress.RewardExperience);
            Changed?.Invoke(this);
            return true;
        }

        public List<QuestProgressData> CaptureState()
        {
            List<QuestProgressData> result = new(quests.Count);
            foreach (QuestProgressData progress in quests.Values)
            {
                result.Add(progress.Clone());
            }

            return result;
        }

        public void RestoreState(IEnumerable<QuestProgressData> savedQuests)
        {
            quests.Clear();

            if (savedQuests != null)
            {
                foreach (QuestProgressData saved in savedQuests)
                {
                    if (saved == null || string.IsNullOrWhiteSpace(saved.QuestId))
                    {
                        continue;
                    }

                    QuestProgressData restored = saved.Clone();
                    restored.RequiredCount = Mathf.Max(1, restored.RequiredCount);
                    restored.CurrentCount = Mathf.Clamp(restored.CurrentCount, 0, restored.RequiredCount);
                    restored.RewardExperience = Mathf.Max(0, restored.RewardExperience);
                    quests[restored.QuestId] = restored;
                }
            }

            Changed?.Invoke(this);
        }
    }
}
