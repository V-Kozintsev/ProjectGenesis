using System;
using System.Collections.Generic;
using ProjectGenesis.Gameplay;

namespace ProjectGenesis.Saving
{
    [Serializable]
    public sealed class PlayerProfileData
    {
        public const int CurrentVersion = 2;

        public int Version = CurrentVersion;
        public string CharacterName;
        public string RaceId;
        public string ClassId;
        public bool HasPosition;
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public int Level = 1;
        public int CurrentExperience;
        public List<string> InventoryItemIds = new();
        public string MainHandItemId;
        public List<QuestProgressData> Quests = new();
    }
}
