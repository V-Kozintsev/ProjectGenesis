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

        public IReadOnlyList<CharacterRaceDefinition> RaceCatalog => raceCatalog;
        public IReadOnlyList<CharacterClassDefinition> ClassCatalog => classCatalog;

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
                Restore(profile);
            }

            isInitialized = true;
            nextSaveTime = Time.unscaledTime + saveInterval;
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

        private void SaveNow()
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
                CharacterName = identity.CharacterName,
                RaceId = identity.Race != null ? identity.Race.RaceId : string.Empty,
                ClassId = identity.CharacterClass != null
                    ? identity.CharacterClass.ClassId
                    : string.Empty,
                Level = progression.Level,
                CurrentExperience = progression.CurrentExperience,
                MainHandItemId = equipment.MainHand != null ? equipment.MainHand.ItemId : string.Empty,
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

            foreach (ItemDefinition item in inventory.Items)
            {
                if (item != null)
                {
                    profile.InventoryItemIds.Add(item.ItemId);
                }
            }

            return profile;
        }

        private void Restore(PlayerProfileData profile)
        {
            identity.RestoreIdentity(
                profile.CharacterName,
                FindRace(profile.RaceId),
                FindClass(profile.ClassId));

            List<ItemDefinition> restoredItems = new();
            if (profile.InventoryItemIds != null)
            {
                foreach (string itemId in profile.InventoryItemIds)
                {
                    ItemDefinition item = FindItem(itemId);
                    if (item != null)
                    {
                        restoredItems.Add(item);
                    }
                }
            }

            inventory.RestoreItems(restoredItems);
            progression.RestoreState(profile.Level, profile.CurrentExperience);
            questLog?.RestoreState(profile.Quests);
            equipment.RestoreMainHand(FindItem(profile.MainHandItemId));

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
