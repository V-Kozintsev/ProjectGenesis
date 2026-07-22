using System;
using System.Collections.Generic;
using ProjectGenesis.Gameplay;

namespace ProjectGenesis.Saving
{
    [Serializable]
    public sealed class ItemInstanceData
    {
        public string InstanceId;
        public string ItemId;
        public int SlotIndex = -1;
    }

    [Serializable]
    public sealed class PlayerProfileData
    {
        public const int CurrentVersion = 6;

        public int Version = CurrentVersion;
        public bool HasCreatedCharacter;
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
        public List<ItemInstanceData> InventoryItems = new();
        public string MainHandInstanceId;
        public string BodyInstanceId;
        public List<QuestProgressData> Quests = new();
    }
}
