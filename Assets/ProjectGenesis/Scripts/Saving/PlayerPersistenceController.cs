using System;
using System.Collections.Generic;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEngine;
using UnityEngine.AI;

namespace ProjectGenesis.Saving
{
    [DefaultExecutionOrder(250)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerProgression), typeof(PlayerInventory), typeof(PlayerEquipment))]
    [RequireComponent(typeof(PlayerIdentity))]
    public sealed class PlayerPersistenceController : MonoBehaviour
    {
        [SerializeField, Min(0.5f)] private float saveInterval = 2f;
        [SerializeField] private ItemDefinition[] itemCatalog;
        [SerializeField] private CharacterRaceDefinition[] raceCatalog;
        [SerializeField] private CharacterClassDefinition[] classCatalog;

        private PlayerIdentity identity;
        private PlayerProgression progression;
        private PlayerInventory inventory;
        private PlayerEquipment equipment;
        private QuestLog questLog;
        private NavMeshAgent agent;
        private IPlayerPersistence persistence;
        private float nextSaveTime;
        private bool isInitialized;
        private bool hasCreatedCharacter;

        public event Action<PlayerPersistenceController> Initialized;

        public IReadOnlyList<ItemDefinition> ItemCatalog => itemCatalog;
        public IReadOnlyList<CharacterRaceDefinition> RaceCatalog => raceCatalog;
        public IReadOnlyList<CharacterClassDefinition> ClassCatalog => classCatalog;
        public bool IsInitialized => isInitialized;
        public bool HasCreatedCharacter => hasCreatedCharacter;

        public void Configure(
            ItemDefinition[] availableItems,
            CharacterRaceDefinition[] availableRaces,
            CharacterClassDefinition[] availableClasses)
        {
            itemCatalog = availableItems;
            raceCatalog = availableRaces;
            classCatalog = availableClasses;
        }

        private void Awake()
        {
            identity = GetComponent<PlayerIdentity>();
            progression = GetComponent<PlayerProgression>();
            inventory = GetComponent<PlayerInventory>();
            equipment = GetComponent<PlayerEquipment>();
            questLog = GetComponent<QuestLog>();
            agent = GetComponent<NavMeshAgent>();
            persistence = new LocalJsonPlayerPersistence();
        }

        private void Start()
        {
            if (persistence.TryLoad(out PlayerProfileData profile))
            {
                hasCreatedCharacter = ResolveHasCreatedCharacter(profile);
                Restore(profile);
            }

            isInitialized = true;
            nextSaveTime = Time.unscaledTime + saveInterval;
            Initialized?.Invoke(this);
        }

        private void Update()
        {
            if (!isInitialized || Time.unscaledTime < nextSaveTime)
            {
                return;
            }

            SaveNow();
            nextSaveTime = Time.unscaledTime + saveInterval;
        }

        private void OnApplicationQuit()
        {
            SaveNow();
        }

        private void OnDisable()
        {
            SaveNow();
        }

        public void SaveNow()
        {
            if (!isInitialized || persistence == null)
            {
                return;
            }

            persistence.Save(Capture());
        }

        private PlayerProfileData Capture()
        {
            PlayerProfileData profile = new()
            {
                HasCreatedCharacter = hasCreatedCharacter,
                CharacterName = identity.CharacterName,
                RaceId = identity.Race != null ? identity.Race.RaceId : string.Empty,
                ClassId = identity.CharacterClass != null
                    ? identity.CharacterClass.ClassId
                    : string.Empty,
                Level = progression.Level,
                CurrentExperience = progression.CurrentExperience,
                MainHandInstanceId = equipment.MainHand != null
                    ? equipment.MainHand.InstanceId
                    : string.Empty,
                BodyInstanceId = equipment.Body != null
                    ? equipment.Body.InstanceId
                    : string.Empty,
                MainHandItem = CaptureEquippedItem(equipment.MainHand),
                BodyItem = CaptureEquippedItem(equipment.Body),
                Quests = questLog != null ? questLog.CaptureState() : new List<QuestProgressData>()
            };

            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                Vector3 position = transform.position;
                profile.HasPosition = true;
                profile.PositionX = position.x;
                profile.PositionY = position.y;
                profile.PositionZ = position.z;
            }

            for (int slotIndex = 0; slotIndex < inventory.Capacity; slotIndex++)
            {
                ItemInstance item = inventory.GetItemAt(slotIndex);
                if (item != null && item.IsValid)
                {
                    profile.InventoryItems.Add(new ItemInstanceData
                    {
                        InstanceId = item.InstanceId,
                        ItemId = item.ItemId,
                        SlotIndex = slotIndex
                    });
                }
            }

            return profile;
        }

        public bool TryCreateCharacter(string requestedName)
        {
            if (!isInitialized || hasCreatedCharacter ||
                identity.Race == null || !identity.Race.IsValid ||
                identity.CharacterClass == null || !identity.CharacterClass.IsValid)
            {
                return false;
            }

            if (!identity.TrySetCharacterName(requestedName))
            {
                return false;
            }

            hasCreatedCharacter = true;
            SaveNow();
            return true;
        }

        public static bool ResolveHasCreatedCharacter(PlayerProfileData profile)
        {
            return profile != null &&
                   (profile.Version < 3 || profile.HasCreatedCharacter);
        }

        private void Restore(PlayerProfileData profile)
        {
            identity.RestoreIdentity(
                profile.CharacterName,
                FindRace(profile.RaceId),
                FindClass(profile.ClassId));

            List<ItemInstance> restoredItems = BuildInventorySlots(
                profile,
                FindItem,
                inventory.Capacity);

            inventory.RestoreSlots(restoredItems);
            progression.RestoreState(profile.Level, profile.CurrentExperience);
            questLog?.RestoreState(profile.Quests);
            if (profile.Version >= 7)
            {
                equipment.RestoreMainHand(BuildEquippedInstance(
                    profile.MainHandItem,
                    FindItem,
                    ItemType.Weapon));
                equipment.RestoreBody(BuildEquippedInstance(
                    profile.BodyItem,
                    FindItem,
                    ItemType.Armor));
            }
            else
            {
                equipment.RestoreMainHand(ResolveMainHandInstanceId(profile, restoredItems));
                equipment.RestoreBody(ResolveBodyInstanceId(profile, restoredItems));
            }

            if (profile.HasPosition)
            {
                RestorePosition(new Vector3(profile.PositionX, profile.PositionY, profile.PositionZ));
            }
        }

        private void RestorePosition(Vector3 savedPosition)
        {
            if (!NavMesh.SamplePosition(savedPosition, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                return;
            }

            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                agent.Warp(hit.position);
                agent.ResetPath();
            }
            else
            {
                transform.position = hit.position;
            }
        }

        private ItemDefinition FindItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId) || itemCatalog == null)
            {
                return null;
            }

            foreach (ItemDefinition item in itemCatalog)
            {
                if (item != null && item.ItemId == itemId)
                {
                    return item;
                }
            }

            return null;
        }

        public static List<ItemInstance> BuildInventoryInstances(
            PlayerProfileData profile,
            Func<string, ItemDefinition> itemResolver)
        {
            List<ItemInstance> restoredItems = new();
            if (profile == null || itemResolver == null)
            {
                return restoredItems;
            }

            bool hasVersionFourItems = profile.InventoryItems != null &&
                                       profile.InventoryItems.Count > 0;
            if (hasVersionFourItems)
            {
                foreach (ItemInstanceData savedItem in profile.InventoryItems)
                {
                    if (savedItem == null ||
                        string.IsNullOrWhiteSpace(savedItem.InstanceId) ||
                        restoredItems.Exists(item =>
                            item.InstanceId == savedItem.InstanceId))
                    {
                        continue;
                    }

                    ItemInstance instance = ItemInstance.Create(
                        itemResolver(savedItem.ItemId),
                        savedItem.InstanceId);
                    if (instance != null)
                    {
                        restoredItems.Add(instance);
                    }
                }

                return restoredItems;
            }

            if (profile.InventoryItemIds == null)
            {
                return restoredItems;
            }

            foreach (string itemId in profile.InventoryItemIds)
            {
                ItemInstance instance = ItemInstance.Create(itemResolver(itemId));
                if (instance != null)
                {
                    restoredItems.Add(instance);
                }
            }

            return restoredItems;
        }

        public static List<ItemInstance> BuildInventorySlots(
            PlayerProfileData profile,
            Func<string, ItemDefinition> itemResolver,
            int capacity)
        {
            int boundedCapacity = Mathf.Max(1, capacity);
            List<ItemInstance> restoredSlots = new(boundedCapacity);
            for (int index = 0; index < boundedCapacity; index++)
            {
                restoredSlots.Add(null);
            }

            List<ItemInstance> restoredItems = BuildInventoryInstances(profile, itemResolver);
            if (profile != null && profile.Version >= 5 && profile.InventoryItems != null)
            {
                Dictionary<string, ItemInstance> instancesById = new();
                foreach (ItemInstance item in restoredItems)
                {
                    if (item != null)
                    {
                        instancesById[item.InstanceId] = item;
                    }
                }

                foreach (ItemInstanceData savedItem in profile.InventoryItems)
                {
                    if (savedItem == null || savedItem.SlotIndex < 0 ||
                        savedItem.SlotIndex >= boundedCapacity ||
                        restoredSlots[savedItem.SlotIndex] != null ||
                        !instancesById.TryGetValue(savedItem.InstanceId, out ItemInstance item))
                    {
                        continue;
                    }

                    restoredSlots[savedItem.SlotIndex] = item;
                }

                return restoredSlots;
            }

            int sequentialCount = Mathf.Min(boundedCapacity, restoredItems.Count);
            for (int index = 0; index < sequentialCount; index++)
            {
                restoredSlots[index] = restoredItems[index];
            }

            return restoredSlots;
        }

        public static string ResolveMainHandInstanceId(
            PlayerProfileData profile,
            IReadOnlyList<ItemInstance> restoredItems)
        {
            if (profile == null || restoredItems == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(profile.MainHandInstanceId))
            {
                foreach (ItemInstance item in restoredItems)
                {
                    if (item != null && item.InstanceId == profile.MainHandInstanceId)
                    {
                        return item.InstanceId;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(profile.MainHandItemId))
            {
                foreach (ItemInstance item in restoredItems)
                {
                    if (item != null && item.ItemId == profile.MainHandItemId)
                    {
                        return item.InstanceId;
                    }
                }
            }

            return string.Empty;
        }

        public static string ResolveBodyInstanceId(
            PlayerProfileData profile,
            IReadOnlyList<ItemInstance> restoredItems)
        {
            if (profile == null || restoredItems == null ||
                string.IsNullOrWhiteSpace(profile.BodyInstanceId))
            {
                return string.Empty;
            }

            foreach (ItemInstance item in restoredItems)
            {
                if (item != null && item.InstanceId == profile.BodyInstanceId &&
                    item.ItemType == ItemType.Armor)
                {
                    return item.InstanceId;
                }
            }

            return string.Empty;
        }

        public static ItemInstance BuildEquippedInstance(
            ItemInstanceData savedItem,
            Func<string, ItemDefinition> itemResolver,
            ItemType expectedType)
        {
            if (savedItem == null || itemResolver == null ||
                string.IsNullOrWhiteSpace(savedItem.InstanceId))
            {
                return null;
            }

            ItemDefinition definition = itemResolver(savedItem.ItemId);
            if (definition == null || definition.ItemType != expectedType)
            {
                return null;
            }

            return ItemInstance.Create(definition, savedItem.InstanceId);
        }

        private static ItemInstanceData CaptureEquippedItem(ItemInstance item)
        {
            return item != null && item.IsValid
                ? new ItemInstanceData
                {
                    InstanceId = item.InstanceId,
                    ItemId = item.ItemId,
                    SlotIndex = -1
                }
                : null;
        }

        private CharacterRaceDefinition FindRace(string raceId)
        {
            if (string.IsNullOrWhiteSpace(raceId) || raceCatalog == null)
            {
                return null;
            }

            foreach (CharacterRaceDefinition race in raceCatalog)
            {
                if (race != null && race.RaceId == raceId)
                {
                    return race;
                }
            }

            return null;
        }

        private CharacterClassDefinition FindClass(string classId)
        {
            if (string.IsNullOrWhiteSpace(classId) || classCatalog == null)
            {
                return null;
            }

            foreach (CharacterClassDefinition characterClass in classCatalog)
            {
                if (characterClass != null && characterClass.ClassId == classId)
                {
                    return characterClass;
                }
            }

            return null;
        }
    }
}
