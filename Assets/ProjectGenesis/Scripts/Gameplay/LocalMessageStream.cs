using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    public enum LocalMessageCategory
    {
        System,
        Loot,
        Combat,
        LocalChat,
        Announcement
    }

    [Flags]
    public enum LocalMessageFilter
    {
        None = 0,
        System = 1 << 0,
        Loot = 1 << 1,
        Combat = 1 << 2,
        LocalChat = 1 << 3,
        Announcement = 1 << 4,
        All = System | Loot | Combat | LocalChat | Announcement
    }

    public readonly struct LocalMessageEntry
    {
        public LocalMessageEntry(long sequence, LocalMessageCategory category, string text)
        {
            Sequence = sequence;
            Category = category;
            Text = text;
        }

        public long Sequence { get; }
        public LocalMessageCategory Category { get; }
        public string Text { get; }
    }

    [DisallowMultipleComponent]
    public sealed class LocalMessageStream : MonoBehaviour
    {
        [SerializeField, Range(10, 200)] private int historyCapacity = 50;

        private readonly List<LocalMessageEntry> entries = new();
        private long nextSequence = 1;

        public event Action<LocalMessageEntry> MessagePublished;
        public event Action Cleared;

        public IReadOnlyList<LocalMessageEntry> Entries => entries;
        public int HistoryCapacity => historyCapacity;

        public bool Publish(LocalMessageCategory category, string text)
        {
            string normalizedText = text?.Trim();
            if (string.IsNullOrEmpty(normalizedText))
            {
                return false;
            }

            LocalMessageEntry entry = new(nextSequence++, category, normalizedText);
            entries.Add(entry);
            TrimHistory();
            MessagePublished?.Invoke(entry);
            return true;
        }

        public void Clear()
        {
            if (entries.Count == 0)
            {
                return;
            }

            entries.Clear();
            Cleared?.Invoke();
        }

        public void ConfigureHistoryCapacity(int capacity)
        {
            historyCapacity = Mathf.Clamp(capacity, 10, 200);
            TrimHistory();
        }

        public static bool Matches(LocalMessageCategory category, LocalMessageFilter filter)
        {
            LocalMessageFilter categoryFilter = category switch
            {
                LocalMessageCategory.System => LocalMessageFilter.System,
                LocalMessageCategory.Loot => LocalMessageFilter.Loot,
                LocalMessageCategory.Combat => LocalMessageFilter.Combat,
                LocalMessageCategory.LocalChat => LocalMessageFilter.LocalChat,
                LocalMessageCategory.Announcement => LocalMessageFilter.Announcement,
                _ => LocalMessageFilter.None
            };

            return (filter & categoryFilter) != 0;
        }

        private void OnValidate()
        {
            historyCapacity = Mathf.Clamp(historyCapacity, 10, 200);
            TrimHistory();
        }

        private void TrimHistory()
        {
            int overflow = entries.Count - historyCapacity;
            if (overflow > 0)
            {
                entries.RemoveRange(0, overflow);
            }
        }
    }
}
