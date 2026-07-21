using ProjectGenesis.Core;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using ProjectGenesis.Saving;
using ProjectGenesis.UI;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectGenesis.Tools.Editor
{
    public static class Sprint002SceneBuilder
    {
        private const string ScenePath = "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath = "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string WolfPrefabPath = "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_Wolf.prefab";
        private const string BoarPrefabPath = "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_Boar.prefab";
        private const string GroundMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Starter_Ground.mat";
        private const string PlayerMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Player_Prototype.mat";
        private const string MarkerMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Move_Target.mat";
        private const string RoadMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Village_Road.mat";
        private const string BuildingMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Village_Building.mat";
        private const string BoundaryMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Village_Boundary.mat";
        private const string PropMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Village_Prop.mat";
        private const string NpcMaterialPath = "Assets/ProjectGenesis/Materials/MAT_NPC_Village_Elder.mat";
        private const string WolfMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Enemy_Wolf.mat";
        private const string BoarMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Enemy_Boar.mat";
        private const string TargetRingMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Combat_TargetRing.mat";
        private const string CombatAreaMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Combat_Area.mat";
        private const string LootMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Loot_Weapon.mat";
        private const string RustySwordPath = "Assets/ProjectGenesis/Data/Items/ITM_RustySword.asset";
        private const string WornAxePath = "Assets/ProjectGenesis/Data/Items/ITM_WornAxe.asset";
        private const string WolfLootTablePath = "Assets/ProjectGenesis/Data/LootTables/LT_Wolf.asset";
        private const string BoarLootTablePath = "Assets/ProjectGenesis/Data/LootTables/LT_Boar.asset";
        private const string HeavyStrikePath = "Assets/ProjectGenesis/Data/Skills/SO_Skill_HeavyStrike.asset";
        private const string HumanRacePath = "Assets/ProjectGenesis/Data/Races/SO_Race_Human.asset";
        private const string WarriorClassPath = "Assets/ProjectGenesis/Data/Classes/SO_Class_Warrior.asset";

        [MenuItem("Project Genesis/Sprint 002/Rebuild Starter Village Blockout")]
        public static void RebuildStarterVillageBlockout()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 003/Rebuild Starter Village First Contact")]
        public static void RebuildStarterVillageFirstContact()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 004/Rebuild Starter Village First Fight")]
        public static void RebuildStarterVillageFirstFight()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 005/Rebuild Starter Village First Reward")]
        public static void RebuildStarterVillageFirstReward()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 006/Rebuild Starter Village First Loop")]
        public static void RebuildStarterVillageFirstLoop()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 007/Rebuild Starter Village First Zone Loop")]
        public static void RebuildStarterVillageFirstZoneLoop()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 008/Rebuild Starter Village Quest Foundation")]
        public static void RebuildStarterVillageQuestFoundation()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 009/Rebuild Starter Village Loot Tables")]
        public static void RebuildStarterVillageLootTables()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 010/Rebuild Starter Village Combat Recovery")]
        public static void RebuildStarterVillageCombatRecovery()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 011/Rebuild Starter Village Enemy Territory")]
        public static void RebuildStarterVillageEnemyTerritory()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 014/Rebuild Starter Village Enemy Variety")]
        public static void RebuildStarterVillageEnemyVariety()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 015/Rebuild Starter Village Skills Foundation")]
        public static void RebuildStarterVillageSkillsFoundation()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 016/Rebuild Starter Village Character Identity")]
        public static void RebuildStarterVillageCharacterIdentity()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 017/Rebuild Starter Village Character Entry")]
        public static void RebuildStarterVillageCharacterEntry()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 018/Rebuild Starter Village Character Stats")]
        public static void RebuildStarterVillageCharacterStats()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 020/Rebuild Starter Village Inventory Rearrangement")]
        public static void RebuildStarterVillageInventoryRearrangement()
        {
            RebuildStarterVillage();
        }

        public static void RebuildStarterVillage()
        {
            EnsureFolders();

            Material groundMaterial = CreateMaterial(GroundMaterialPath, new Color(0.28f, 0.42f, 0.28f));
            Material playerMaterial = CreateMaterial(PlayerMaterialPath, new Color(0.25f, 0.45f, 0.9f));
            Material markerMaterial = CreateMaterial(MarkerMaterialPath, new Color(1f, 0.72f, 0.12f));
            Material roadMaterial = CreateMaterial(RoadMaterialPath, new Color(0.46f, 0.38f, 0.28f));
            Material buildingMaterial = CreateMaterial(BuildingMaterialPath, new Color(0.55f, 0.45f, 0.34f));
            Material boundaryMaterial = CreateMaterial(BoundaryMaterialPath, new Color(0.36f, 0.28f, 0.18f));
            Material propMaterial = CreateMaterial(PropMaterialPath, new Color(0.48f, 0.32f, 0.18f));
            Material npcMaterial = CreateMaterial(NpcMaterialPath, new Color(0.72f, 0.64f, 0.42f));
            Material wolfMaterial = CreateMaterial(WolfMaterialPath, new Color(0.28f, 0.3f, 0.34f));
            Material boarMaterial = CreateMaterial(BoarMaterialPath, new Color(0.4f, 0.22f, 0.12f));
            Material targetRingMaterial = CreateMaterial(TargetRingMaterialPath, new Color(0.85f, 0.16f, 0.12f));
            Material combatAreaMaterial = CreateMaterial(CombatAreaMaterialPath, new Color(0.22f, 0.32f, 0.24f));
            Material lootMaterial = CreateMaterial(LootMaterialPath, new Color(0.92f, 0.62f, 0.16f));
            ItemDefinition rustySword = CreateRustySword();
            ItemDefinition wornAxe = CreateWornAxe();
            LootTableDefinition wolfLootTable = CreateWolfLootTable(rustySword);
            LootTableDefinition boarLootTable = CreateBoarLootTable(wornAxe);
            SkillDefinition heavyStrike = CreateHeavyStrike();
            CharacterRaceDefinition humanRace = CreateHumanRace();
            CharacterClassDefinition warriorClass = CreateWarriorClass();

            GameObject playerPrefab = CreatePlayerPrefab(
                playerMaterial,
                rustySword,
                wornAxe,
                heavyStrike,
                humanRace,
                warriorClass);
            GameObject wolfPrefab = CreateWolfPrefab(
                wolfMaterial,
                targetRingMaterial,
                wolfLootTable,
                lootMaterial);
            GameObject boarPrefab = CreateBoarPrefab(
                boarMaterial,
                targetRingMaterial,
                boarLootTable,
                lootMaterial);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "StarterVillage";

            GameObject navigationRoot = new("Navigation");
            CreateGround(navigationRoot.transform, groundMaterial);
            CreateRoad("Road_Main_NorthSouth", new Vector3(0f, 0.015f, 4.5f), new Vector3(1.4f, 0.03f, 25f), roadMaterial);
            CreateRoad("Road_Main_EastWest", new Vector3(0f, 0.02f, 0f), new Vector3(14f, 0.03f, 1.4f), roadMaterial);

            CreateVillageBlockout(buildingMaterial, boundaryMaterial, propMaterial);
            EnemyTerritory combatTerritory =
                CreateNorthCombatArea(combatAreaMaterial, boundaryMaterial, propMaterial);

            GameObject spawnPoint = CreateSpawnPoint();
            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            player.name = "Player";
            player.transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);
            player.AddComponent<QuestLog>();
            PlayerInteractionController interactionController = player.AddComponent<PlayerInteractionController>();
            PlayerCombatController combatController = player.GetComponent<PlayerCombatController>();
            combatController.SetRespawnPoint(spawnPoint.transform);

            GameObject destinationMarker = CreateDestinationMarker(markerMaterial);
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetDestinationMarker(destinationMarker);

            Camera camera = CreateCamera(player.transform);
            playerController.SetCameraTransform(camera.transform);
            interactionController.SetGameplayCamera(camera);

            InteractableNpc villageElder = CreateVillageElder(npcMaterial, targetRingMaterial);
            CreateEnemySpawners(wolfPrefab, boarPrefab, player.transform, combatTerritory);
            FirstContactUi ui = CreateFirstContactUi(player, combatController, villageElder);
            interactionController.SetDialogueWindow(ui.DialogueWindow);
            interactionController.SetPromptView(ui.PromptView);

            CreateEventSystem();
            CreateLighting();
            AddRuntimeNavMesh(navigationRoot);

            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsureFolders()
        {
            string[] folders =
            {
                "Assets/ProjectGenesis/Materials",
                "Assets/ProjectGenesis/Prefabs/Characters",
                "Assets/ProjectGenesis/Prefabs/Enemies",
                "Assets/ProjectGenesis/Prefabs/UI",
                "Assets/ProjectGenesis/Prefabs/World",
                "Assets/ProjectGenesis/Scenes",
                "Assets/ProjectGenesis/Data/Items",
                "Assets/ProjectGenesis/Data/LootTables",
                "Assets/ProjectGenesis/Data/Skills",
                "Assets/ProjectGenesis/Data/Races",
                "Assets/ProjectGenesis/Data/Classes"
            };

            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    string parent = System.IO.Path.GetDirectoryName(folder)?.Replace('\\', '/');
                    string child = System.IO.Path.GetFileName(folder);

                    if (!string.IsNullOrEmpty(parent))
                    {
                        AssetDatabase.CreateFolder(parent, child);
                    }
                }
            }
        }

        private static Material CreateMaterial(string path, Color color)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader)
                {
                    color = color
                };

                AssetDatabase.CreateAsset(material, path);
            }
            else
            {
                material.color = color;
                EditorUtility.SetDirty(material);
            }

            return material;
        }

        private static ItemDefinition CreateRustySword()
        {
            ItemDefinition item = AssetDatabase.LoadAssetAtPath<ItemDefinition>(RustySwordPath);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<ItemDefinition>();
                AssetDatabase.CreateAsset(item, RustySwordPath);
            }

            item.Configure("weapon.rusty_sword", "Ржавый меч", ItemType.Weapon, 4);
            EditorUtility.SetDirty(item);
            return item;
        }

        private static ItemDefinition CreateWornAxe()
        {
            ItemDefinition item = AssetDatabase.LoadAssetAtPath<ItemDefinition>(WornAxePath);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<ItemDefinition>();
                AssetDatabase.CreateAsset(item, WornAxePath);
            }

            item.Configure("weapon.worn_axe", "Потёртый топор", ItemType.Weapon, 7);
            EditorUtility.SetDirty(item);
            return item;
        }

        private static LootTableDefinition CreateWolfLootTable(ItemDefinition rustySword)
        {
            LootTableDefinition lootTable = AssetDatabase.LoadAssetAtPath<LootTableDefinition>(WolfLootTablePath);
            if (lootTable != null)
            {
                return lootTable;
            }

            lootTable = ScriptableObject.CreateInstance<LootTableDefinition>();
            lootTable.ConfigureSingleItem(rustySword, 0.1f, LootRarity.Common);
            AssetDatabase.CreateAsset(lootTable, WolfLootTablePath);
            return lootTable;
        }

        private static LootTableDefinition CreateBoarLootTable(ItemDefinition wornAxe)
        {
            LootTableDefinition lootTable =
                AssetDatabase.LoadAssetAtPath<LootTableDefinition>(BoarLootTablePath);
            if (lootTable != null)
            {
                return lootTable;
            }

            lootTable = ScriptableObject.CreateInstance<LootTableDefinition>();
            lootTable.ConfigureSingleItem(wornAxe, 0.2f, LootRarity.Uncommon);
            AssetDatabase.CreateAsset(lootTable, BoarLootTablePath);
            return lootTable;
        }

        private static SkillDefinition CreateHeavyStrike()
        {
            SkillDefinition skill = AssetDatabase.LoadAssetAtPath<SkillDefinition>(HeavyStrikePath);
            if (skill == null)
            {
                skill = ScriptableObject.CreateInstance<SkillDefinition>();
                AssetDatabase.CreateAsset(skill, HeavyStrikePath);
            }

            skill.Configure(
                "warrior.heavy_strike",
                "Heavy Strike",
                SkillClassRequirement.Warrior,
                SkillTargetType.Enemy,
                1.7f,
                1.55f,
                4f,
                "Мощный удар оружием по выбранному врагу.");
            EditorUtility.SetDirty(skill);
            return skill;
        }

        private static CharacterRaceDefinition CreateHumanRace()
        {
            CharacterRaceDefinition race =
                AssetDatabase.LoadAssetAtPath<CharacterRaceDefinition>(HumanRacePath);
            if (race == null)
            {
                race = ScriptableObject.CreateInstance<CharacterRaceDefinition>();
                AssetDatabase.CreateAsset(race, HumanRacePath);
            }

            race.Configure("race.human", "Человек");
            EditorUtility.SetDirty(race);
            return race;
        }

        private static CharacterClassDefinition CreateWarriorClass()
        {
            CharacterClassDefinition characterClass =
                AssetDatabase.LoadAssetAtPath<CharacterClassDefinition>(WarriorClassPath);
            if (characterClass == null)
            {
                characterClass = ScriptableObject.CreateInstance<CharacterClassDefinition>();
                AssetDatabase.CreateAsset(characterClass, WarriorClassPath);
            }

            characterClass.Configure("class.warrior", "Воин", 10, 2);
            EditorUtility.SetDirty(characterClass);
            return characterClass;
        }

        private static GameObject CreatePlayerPrefab(
            Material playerMaterial,
            ItemDefinition rustySword,
            ItemDefinition wornAxe,
            SkillDefinition heavyStrike,
            CharacterRaceDefinition humanRace,
            CharacterClassDefinition warriorClass)
        {
            GameObject player = new("PF_Player_Prototype");
            player.transform.position = Vector3.zero;

            NavMeshAgent agent = player.AddComponent<NavMeshAgent>();
            agent.height = 2f;
            agent.radius = 0.35f;
            agent.baseOffset = 0f;
            agent.speed = 5f;
            agent.angularSpeed = 720f;
            agent.acceleration = 24f;
            agent.stoppingDistance = 0.25f;
            agent.enabled = false;

            player.AddComponent<PlayerController>();
            Health health = player.AddComponent<Health>();
            health.Configure(90);
            HealthRegeneration regeneration = player.AddComponent<HealthRegeneration>();
            regeneration.Configure(8f, 2, 1f, true);
            CombatStats stats = player.AddComponent<CombatStats>();
            stats.Configure(12, 3, 1.35f, 0.8f);
            PlayerIdentity identity = player.AddComponent<PlayerIdentity>();
            identity.Configure(PlayerIdentity.DefaultCharacterName, humanRace, warriorClass);
            PlayerProgression progression = player.AddComponent<PlayerProgression>();
            progression.ConfigureGrowth(90, 10, 2);
            progression.ConfigureDeathPenalty(0.1f, 10);
            progression.ConfigureEnemyExperienceScaling(0.25f, 0.1f, 0.1f, 1.5f);
            player.AddComponent<PlayerInventory>();
            player.AddComponent<PlayerEquipment>();
            player.AddComponent<PlayerLootController>();
            PlayerPersistenceController persistence = player.AddComponent<PlayerPersistenceController>();
            persistence.Configure(
                new[] { rustySword, wornAxe },
                new[] { humanRace },
                new[] { warriorClass });
            PlayerCombatController combatController = player.AddComponent<PlayerCombatController>();
            PlayerSkillController skillController = player.AddComponent<PlayerSkillController>();
            skillController.ConfigureQuickSlots(new[] { heavyStrike });

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(player.transform, false);
            visual.transform.localPosition = new Vector3(0f, 1f, 0f);
            Object.DestroyImmediate(visual.GetComponent<Collider>());
            visual.GetComponent<Renderer>().sharedMaterial = playerMaterial;
            combatController.SetVisualRoot(visual);

            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(player, PlayerPrefabPath);
            Object.DestroyImmediate(player);

            return savedPrefab;
        }

        private static GameObject CreateWolfPrefab(
            Material wolfMaterial,
            Material targetRingMaterial,
            LootTableDefinition lootTable,
            Material lootMaterial)
        {
            GameObject wolf = new("PF_Enemy_Wolf");

            NavMeshAgent agent = wolf.AddComponent<NavMeshAgent>();
            agent.height = 1.4f;
            agent.radius = 0.4f;
            agent.baseOffset = 0f;
            agent.speed = 3.8f;
            agent.angularSpeed = 540f;
            agent.acceleration = 18f;
            agent.stoppingDistance = 0.8f;
            agent.enabled = false;

            BoxCollider targetCollider = wolf.AddComponent<BoxCollider>();
            targetCollider.center = new Vector3(0f, 0.68f, 0.25f);
            targetCollider.size = new Vector3(1f, 1.4f, 2.1f);

            Health health = wolf.AddComponent<Health>();
            health.Configure(45);
            HealthRegeneration regeneration = wolf.AddComponent<HealthRegeneration>();
            regeneration.Configure(5f, 3, 1f, false);
            CombatStats stats = wolf.AddComponent<CombatStats>();
            stats.Configure(8, 1, 1.05f, 1.1f);
            EnemyBrain brain = wolf.AddComponent<EnemyBrain>();
            brain.Configure(
                2.2f,
                6f,
                20,
                6f,
                "wolf",
                2.4f,
                0.5f,
                1.5f,
                1,
                "Молодой волк");
            EnemyLootDrop lootDrop = wolf.AddComponent<EnemyLootDrop>();
            lootDrop.Configure(
                lootTable,
                lootMaterial,
                "wolf_tail",
                0.7f);

            GameObject visualRoot = new("Visual");
            visualRoot.transform.SetParent(wolf.transform, false);

            CreateEnemyPart(
                "Body",
                visualRoot.transform,
                new Vector3(0f, 0.65f, 0f),
                new Vector3(0.85f, 0.7f, 1.4f),
                wolfMaterial);
            CreateEnemyPart(
                "Head",
                visualRoot.transform,
                new Vector3(0f, 0.82f, 0.86f),
                new Vector3(0.68f, 0.62f, 0.62f),
                wolfMaterial);
            CreateEnemyPart(
                "Ear_Left",
                visualRoot.transform,
                new Vector3(-0.2f, 1.2f, 0.96f),
                new Vector3(0.16f, 0.34f, 0.16f),
                wolfMaterial);
            CreateEnemyPart(
                "Ear_Right",
                visualRoot.transform,
                new Vector3(0.2f, 1.2f, 0.96f),
                new Vector3(0.16f, 0.34f, 0.16f),
                wolfMaterial);

            GameObject selectionRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            selectionRing.name = "SelectionRing";
            selectionRing.transform.SetParent(wolf.transform, false);
            selectionRing.transform.localPosition = new Vector3(0f, 0.025f, 0f);
            selectionRing.transform.localScale = new Vector3(0.78f, 0.025f, 0.78f);
            Object.DestroyImmediate(selectionRing.GetComponent<Collider>());
            selectionRing.GetComponent<Renderer>().sharedMaterial = targetRingMaterial;
            selectionRing.SetActive(false);

            brain.SetVisuals(visualRoot, selectionRing);

            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(wolf, WolfPrefabPath);
            Object.DestroyImmediate(wolf);
            return savedPrefab;
        }

        private static GameObject CreateBoarPrefab(
            Material boarMaterial,
            Material targetRingMaterial,
            LootTableDefinition lootTable,
            Material lootMaterial)
        {
            GameObject boar = new("PF_Enemy_Boar");

            NavMeshAgent agent = boar.AddComponent<NavMeshAgent>();
            agent.height = 1.25f;
            agent.radius = 0.48f;
            agent.baseOffset = 0f;
            agent.speed = 3.4f;
            agent.angularSpeed = 500f;
            agent.acceleration = 16f;
            agent.stoppingDistance = 0.85f;
            agent.enabled = false;

            BoxCollider targetCollider = boar.AddComponent<BoxCollider>();
            targetCollider.center = new Vector3(0f, 0.58f, 0.25f);
            targetCollider.size = new Vector3(1.25f, 1.15f, 2.2f);

            Health health = boar.AddComponent<Health>();
            health.Configure(60);
            HealthRegeneration regeneration = boar.AddComponent<HealthRegeneration>();
            regeneration.Configure(6f, 4, 1.2f, false);
            CombatStats stats = boar.AddComponent<CombatStats>();
            stats.Configure(10, 2, 1.05f, 1.3f);
            EnemyBrain brain = boar.AddComponent<EnemyBrain>();
            brain.Configure(
                2f,
                6f,
                30,
                6f,
                "boar",
                2f,
                1f,
                2.2f,
                2,
                "Лесной кабан");
            EnemyLootDrop lootDrop = boar.AddComponent<EnemyLootDrop>();
            lootDrop.Configure(lootTable, lootMaterial, string.Empty, 0f);

            GameObject visualRoot = new("Visual");
            visualRoot.transform.SetParent(boar.transform, false);

            CreateEnemyPart(
                "Body",
                visualRoot.transform,
                new Vector3(0f, 0.58f, 0f),
                new Vector3(1.1f, 0.78f, 1.5f),
                boarMaterial);
            CreateEnemyPart(
                "Head",
                visualRoot.transform,
                new Vector3(0f, 0.6f, 0.92f),
                new Vector3(0.82f, 0.68f, 0.68f),
                boarMaterial);
            CreateEnemyPart(
                "Snout",
                visualRoot.transform,
                new Vector3(0f, 0.5f, 1.32f),
                new Vector3(0.5f, 0.34f, 0.46f),
                boarMaterial);
            CreateEnemyPart(
                "Ear_Left",
                visualRoot.transform,
                new Vector3(-0.25f, 1f, 0.88f),
                new Vector3(0.18f, 0.3f, 0.18f),
                boarMaterial);
            CreateEnemyPart(
                "Ear_Right",
                visualRoot.transform,
                new Vector3(0.25f, 1f, 0.88f),
                new Vector3(0.18f, 0.3f, 0.18f),
                boarMaterial);

            GameObject selectionRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            selectionRing.name = "SelectionRing";
            selectionRing.transform.SetParent(boar.transform, false);
            selectionRing.transform.localPosition = new Vector3(0f, 0.025f, 0f);
            selectionRing.transform.localScale = new Vector3(0.88f, 0.025f, 0.88f);
            Object.DestroyImmediate(selectionRing.GetComponent<Collider>());
            selectionRing.GetComponent<Renderer>().sharedMaterial = targetRingMaterial;
            selectionRing.SetActive(false);

            brain.SetVisuals(visualRoot, selectionRing);

            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(boar, BoarPrefabPath);
            Object.DestroyImmediate(boar);
            return savedPrefab;
        }

        private static void CreateEnemyPart(
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Material material)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            Object.DestroyImmediate(part.GetComponent<Collider>());
            part.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void CreateGround(Transform parent, Material groundMaterial)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground_StarterVillage";
            ground.transform.SetParent(parent, false);
            ground.transform.localScale = new Vector3(8f, 1f, 8f);
            ground.GetComponent<Renderer>().sharedMaterial = groundMaterial;
        }

        private static void CreateRoad(string name, Vector3 position, Vector3 scale, Material roadMaterial)
        {
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = name;
            road.transform.SetPositionAndRotation(position, Quaternion.identity);
            road.transform.localScale = scale;
            road.GetComponent<Renderer>().sharedMaterial = roadMaterial;
            Object.DestroyImmediate(road.GetComponent<Collider>());
        }

        private static void CreateVillageBlockout(Material buildingMaterial, Material boundaryMaterial, Material propMaterial)
        {
            CreateBuilding("Blockout_House_NorthWest", new Vector3(-5.2f, 1.25f, 4.8f), new Vector3(2.4f, 2.5f, 2.2f), buildingMaterial);
            CreateBuilding("Blockout_House_NorthEast", new Vector3(5.2f, 1.25f, 4.5f), new Vector3(2.6f, 2.5f, 2.4f), buildingMaterial);
            CreateBuilding("Blockout_House_SouthWest", new Vector3(-4.8f, 1.25f, -4.8f), new Vector3(2.5f, 2.5f, 2.5f), buildingMaterial);
            CreateBuilding("Blockout_Barn_SouthEast", new Vector3(5.2f, 1.5f, -4.9f), new Vector3(3.2f, 3f, 2.4f), buildingMaterial);

            CreateBuilding("Blockout_Well", new Vector3(1.8f, 0.45f, 1.7f), new Vector3(1.2f, 0.9f, 1.2f), propMaterial);
            CreateBuilding("Blockout_Crates_WestRoad", new Vector3(-2.7f, 0.45f, 1.1f), new Vector3(1.1f, 0.9f, 1.1f), propMaterial);
            CreateBuilding("Blockout_Cart_EastRoad", new Vector3(3.1f, 0.45f, -1.2f), new Vector3(1.5f, 0.9f, 0.8f), propMaterial);

            CreateBoundary("Boundary_NorthWest", new Vector3(-5.1f, 0.65f, 8.2f), new Vector3(6.8f, 1.3f, 0.4f), boundaryMaterial);
            CreateBoundary("Boundary_NorthEast", new Vector3(5.1f, 0.65f, 8.2f), new Vector3(6.8f, 1.3f, 0.4f), boundaryMaterial);
            CreateBoundary("Boundary_South", new Vector3(0f, 0.65f, -8.2f), new Vector3(17f, 1.3f, 0.4f), boundaryMaterial);
            CreateBoundary("Boundary_East", new Vector3(8.2f, 0.65f, 0f), new Vector3(0.4f, 1.3f, 17f), boundaryMaterial);
            CreateBoundary("Boundary_West", new Vector3(-8.2f, 0.65f, 0f), new Vector3(0.4f, 1.3f, 17f), boundaryMaterial);
        }

        private static EnemyTerritory CreateNorthCombatArea(
            Material combatAreaMaterial,
            Material boundaryMaterial,
            Material propMaterial)
        {
            CreateRoad(
                "Ground_NorthCombatArea",
                new Vector3(0f, 0.008f, 13.2f),
                new Vector3(16f, 0.015f, 9.6f),
                combatAreaMaterial);

            CreateBoundary("Boundary_CombatNorth", new Vector3(0f, 0.65f, 18.2f), new Vector3(17f, 1.3f, 0.4f), boundaryMaterial);
            CreateBoundary("Boundary_CombatEast", new Vector3(8.2f, 0.65f, 13.2f), new Vector3(0.4f, 1.3f, 10f), boundaryMaterial);
            CreateBoundary("Boundary_CombatWest", new Vector3(-8.2f, 0.65f, 13.2f), new Vector3(0.4f, 1.3f, 10f), boundaryMaterial);

            CreateBuilding("CombatArea_Rock_West", new Vector3(-4.6f, 0.6f, 13.8f), new Vector3(1.4f, 1.2f, 1.1f), propMaterial);
            CreateBuilding("CombatArea_Rock_East", new Vector3(4.7f, 0.45f, 15.4f), new Vector3(1.1f, 0.9f, 1.5f), propMaterial);

            GameObject territoryObject = new("Zone_NorthCombat");
            territoryObject.transform.position = new Vector3(0f, 0.05f, 13.2f);
            EnemyTerritory territory = territoryObject.AddComponent<EnemyTerritory>();
            territory.Configure(new Vector2(15.6f, 9.2f), 0.2f);
            return territory;
        }

        private static void CreateBuilding(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetPositionAndRotation(position, Quaternion.identity);
            block.transform.localScale = scale;
            block.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void CreateBoundary(string name, Vector3 position, Vector3 scale, Material material)
        {
            CreateBuilding(name, position, scale, material);
        }

        private static GameObject CreateSpawnPoint()
        {
            GameObject spawnPoint = new("PlayerSpawnPoint");
            spawnPoint.transform.SetPositionAndRotation(new Vector3(0f, 0.05f, -5.8f), Quaternion.identity);
            spawnPoint.AddComponent<PlayerSpawnPoint>();
            return spawnPoint;
        }

        private static GameObject CreateDestinationMarker(Material markerMaterial)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = "MoveTargetMarker";
            marker.transform.localScale = new Vector3(0.35f, 0.015f, 0.35f);
            Object.DestroyImmediate(marker.GetComponent<Collider>());
            marker.GetComponent<Renderer>().sharedMaterial = markerMaterial;
            marker.SetActive(false);
            return marker;
        }

        private static Camera CreateCamera(Transform target)
        {
            GameObject cameraObject = new("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 4.5f, -8f);

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 250f;

            cameraObject.AddComponent<AudioListener>();
            ThirdPersonCameraFollow follow = cameraObject.AddComponent<ThirdPersonCameraFollow>();
            follow.SetTarget(target);

            return camera;
        }

        private static InteractableNpc CreateVillageElder(Material npcMaterial, Material targetRingMaterial)
        {
            GameObject elder = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            elder.name = "NPC_VillageElder";
            elder.transform.SetPositionAndRotation(new Vector3(-1.9f, 1f, -1.9f), Quaternion.Euler(0f, 35f, 0f));
            elder.transform.localScale = new Vector3(0.9f, 1f, 0.9f);
            elder.GetComponent<Renderer>().sharedMaterial = npcMaterial;
            InteractableNpc npc = elder.AddComponent<InteractableNpc>();
            npc.ConfigureQuest(
                "wolves-near-the-road",
                "Волчьи трофеи",
                "Стая у северной дороги угрожает деревне. Принесите старосте доказательства охоты.",
                "Волчьи хвосты",
                "wolf_tail",
                5,
                80);

            GameObject selectionRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            selectionRing.name = "SelectionRing";
            selectionRing.transform.SetParent(elder.transform, false);
            selectionRing.transform.localPosition = new Vector3(0f, -0.98f, 0f);
            selectionRing.transform.localScale = new Vector3(0.78f, 0.025f, 0.78f);
            Object.DestroyImmediate(selectionRing.GetComponent<Collider>());
            selectionRing.GetComponent<Renderer>().sharedMaterial = targetRingMaterial;
            npc.SetSelectionRing(selectionRing);
            return npc;
        }

        private static void CreateEnemySpawners(
            GameObject wolfPrefab,
            GameObject boarPrefab,
            Transform player,
            EnemyTerritory territory)
        {
            CreateEnemySpawner(
                "WolfSpawn_West",
                new Vector3(-6.2f, 0.05f, 11.3f),
                wolfPrefab,
                player,
                territory);
            CreateEnemySpawner(
                "BoarSpawn_North",
                new Vector3(0f, 0.05f, 16.3f),
                boarPrefab,
                player,
                territory);
            CreateEnemySpawner(
                "WolfSpawn_East",
                new Vector3(6.2f, 0.05f, 14.8f),
                wolfPrefab,
                player,
                territory);
        }

        private static void CreateEnemySpawner(
            string name,
            Vector3 position,
            GameObject enemyPrefab,
            Transform player,
            EnemyTerritory territory)
        {
            GameObject spawn = new(name);
            spawn.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 180f, 0f));
            EnemySpawner spawner = spawn.AddComponent<EnemySpawner>();
            spawner.Configure(enemyPrefab, player, 12f, territory);
        }

        private static FirstContactUi CreateFirstContactUi(
            GameObject player,
            PlayerCombatController combatController,
            InteractableNpc villageElder)
        {
            GameObject canvasObject = new("UI_FirstContact");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject textObject = new("Text_Status");
            textObject.transform.SetParent(canvasObject.transform, false);
            Text text = textObject.AddComponent<Text>();
            text.text = "First Zone Prototype";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.color = new Color(1f, 0.95f, 0.82f);
            text.alignment = TextAnchor.UpperLeft;

            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(24f, -24f);
            rect.sizeDelta = new Vector2(420f, 48f);

            InteractionPromptView promptView = CreateInteractionPrompt(canvasObject.transform);
            DialogueWindow dialogueWindow = CreateDialogueWindow(canvasObject.transform);
            CreateCombatHud(canvasObject, player, combatController);
            CreateQuestTracker(canvasObject, player.GetComponent<QuestLog>(), villageElder);
            CreateQuestProgressToast(canvasObject, player.GetComponent<QuestLog>());
            CreateQuestJournalUi(canvasObject, player.GetComponent<QuestLog>(), dialogueWindow);
            CreateInventoryUi(canvasObject, player);
            CreateCharacterStatsUi(canvasObject, player);
            CreateSkillHotbar(canvasObject, player.GetComponent<PlayerSkillController>());
            CreateCharacterEntryUi(canvasObject, player);

            return new FirstContactUi(dialogueWindow, promptView);
        }

        private static void CreateQuestTracker(
            GameObject canvasObject,
            QuestLog questLog,
            InteractableNpc questOwner)
        {
            GameObject root = CreatePanel(
                "QuestTracker",
                canvasObject.transform,
                new Color(0.045f, 0.055f, 0.06f, 0.92f));
            root.GetComponent<Image>().raycastTarget = false;
            SetRect(
                root.GetComponent<RectTransform>(),
                new Vector2(-24f, -24f),
                new Vector2(380f, 116f),
                new Vector2(1f, 1f));

            Text title = CreateText(
                "Text_QuestTrackerTitle",
                root.transform,
                "Волчьи трофеи",
                22,
                TextAnchor.UpperLeft);
            title.color = new Color(1f, 0.84f, 0.48f);
            SetRect(
                title.GetComponent<RectTransform>(),
                new Vector2(16f, -14f),
                new Vector2(348f, 30f),
                new Vector2(0f, 1f));

            Text objective = CreateText(
                "Text_QuestTrackerObjective",
                root.transform,
                "Волчьи хвосты: 0 / 5",
                19,
                TextAnchor.UpperLeft);
            objective.color = new Color(0.77f, 0.91f, 1f);
            SetRect(
                objective.GetComponent<RectTransform>(),
                new Vector2(16f, -56f),
                new Vector2(348f, 42f),
                new Vector2(0f, 1f));

            QuestTrackerView tracker = canvasObject.AddComponent<QuestTrackerView>();
            tracker.Initialize(root, title, objective, questLog, questOwner);
            root.SetActive(false);
        }

        private static void CreateQuestProgressToast(GameObject canvasObject, QuestLog questLog)
        {
            GameObject root = CreatePanel(
                "QuestProgressToast",
                canvasObject.transform,
                new Color(0.055f, 0.075f, 0.08f, 0.96f));
            root.GetComponent<Image>().raycastTarget = false;
            SetRect(
                root.GetComponent<RectTransform>(),
                new Vector2(0f, -146f),
                new Vector2(520f, 86f),
                new Vector2(0.5f, 1f));

            Text message = CreateText(
                "Text_QuestProgressToast",
                root.transform,
                "Волчьи трофеи\nВолчьи хвосты: 1 / 5",
                20,
                TextAnchor.MiddleCenter);
            message.color = new Color(1f, 0.86f, 0.5f);
            message.lineSpacing = 1.05f;
            Stretch(message.GetComponent<RectTransform>(), 18f, 8f, 18f, 8f);

            QuestProgressToastView toast = canvasObject.AddComponent<QuestProgressToastView>();
            toast.Initialize(root, message, questLog);
            root.SetActive(false);
        }

        private static void CreateQuestJournalUi(
            GameObject canvasObject,
            QuestLog questLog,
            DialogueWindow dialogueWindow)
        {
            Button openButton = CreateButton("Button_OpenQuestJournal", canvasObject.transform, "Задания [J]");
            openButton.GetComponent<Image>().color = new Color(0.12f, 0.18f, 0.22f, 0.96f);
            SetRect(
                openButton.GetComponent<RectTransform>(),
                new Vector2(-24f, 92f),
                new Vector2(220f, 56f),
                new Vector2(1f, 0f));

            GameObject window = CreatePanel(
                "QuestJournalWindow",
                canvasObject.transform,
                new Color(0.04f, 0.05f, 0.055f, 0.98f));
            SetRect(
                window.GetComponent<RectTransform>(),
                new Vector2(-180f, 0f),
                new Vector2(1020f, 640f),
                new Vector2(0.5f, 0.5f));
            CreateWindowDragHandle(window, 950f);

            Text header = CreateText("Text_QuestJournalHeader", window.transform, "Журнал заданий", 30, TextAnchor.UpperLeft);
            header.color = new Color(1f, 0.84f, 0.48f);
            SetRect(header.GetComponent<RectTransform>(), new Vector2(24f, -20f), new Vector2(760f, 44f), new Vector2(0f, 1f));

            Button closeButton = CreateButton("Button_CloseQuestJournal", window.transform, "X");
            closeButton.GetComponent<Image>().color = new Color(0.28f, 0.1f, 0.09f, 1f);
            SetRect(closeButton.GetComponent<RectTransform>(), new Vector2(-18f, -18f), new Vector2(38f, 36f), new Vector2(1f, 1f));

            Button activeTab = CreateButton("Button_ActiveQuests", window.transform, "Активные");
            SetRect(activeTab.GetComponent<RectTransform>(), new Vector2(24f, -78f), new Vector2(136f, 44f), new Vector2(0f, 1f));

            Button completedTab = CreateButton("Button_CompletedQuests", window.transform, "Завершённые");
            SetRect(completedTab.GetComponent<RectTransform>(), new Vector2(170f, -78f), new Vector2(166f, 44f), new Vector2(0f, 1f));

            GameObject listRootObject = new("QuestListRoot", typeof(RectTransform));
            listRootObject.transform.SetParent(window.transform, false);
            RectTransform listRoot = listRootObject.GetComponent<RectTransform>();
            SetRect(listRoot, new Vector2(24f, -144f), new Vector2(262f, 390f), new Vector2(0f, 1f));

            Button questButtonTemplate = CreateButton("Button_QuestTemplate", listRoot, "Название задания");
            SetRect(
                questButtonTemplate.GetComponent<RectTransform>(),
                Vector2.zero,
                new Vector2(262f, 50f),
                new Vector2(0f, 1f));
            questButtonTemplate.gameObject.SetActive(false);

            Text emptyText = CreateText("Text_EmptyQuestList", listRoot, "Нет активных заданий", 18, TextAnchor.UpperLeft);
            emptyText.color = new Color(0.62f, 0.68f, 0.7f);
            SetRect(emptyText.GetComponent<RectTransform>(), new Vector2(4f, -8f), new Vector2(250f, 60f), new Vector2(0f, 1f));

            GameObject divider = CreatePanel("QuestJournalDivider", window.transform, new Color(0.25f, 0.3f, 0.32f, 1f));
            divider.GetComponent<Image>().raycastTarget = false;
            SetRect(divider.GetComponent<RectTransform>(), new Vector2(310f, -142f), new Vector2(2f, 438f), new Vector2(0f, 1f));

            Text title = CreateText("Text_SelectedQuestTitle", window.transform, "Выберите задание", 28, TextAnchor.UpperLeft);
            title.color = new Color(1f, 0.84f, 0.48f);
            SetRect(title.GetComponent<RectTransform>(), new Vector2(340f, -142f), new Vector2(638f, 42f), new Vector2(0f, 1f));

            Text state = CreateText("Text_SelectedQuestState", window.transform, "", 19, TextAnchor.UpperLeft);
            state.color = new Color(0.48f, 0.82f, 1f);
            SetRect(state.GetComponent<RectTransform>(), new Vector2(340f, -190f), new Vector2(638f, 30f), new Vector2(0f, 1f));

            Text description = CreateText("Text_SelectedQuestDescription", window.transform, "", 20, TextAnchor.UpperLeft);
            description.color = new Color(0.9f, 0.91f, 0.88f);
            description.lineSpacing = 1.08f;
            SetRect(description.GetComponent<RectTransform>(), new Vector2(340f, -234f), new Vector2(638f, 102f), new Vector2(0f, 1f));

            Text objective = CreateText("Text_SelectedQuestObjective", window.transform, "", 21, TextAnchor.UpperLeft);
            objective.color = new Color(0.76f, 0.91f, 1f);
            SetRect(objective.GetComponent<RectTransform>(), new Vector2(340f, -350f), new Vector2(638f, 36f), new Vector2(0f, 1f));

            Text giver = CreateText("Text_SelectedQuestGiver", window.transform, "", 19, TextAnchor.UpperLeft);
            giver.color = new Color(0.73f, 0.78f, 0.8f);
            SetRect(giver.GetComponent<RectTransform>(), new Vector2(340f, -400f), new Vector2(638f, 32f), new Vector2(0f, 1f));

            Text reward = CreateText("Text_SelectedQuestReward", window.transform, "", 20, TextAnchor.UpperLeft);
            reward.color = new Color(1f, 0.72f, 0.48f);
            SetRect(reward.GetComponent<RectTransform>(), new Vector2(340f, -446f), new Vector2(638f, 34f), new Vector2(0f, 1f));

            Button abandonButton = CreateButton("Button_AbandonQuest", window.transform, "Отказаться");
            abandonButton.GetComponent<Image>().color = new Color(0.3f, 0.13f, 0.1f, 1f);
            SetRect(abandonButton.GetComponent<RectTransform>(), new Vector2(340f, 30f), new Vector2(240f, 54f), new Vector2(0f, 0f));
            Text abandonText = abandonButton.GetComponentInChildren<Text>();

            Text hint = CreateText("Text_QuestJournalHint", window.transform, "J - закрыть журнал", 18, TextAnchor.LowerRight);
            hint.color = new Color(0.62f, 0.68f, 0.7f);
            SetRect(hint.GetComponent<RectTransform>(), new Vector2(-24f, 42f), new Vector2(230f, 28f), new Vector2(1f, 0f));

            QuestJournalView journal = canvasObject.AddComponent<QuestJournalView>();
            journal.Initialize(
                window,
                openButton,
                closeButton,
                activeTab,
                completedTab,
                listRoot,
                questButtonTemplate,
                emptyText,
                title,
                state,
                description,
                objective,
                giver,
                reward,
                abandonButton,
                abandonText,
                questLog,
                dialogueWindow);
            window.SetActive(false);
        }

        private static void CreateInventoryUi(GameObject canvasObject, GameObject player)
        {
            Button openButton = CreateButton("Button_OpenInventory", canvasObject.transform, "Инвентарь [I]");
            openButton.GetComponent<Image>().color = new Color(0.12f, 0.18f, 0.22f, 0.96f);
            SetRect(
                openButton.GetComponent<RectTransform>(),
                new Vector2(-24f, 24f),
                new Vector2(220f, 56f),
                new Vector2(1f, 0f));

            GameObject window = CreatePanel(
                "InventoryWindow",
                canvasObject.transform,
                new Color(0.045f, 0.055f, 0.06f, 0.97f));
            SetRect(
                window.GetComponent<RectTransform>(),
                new Vector2(-28f, 0f),
                new Vector2(520f, 640f),
                new Vector2(1f, 0.5f));
            CreateWindowDragHandle(window, 450f);

            Text title = CreateText("Text_InventoryTitle", window.transform, "Инвентарь", 30, TextAnchor.UpperLeft);
            title.color = new Color(1f, 0.84f, 0.48f);
            SetRect(title.GetComponent<RectTransform>(), new Vector2(24f, -22f), new Vector2(390f, 42f), new Vector2(0f, 1f));

            Button closeButton = CreateButton("Button_CloseInventory", window.transform, "X");
            closeButton.GetComponent<Image>().color = new Color(0.28f, 0.1f, 0.09f, 1f);
            SetRect(closeButton.GetComponent<RectTransform>(), new Vector2(-18f, -18f), new Vector2(38f, 36f), new Vector2(1f, 1f));

            Text characterNameText = CreateText(
                "Text_CharacterName",
                window.transform,
                PlayerIdentity.DefaultCharacterName,
                24,
                TextAnchor.UpperLeft);
            characterNameText.color = new Color(0.95f, 0.95f, 0.92f);
            SetRect(
                characterNameText.GetComponent<RectTransform>(),
                new Vector2(24f, -76f),
                new Vector2(468f, 34f),
                new Vector2(0f, 1f));

            Text identityDetailsText = CreateText(
                "Text_CharacterIdentity",
                window.transform,
                "Человек · Воин",
                19,
                TextAnchor.UpperLeft);
            identityDetailsText.color = new Color(0.72f, 0.82f, 0.88f);
            SetRect(
                identityDetailsText.GetComponent<RectTransform>(),
                new Vector2(24f, -112f),
                new Vector2(468f, 30f),
                new Vector2(0f, 1f));

            Text capacityText = CreateText("Text_InventoryCapacity", window.transform, "Ячейки: 0 / 8", 21, TextAnchor.UpperLeft);
            capacityText.color = new Color(0.82f, 0.88f, 0.92f);
            SetRect(capacityText.GetComponent<RectTransform>(), new Vector2(24f, -158f), new Vector2(220f, 30f), new Vector2(0f, 1f));

            Text attackText = CreateText("Text_AttackPower", window.transform, "Сила атаки: 14", 21, TextAnchor.UpperLeft);
            attackText.color = new Color(1f, 0.72f, 0.48f);
            SetRect(attackText.GetComponent<RectTransform>(), new Vector2(272f, -158f), new Vector2(220f, 30f), new Vector2(0f, 1f));

            Text mainHandText = CreateText("Text_MainHand", window.transform, "Оружие: не экипировано", 21, TextAnchor.UpperLeft);
            mainHandText.color = new Color(0.72f, 0.9f, 1f);
            SetRect(mainHandText.GetComponent<RectTransform>(), new Vector2(24f, -202f), new Vector2(468f, 34f), new Vector2(0f, 1f));

            GameObject divider = CreatePanel("Divider", window.transform, new Color(0.28f, 0.32f, 0.34f, 1f));
            divider.GetComponent<Image>().raycastTarget = false;
            SetRect(divider.GetComponent<RectTransform>(), new Vector2(24f, -250f), new Vector2(468f, 2f), new Vector2(0f, 1f));

            Text contentsTitle = CreateText("Text_ContentsTitle", window.transform, "Содержимое", 22, TextAnchor.UpperLeft);
            contentsTitle.color = new Color(0.9f, 0.92f, 0.94f);
            SetRect(contentsTitle.GetComponent<RectTransform>(), new Vector2(24f, -274f), new Vector2(468f, 32f), new Vector2(0f, 1f));

            Button[] slotButtons = new Button[8];
            Text[] slotTexts = new Text[8];
            for (int index = 0; index < slotButtons.Length; index++)
            {
                int column = index % 2;
                int row = index / 2;
                Button slotButton = CreateButton(
                    $"Button_InventorySlot_{index + 1}",
                    window.transform,
                    $"{index + 1}. Пусто");
                SetRect(
                    slotButton.GetComponent<RectTransform>(),
                    new Vector2(24f + column * 238f, -312f - row * 48f),
                    new Vector2(230f, 40f),
                    new Vector2(0f, 1f));
                Text slotText = slotButton.GetComponentInChildren<Text>();
                slotText.fontSize = 16;
                slotText.alignment = TextAnchor.MiddleLeft;
                slotButtons[index] = slotButton;
                slotTexts[index] = slotText;
            }

            Text itemNameText = CreateText("Text_ItemName", window.transform, "Инвентарь пуст", 24, TextAnchor.UpperLeft);
            itemNameText.color = new Color(1f, 0.82f, 0.42f);
            SetRect(itemNameText.GetComponent<RectTransform>(), new Vector2(24f, -510f), new Vector2(468f, 32f), new Vector2(0f, 1f));

            Text itemDetailsText = CreateText(
                "Text_ItemDetails",
                window.transform,
                "Победите волка и подберите выпавший предмет.",
                20,
                TextAnchor.UpperLeft);
            itemDetailsText.color = new Color(0.82f, 0.84f, 0.82f);
            itemDetailsText.lineSpacing = 1.1f;
            SetRect(itemDetailsText.GetComponent<RectTransform>(), new Vector2(24f, -548f), new Vector2(468f, 40f), new Vector2(0f, 1f));

            Button actionButton = CreateButton("Button_ItemAction", window.transform, "Надеть");
            SetRect(actionButton.GetComponent<RectTransform>(), new Vector2(24f, 8f), new Vector2(180f, 42f), new Vector2(0f, 0f));
            Text actionText = actionButton.GetComponentInChildren<Text>();
            actionText.fontSize = 18;

            Text hintText = CreateText("Text_InventoryHint", window.transform, "Перетащите предмет · I закрыть", 18, TextAnchor.LowerRight);
            hintText.color = new Color(0.62f, 0.68f, 0.7f);
            SetRect(hintText.GetComponent<RectTransform>(), new Vector2(-24f, 16f), new Vector2(240f, 28f), new Vector2(1f, 0f));

            InventoryView inventoryView = canvasObject.AddComponent<InventoryView>();
            inventoryView.Initialize(
                window,
                openButton,
                closeButton,
                actionButton,
                slotButtons,
                slotTexts,
                capacityText,
                attackText,
                mainHandText,
                itemNameText,
                itemDetailsText,
                actionText,
                player.GetComponent<PlayerInventory>(),
                player.GetComponent<PlayerEquipment>(),
                player.GetComponent<CombatStats>());

            for (int index = 0; index < slotButtons.Length; index++)
            {
                InventorySlotDragHandler dragHandler =
                    slotButtons[index].gameObject.AddComponent<InventorySlotDragHandler>();
                dragHandler.Initialize(inventoryView, index);
            }

            CharacterIdentityView identityView = canvasObject.AddComponent<CharacterIdentityView>();
            identityView.Initialize(
                characterNameText,
                identityDetailsText,
                player.GetComponent<PlayerIdentity>());
            window.SetActive(false);
        }

        private static void CreateCharacterStatsUi(GameObject canvasObject, GameObject player)
        {
            Button openButton = CreateButton(
                "Button_OpenCharacterStats",
                canvasObject.transform,
                "Характеристики [C]");
            openButton.GetComponent<Image>().color = new Color(0.12f, 0.18f, 0.22f, 0.96f);
            SetRect(
                openButton.GetComponent<RectTransform>(),
                new Vector2(-24f, 160f),
                new Vector2(220f, 56f),
                new Vector2(1f, 0f));

            GameObject window = CreatePanel(
                "CharacterStatsWindow",
                canvasObject.transform,
                new Color(0.045f, 0.055f, 0.06f, 0.97f));
            SetRect(
                window.GetComponent<RectTransform>(),
                new Vector2(28f, 0f),
                new Vector2(520f, 600f),
                new Vector2(0f, 0.5f));
            CreateWindowDragHandle(window, 450f);

            Text title = CreateText(
                "Text_CharacterStatsTitle",
                window.transform,
                "Характеристики",
                30,
                TextAnchor.UpperLeft);
            title.color = new Color(1f, 0.84f, 0.48f);
            SetRect(
                title.GetComponent<RectTransform>(),
                new Vector2(24f, -22f),
                new Vector2(390f, 42f),
                new Vector2(0f, 1f));

            Button closeButton = CreateButton("Button_CloseCharacterStats", window.transform, "X");
            closeButton.GetComponent<Image>().color = new Color(0.28f, 0.1f, 0.09f, 1f);
            SetRect(
                closeButton.GetComponent<RectTransform>(),
                new Vector2(-18f, -18f),
                new Vector2(38f, 36f),
                new Vector2(1f, 1f));

            Text characterName = CreateText(
                "Text_StatsCharacterName",
                window.transform,
                PlayerIdentity.DefaultCharacterName,
                26,
                TextAnchor.UpperLeft);
            characterName.color = new Color(0.95f, 0.95f, 0.92f);
            SetRect(
                characterName.GetComponent<RectTransform>(),
                new Vector2(24f, -76f),
                new Vector2(468f, 36f),
                new Vector2(0f, 1f));

            Text identity = CreateText(
                "Text_StatsIdentity",
                window.transform,
                "Уровень 1 · Человек · Воин",
                19,
                TextAnchor.UpperLeft);
            identity.color = new Color(0.72f, 0.82f, 0.88f);
            SetRect(
                identity.GetComponent<RectTransform>(),
                new Vector2(24f, -114f),
                new Vector2(468f, 30f),
                new Vector2(0f, 1f));

            GameObject divider = CreatePanel(
                "Divider_CharacterStats",
                window.transform,
                new Color(0.28f, 0.32f, 0.34f, 1f));
            divider.GetComponent<Image>().raycastTarget = false;
            SetRect(
                divider.GetComponent<RectTransform>(),
                new Vector2(24f, -158f),
                new Vector2(468f, 2f),
                new Vector2(0f, 1f));

            Text experience = CreateText(
                "Text_StatsExperience",
                window.transform,
                "Опыт: 0 / 100",
                20,
                TextAnchor.UpperLeft);
            experience.color = new Color(0.82f, 0.88f, 0.92f);
            SetRect(experience.GetComponent<RectTransform>(), new Vector2(24f, -184f), new Vector2(468f, 30f), new Vector2(0f, 1f));

            Text health = CreateText(
                "Text_StatsHealth",
                window.transform,
                "Здоровье: 100 / 100",
                22,
                TextAnchor.UpperLeft);
            health.color = new Color(0.48f, 0.9f, 0.58f);
            SetRect(health.GetComponent<RectTransform>(), new Vector2(24f, -232f), new Vector2(468f, 32f), new Vector2(0f, 1f));

            Text healthBreakdown = CreateText(
                "Text_StatsHealthBreakdown",
                window.transform,
                "Бонусы: класс +10 · уровень +0",
                18,
                TextAnchor.UpperLeft);
            healthBreakdown.color = new Color(0.68f, 0.74f, 0.72f);
            SetRect(healthBreakdown.GetComponent<RectTransform>(), new Vector2(24f, -266f), new Vector2(468f, 28f), new Vector2(0f, 1f));

            Text attack = CreateText(
                "Text_StatsAttack",
                window.transform,
                "Сила атаки: 14",
                22,
                TextAnchor.UpperLeft);
            attack.color = new Color(1f, 0.72f, 0.48f);
            SetRect(attack.GetComponent<RectTransform>(), new Vector2(24f, -316f), new Vector2(468f, 32f), new Vector2(0f, 1f));

            Text attackBreakdown = CreateText(
                "Text_StatsAttackBreakdown",
                window.transform,
                "Бонусы: класс +2 · уровень +0 · оружие +0",
                18,
                TextAnchor.UpperLeft);
            attackBreakdown.color = new Color(0.72f, 0.7f, 0.66f);
            SetRect(attackBreakdown.GetComponent<RectTransform>(), new Vector2(24f, -350f), new Vector2(468f, 28f), new Vector2(0f, 1f));

            Text defense = CreateText(
                "Text_StatsDefense",
                window.transform,
                "Защита: 3",
                21,
                TextAnchor.UpperLeft);
            defense.color = new Color(0.62f, 0.8f, 1f);
            SetRect(defense.GetComponent<RectTransform>(), new Vector2(24f, -400f), new Vector2(468f, 30f), new Vector2(0f, 1f));

            Text attackTiming = CreateText(
                "Text_StatsAttackTiming",
                window.transform,
                "Скорость атаки: 1 удар каждые 0,8 с",
                18,
                TextAnchor.UpperLeft);
            attackTiming.color = new Color(0.78f, 0.8f, 0.82f);
            SetRect(attackTiming.GetComponent<RectTransform>(), new Vector2(24f, -442f), new Vector2(468f, 48f), new Vector2(0f, 1f));

            Text skillPower = CreateText(
                "Text_StatsHeavyStrike",
                window.transform,
                "Heavy Strike: усиленный удар (170% силы атаки)",
                20,
                TextAnchor.UpperLeft);
            skillPower.color = new Color(1f, 0.84f, 0.48f);
            SetRect(skillPower.GetComponent<RectTransform>(), new Vector2(24f, -510f), new Vector2(468f, 52f), new Vector2(0f, 1f));

            CharacterStatsView statsView = canvasObject.AddComponent<CharacterStatsView>();
            statsView.Initialize(
                window,
                openButton,
                closeButton,
                characterName,
                identity,
                experience,
                health,
                healthBreakdown,
                attack,
                attackBreakdown,
                defense,
                attackTiming,
                skillPower,
                player.GetComponent<PlayerIdentity>(),
                player.GetComponent<PlayerProgression>(),
                player.GetComponent<Health>(),
                player.GetComponent<CombatStats>(),
                player.GetComponent<PlayerCombatController>(),
                player.GetComponent<PlayerSkillController>());
            window.SetActive(false);
        }

        private static void CreateSkillHotbar(GameObject canvasObject, PlayerSkillController skillController)
        {
            GameObject root = CreatePanel(
                "SkillHotbar",
                canvasObject.transform,
                new Color(0.035f, 0.045f, 0.05f, 0.9f));
            root.GetComponent<Image>().raycastTarget = false;
            SetRect(
                root.GetComponent<RectTransform>(),
                new Vector2(0f, 24f),
                new Vector2(260f, 72f),
                new Vector2(0.5f, 0f));

            Button heavyStrikeButton = CreateButton("Button_Skill_HeavyStrike", root.transform, "Heavy Strike");
            SetRect(
                heavyStrikeButton.GetComponent<RectTransform>(),
                new Vector2(12f, 12f),
                new Vector2(236f, 48f),
                new Vector2(0f, 0f));
            Text heavyStrikeLabel = heavyStrikeButton.GetComponentInChildren<Text>();
            heavyStrikeLabel.fontSize = 18;

            Text feedback = CreateText(
                "Text_SkillFeedback",
                canvasObject.transform,
                "Heavy Strike",
                20,
                TextAnchor.MiddleCenter);
            feedback.color = new Color(1f, 0.86f, 0.5f);
            SetRect(
                feedback.GetComponent<RectTransform>(),
                new Vector2(0f, 106f),
                new Vector2(620f, 34f),
                new Vector2(0.5f, 0f));

            SkillHotbarView hotbar = canvasObject.AddComponent<SkillHotbarView>();
            hotbar.Initialize(
                new[] { heavyStrikeButton },
                new[] { heavyStrikeLabel },
                feedback,
                skillController);

            GameObject tooltip = CreatePanel(
                "SkillTooltip",
                canvasObject.transform,
                new Color(0.035f, 0.045f, 0.05f, 0.98f));
            tooltip.GetComponent<Image>().raycastTarget = false;
            SetRect(
                tooltip.GetComponent<RectTransform>(),
                new Vector2(0f, 148f),
                new Vector2(430f, 196f),
                new Vector2(0.5f, 0f));

            Text tooltipName = CreateText(
                "Text_SkillTooltipName",
                tooltip.transform,
                "Heavy Strike",
                23,
                TextAnchor.UpperLeft);
            tooltipName.color = new Color(1f, 0.84f, 0.48f);
            SetRect(tooltipName.GetComponent<RectTransform>(), new Vector2(18f, -14f), new Vector2(394f, 30f), new Vector2(0f, 1f));

            Text tooltipDescription = CreateText(
                "Text_SkillTooltipDescription",
                tooltip.transform,
                "Мощный удар оружием по выбранному врагу.",
                18,
                TextAnchor.UpperLeft);
            tooltipDescription.color = new Color(0.88f, 0.9f, 0.92f);
            SetRect(tooltipDescription.GetComponent<RectTransform>(), new Vector2(18f, -50f), new Vector2(394f, 48f), new Vector2(0f, 1f));

            Text tooltipDamage = CreateText(
                "Text_SkillTooltipDamage",
                tooltip.transform,
                "Урон: 170% силы атаки",
                19,
                TextAnchor.UpperLeft);
            tooltipDamage.color = new Color(1f, 0.72f, 0.48f);
            SetRect(tooltipDamage.GetComponent<RectTransform>(), new Vector2(18f, -108f), new Vector2(394f, 28f), new Vector2(0f, 1f));

            Text tooltipDetails = CreateText(
                "Text_SkillTooltipDetails",
                tooltip.transform,
                "Дальность: 1,55 · Перезарядка: 4 с",
                17,
                TextAnchor.UpperLeft);
            tooltipDetails.color = new Color(0.7f, 0.78f, 0.82f);
            SetRect(tooltipDetails.GetComponent<RectTransform>(), new Vector2(18f, -146f), new Vector2(394f, 28f), new Vector2(0f, 1f));

            SkillTooltipView tooltipView = heavyStrikeButton.gameObject.AddComponent<SkillTooltipView>();
            tooltipView.Initialize(
                tooltip,
                tooltipName,
                tooltipDescription,
                tooltipDamage,
                tooltipDetails,
                skillController,
                skillController.GetComponent<PlayerCombatController>(),
                skillController.GetComponent<CombatStats>(),
                0);
            tooltip.SetActive(false);
        }

        private static void CreateCharacterEntryUi(GameObject canvasObject, GameObject player)
        {
            GameObject root = CreatePanel(
                "CharacterEntryOverlay",
                canvasObject.transform,
                new Color(0.025f, 0.03f, 0.035f, 0.985f));
            Stretch(root.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);

            GameObject creationRoot = CreatePanel(
                "CharacterCreationMode",
                root.transform,
                Color.clear);
            creationRoot.GetComponent<Image>().raycastTarget = false;
            Stretch(creationRoot.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);

            Text creationTitle = CreateText(
                "Text_CharacterCreationTitle",
                creationRoot.transform,
                "Создание персонажа",
                38,
                TextAnchor.MiddleCenter);
            creationTitle.color = new Color(1f, 0.84f, 0.48f);
            SetRect(
                creationTitle.GetComponent<RectTransform>(),
                new Vector2(0f, -170f),
                new Vector2(760f, 58f),
                new Vector2(0.5f, 1f));

            Text creationIdentity = CreateText(
                "Text_CreationIdentity",
                creationRoot.transform,
                "Человек · Воин",
                23,
                TextAnchor.MiddleCenter);
            creationIdentity.color = new Color(0.72f, 0.86f, 0.94f);
            SetRect(
                creationIdentity.GetComponent<RectTransform>(),
                new Vector2(0f, -246f),
                new Vector2(600f, 40f),
                new Vector2(0.5f, 1f));

            Text nameLabel = CreateText(
                "Text_CharacterNameLabel",
                creationRoot.transform,
                "Имя персонажа",
                21,
                TextAnchor.MiddleLeft);
            nameLabel.color = new Color(0.88f, 0.9f, 0.92f);
            SetRect(
                nameLabel.GetComponent<RectTransform>(),
                new Vector2(0f, -342f),
                new Vector2(560f, 36f),
                new Vector2(0.5f, 1f));

            InputField nameInput = CreateInputField(
                "Input_CharacterName",
                creationRoot.transform,
                "Введите имя");
            nameInput.characterLimit = PlayerIdentity.MaximumNameLength;
            SetRect(
                nameInput.GetComponent<RectTransform>(),
                new Vector2(0f, -386f),
                new Vector2(560f, 62f),
                new Vector2(0.5f, 1f));

            Button createButton = CreateButton(
                "Button_CreateCharacter",
                creationRoot.transform,
                "Создать персонажа");
            SetRect(
                createButton.GetComponent<RectTransform>(),
                new Vector2(0f, -492f),
                new Vector2(300f, 60f),
                new Vector2(0.5f, 1f));

            Text creationFeedback = CreateText(
                "Text_CharacterCreationFeedback",
                creationRoot.transform,
                string.Empty,
                19,
                TextAnchor.MiddleCenter);
            creationFeedback.color = new Color(1f, 0.58f, 0.45f);
            SetRect(
                creationFeedback.GetComponent<RectTransform>(),
                new Vector2(0f, -568f),
                new Vector2(620f, 40f),
                new Vector2(0.5f, 1f));

            GameObject selectionRoot = CreatePanel(
                "CharacterSelectionMode",
                root.transform,
                Color.clear);
            selectionRoot.GetComponent<Image>().raycastTarget = false;
            Stretch(selectionRoot.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);

            Text selectionTitle = CreateText(
                "Text_CharacterSelectionTitle",
                selectionRoot.transform,
                "Выбор персонажа",
                38,
                TextAnchor.MiddleCenter);
            selectionTitle.color = new Color(1f, 0.84f, 0.48f);
            SetRect(
                selectionTitle.GetComponent<RectTransform>(),
                new Vector2(0f, -190f),
                new Vector2(760f, 58f),
                new Vector2(0.5f, 1f));

            Text selectedName = CreateText(
                "Text_SelectedCharacterName",
                selectionRoot.transform,
                PlayerIdentity.DefaultCharacterName,
                32,
                TextAnchor.MiddleCenter);
            selectedName.color = new Color(0.96f, 0.96f, 0.93f);
            SetRect(
                selectedName.GetComponent<RectTransform>(),
                new Vector2(0f, -314f),
                new Vector2(680f, 50f),
                new Vector2(0.5f, 1f));

            Text selectedIdentity = CreateText(
                "Text_SelectedCharacterIdentity",
                selectionRoot.transform,
                "Человек · Воин",
                23,
                TextAnchor.MiddleCenter);
            selectedIdentity.color = new Color(0.72f, 0.86f, 0.94f);
            SetRect(
                selectedIdentity.GetComponent<RectTransform>(),
                new Vector2(0f, -376f),
                new Vector2(620f, 40f),
                new Vector2(0.5f, 1f));

            Text selectedLevel = CreateText(
                "Text_SelectedCharacterLevel",
                selectionRoot.transform,
                "Уровень 1",
                21,
                TextAnchor.MiddleCenter);
            selectedLevel.color = new Color(0.84f, 0.86f, 0.88f);
            SetRect(
                selectedLevel.GetComponent<RectTransform>(),
                new Vector2(0f, -430f),
                new Vector2(400f, 38f),
                new Vector2(0.5f, 1f));

            Button playButton = CreateButton(
                "Button_EnterCharacter",
                selectionRoot.transform,
                "Играть");
            SetRect(
                playButton.GetComponent<RectTransform>(),
                new Vector2(0f, -518f),
                new Vector2(260f, 60f),
                new Vector2(0.5f, 1f));

            MonoBehaviour[] gameplayBehaviours =
            {
                player.GetComponent<PlayerController>(),
                player.GetComponent<PlayerInteractionController>(),
                player.GetComponent<PlayerCombatController>(),
                player.GetComponent<PlayerSkillController>(),
                player.GetComponent<PlayerLootController>(),
                canvasObject.GetComponent<InventoryView>(),
                canvasObject.GetComponent<QuestJournalView>(),
                canvasObject.GetComponent<SkillHotbarView>(),
                canvasObject.GetComponent<CharacterStatsView>()
            };

            CharacterEntryView entryView = canvasObject.AddComponent<CharacterEntryView>();
            entryView.Initialize(
                root,
                creationRoot,
                selectionRoot,
                nameInput,
                createButton,
                playButton,
                creationFeedback,
                selectedName,
                selectedIdentity,
                selectedLevel,
                player.GetComponent<PlayerPersistenceController>(),
                player.GetComponent<PlayerIdentity>(),
                player.GetComponent<PlayerProgression>(),
                gameplayBehaviours);
            root.SetActive(true);
        }

        private static void CreateCombatHud(
            GameObject canvasObject,
            GameObject player,
            PlayerCombatController combatController)
        {
            GameObject playerPanel = CreatePanel(
                "CombatHud_Player",
                canvasObject.transform,
                new Color(0.045f, 0.055f, 0.065f, 0.9f));
            playerPanel.GetComponent<Image>().raycastTarget = false;
            SetRect(
                playerPanel.GetComponent<RectTransform>(),
                new Vector2(24f, -76f),
                new Vector2(360f, 116f),
                new Vector2(0f, 1f));

            Text playerHealthText = CreateText(
                "Text_PlayerHealth",
                playerPanel.transform,
                "Здоровье: 100 / 100",
                21,
                TextAnchor.UpperLeft);
            playerHealthText.color = Color.white;
            SetRect(
                playerHealthText.GetComponent<RectTransform>(),
                new Vector2(16f, -12f),
                new Vector2(328f, 28f),
                new Vector2(0f, 1f));

            Image playerHealthFill = CreateHealthBar(
                "PlayerHealthBar",
                playerPanel.transform,
                new Vector2(16f, -46f),
                new Vector2(328f, 20f),
                new Color(0.22f, 0.72f, 0.32f));

            Text experienceText = CreateText(
                "Text_Experience",
                playerPanel.transform,
                "Уровень 1   Опыт: 0",
                19,
                TextAnchor.UpperLeft);
            experienceText.color = new Color(0.8f, 0.88f, 1f);
            SetRect(
                experienceText.GetComponent<RectTransform>(),
                new Vector2(16f, -76f),
                new Vector2(328f, 26f),
                new Vector2(0f, 1f));

            GameObject targetPanel = CreatePanel(
                "CombatHud_Target",
                canvasObject.transform,
                new Color(0.07f, 0.045f, 0.045f, 0.92f));
            targetPanel.GetComponent<Image>().raycastTarget = false;
            SetRect(
                targetPanel.GetComponent<RectTransform>(),
                new Vector2(0f, -24f),
                new Vector2(430f, 104f),
                new Vector2(0.5f, 1f));

            Text targetNameText = CreateText(
                "Text_TargetName",
                targetPanel.transform,
                "Молодой волк",
                22,
                TextAnchor.UpperLeft);
            targetNameText.color = new Color(1f, 0.78f, 0.58f);
            SetRect(
                targetNameText.GetComponent<RectTransform>(),
                new Vector2(16f, -10f),
                new Vector2(344f, 28f),
                new Vector2(0f, 1f));

            Button clearTargetButton = CreateButton("Button_ClearTarget", targetPanel.transform, "X");
            clearTargetButton.GetComponent<Image>().color = new Color(0.3f, 0.11f, 0.1f, 1f);
            SetRect(
                clearTargetButton.GetComponent<RectTransform>(),
                new Vector2(382f, -8f),
                new Vector2(32f, 30f),
                new Vector2(0f, 1f));

            Text targetHealthText = CreateText(
                "Text_TargetHealth",
                targetPanel.transform,
                "Здоровье: 45 / 45",
                18,
                TextAnchor.UpperLeft);
            targetHealthText.color = Color.white;
            SetRect(
                targetHealthText.GetComponent<RectTransform>(),
                new Vector2(16f, -42f),
                new Vector2(398f, 24f),
                new Vector2(0f, 1f));

            Image targetHealthFill = CreateHealthBar(
                "TargetHealthBar",
                targetPanel.transform,
                new Vector2(16f, -70f),
                new Vector2(398f, 18f),
                new Color(0.82f, 0.18f, 0.14f));

            CombatHudView hud = canvasObject.AddComponent<CombatHudView>();
            hud.Initialize(
                playerHealthText,
                playerHealthFill,
                targetNameText,
                targetHealthText,
                targetHealthFill,
                experienceText,
                targetPanel,
                clearTargetButton,
                player.GetComponent<Health>(),
                player.GetComponent<PlayerProgression>(),
                combatController,
                player.GetComponent<PlayerInteractionController>());
            targetPanel.SetActive(false);
        }

        private static Image CreateHealthBar(
            string name,
            Transform parent,
            Vector2 anchoredPosition,
            Vector2 size,
            Color fillColor)
        {
            GameObject background = CreatePanel(name, parent, new Color(0.02f, 0.025f, 0.03f, 1f));
            background.GetComponent<Image>().raycastTarget = false;
            SetRect(
                background.GetComponent<RectTransform>(),
                anchoredPosition,
                size,
                new Vector2(0f, 1f));

            GameObject fillObject = CreatePanel("Fill", background.transform, fillColor);
            Image fill = fillObject.GetComponent<Image>();
            fill.raycastTarget = false;
            fill.type = Image.Type.Simple;
            Stretch(fillObject.GetComponent<RectTransform>(), 2f, 2f, 2f, 2f);
            return fill;
        }

        private static InteractionPromptView CreateInteractionPrompt(Transform canvasTransform)
        {
            GameObject root = CreatePanel("InteractionPrompt", canvasTransform, new Color(0.05f, 0.06f, 0.07f, 0.82f));
            root.GetComponent<Image>().raycastTarget = false;
            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0f);
            rootRect.anchorMax = new Vector2(0.5f, 0f);
            rootRect.pivot = new Vector2(0.5f, 0f);
            rootRect.anchoredPosition = new Vector2(0f, 42f);
            rootRect.sizeDelta = new Vector2(420f, 56f);

            Text promptText = CreateText("Text_Prompt", root.transform, "Press E - Talk", 24, TextAnchor.MiddleCenter);
            promptText.color = new Color(1f, 0.95f, 0.82f);
            Stretch(promptText.GetComponent<RectTransform>(), 18f, 8f, 18f, 8f);

            InteractionPromptView promptView = root.AddComponent<InteractionPromptView>();
            promptView.Initialize(root, promptText);
            return promptView;
        }

        private static DialogueWindow CreateDialogueWindow(Transform canvasTransform)
        {
            GameObject root = CreatePanel("DialogueWindow", canvasTransform, new Color(0.06f, 0.07f, 0.08f, 0.94f));
            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0f, 0.5f);
            rootRect.anchorMax = new Vector2(0f, 0.5f);
            rootRect.pivot = new Vector2(0f, 0.5f);
            rootRect.anchoredPosition = new Vector2(28f, -20f);
            rootRect.sizeDelta = new Vector2(720f, 420f);

            Text speakerText = CreateText("Text_Speaker", root.transform, "Староста деревни", 30, TextAnchor.UpperLeft);
            speakerText.color = new Color(1f, 0.86f, 0.48f);
            SetRect(speakerText.GetComponent<RectTransform>(), new Vector2(28f, -24f), new Vector2(664f, 42f), new Vector2(0f, 1f));

            Text bodyText = CreateText("Text_Body", root.transform, "", 22, TextAnchor.UpperLeft);
            bodyText.color = new Color(0.94f, 0.94f, 0.9f);
            bodyText.lineSpacing = 1.12f;
            SetRect(bodyText.GetComponent<RectTransform>(), new Vector2(28f, -78f), new Vector2(664f, 118f), new Vector2(0f, 1f));

            Text questText = CreateText("Text_Quest", root.transform, "", 20, TextAnchor.UpperLeft);
            questText.color = new Color(0.77f, 0.91f, 1f);
            questText.lineSpacing = 1.08f;
            SetRect(questText.GetComponent<RectTransform>(), new Vector2(28f, -202f), new Vector2(664f, 122f), new Vector2(0f, 1f));

            Button acceptButton = CreateButton("Button_AcceptQuest", root.transform, "Принять поручение");
            SetRect(acceptButton.GetComponent<RectTransform>(), new Vector2(28f, 28f), new Vector2(300f, 58f), new Vector2(0f, 0f));

            Button closeButton = CreateButton("Button_CloseDialogue", root.transform, "Закрыть");
            SetRect(closeButton.GetComponent<RectTransform>(), new Vector2(348f, 28f), new Vector2(180f, 58f), new Vector2(0f, 0f));

            DialogueWindow dialogueWindow = root.AddComponent<DialogueWindow>();
            dialogueWindow.Initialize(root, speakerText, bodyText, questText, acceptButton, closeButton);
            return dialogueWindow;
        }

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            GameObject panel = new(name);
            panel.transform.SetParent(parent, false);
            Image image = panel.AddComponent<Image>();
            image.color = color;
            return panel;
        }

        private static Text CreateText(string name, Transform parent, string value, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new(name);
            textObject.transform.SetParent(parent, false);
            Text text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label)
        {
            GameObject buttonObject = CreatePanel(name, parent, new Color(0.16f, 0.25f, 0.34f, 1f));
            Button button = buttonObject.AddComponent<Button>();
            Text labelText = CreateText("Text_Label", buttonObject.transform, label, 20, TextAnchor.MiddleCenter);
            labelText.color = Color.white;
            Stretch(labelText.GetComponent<RectTransform>(), 8f, 4f, 8f, 4f);
            return button;
        }

        private static InputField CreateInputField(string name, Transform parent, string placeholderValue)
        {
            GameObject inputObject = CreatePanel(name, parent, new Color(0.08f, 0.1f, 0.12f, 1f));
            InputField input = inputObject.AddComponent<InputField>();
            input.targetGraphic = inputObject.GetComponent<Image>();
            input.lineType = InputField.LineType.SingleLine;

            Text valueText = CreateText(
                "Text_Value",
                inputObject.transform,
                string.Empty,
                23,
                TextAnchor.MiddleLeft);
            valueText.color = Color.white;
            valueText.horizontalOverflow = HorizontalWrapMode.Overflow;
            Stretch(valueText.GetComponent<RectTransform>(), 16f, 6f, 16f, 6f);

            Text placeholder = CreateText(
                "Text_Placeholder",
                inputObject.transform,
                placeholderValue,
                23,
                TextAnchor.MiddleLeft);
            placeholder.color = new Color(0.55f, 0.6f, 0.64f);
            Stretch(placeholder.GetComponent<RectTransform>(), 16f, 6f, 16f, 6f);

            input.textComponent = valueText;
            input.placeholder = placeholder;
            return input;
        }

        private static void CreateWindowDragHandle(GameObject window, float handleWidth)
        {
            GameObject handle = CreatePanel("DragHandle", window.transform, Color.clear);
            SetRect(
                handle.GetComponent<RectTransform>(),
                Vector2.zero,
                new Vector2(handleWidth, 64f),
                new Vector2(0f, 1f));

            DraggableWindow draggable = handle.AddComponent<DraggableWindow>();
            draggable.Initialize(window.GetComponent<RectTransform>());
            handle.transform.SetAsFirstSibling();
        }

        private static void SetRect(RectTransform rect, Vector2 anchoredPosition, Vector2 size, Vector2 anchor)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        private static void Stretch(RectTransform rect, float left, float top, float right, float bottom)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        private static void CreateLighting()
        {
            GameObject lightObject = new("Directional Light");
            lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.45f, 0.48f, 0.52f);
        }

        private static void CreateEventSystem()
        {
            GameObject eventSystemObject = new("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static void AddRuntimeNavMesh(GameObject navigationRoot)
        {
            NavMeshSurface surface = navigationRoot.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.defaultArea = 0;
            navigationRoot.AddComponent<StarterVillageRuntimeBuilder>();
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
        }

        private readonly struct FirstContactUi
        {
            public FirstContactUi(DialogueWindow dialogueWindow, InteractionPromptView promptView)
            {
                DialogueWindow = dialogueWindow;
                PromptView = promptView;
            }

            public DialogueWindow DialogueWindow { get; }
            public InteractionPromptView PromptView { get; }
        }
    }
}
