using System;
using System.Linq;
using System.Reflection;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class EliteEncounterDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string AlphaPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Enemies/PF_Enemy_WolfAlpha.prefab";
        private const string AlphaLootTablePath =
            "Assets/ProjectGenesis/Data/LootTables/LT_WolfAlpha.asset";
        private const string WornAxePath =
            "Assets/ProjectGenesis/Data/Items/ITM_WornAxe.asset";
        private const string WornArmorPath =
            "Assets/ProjectGenesis/Data/Items/ITM_WornLeatherArmor.asset";
        private const string PotionPath =
            "Assets/ProjectGenesis/Data/Items/ITM_MinorHealingPotion.asset";
        private const string NorthWildsZonePath =
            "Assets/ProjectGenesis/Data/Zones/SO_Zone_NorthWilds.asset";

        [MenuItem("Project Genesis/Sprint 028/Validate Elite Encounter")]
        public static void ValidateEliteEncounter()
        {
            GameObject alphaPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(AlphaPrefabPath);
            LootTableDefinition lootTable =
                AssetDatabase.LoadAssetAtPath<LootTableDefinition>(AlphaLootTablePath);
            WorldZoneDefinition northWilds =
                AssetDatabase.LoadAssetAtPath<WorldZoneDefinition>(NorthWildsZonePath);

            ValidateAlphaPrefab(alphaPrefab);
            ValidateLootTable(lootTable);
            ValidateSpawnerRespawnScheduling(alphaPrefab);
            ValidateTelegraphedAttackRuntime();
            ValidateScene(alphaPrefab, northWilds);
            Debug.Log("Sprint 028 elite encounter validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 028/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateEliteEncounter();
            ZoneRulesDevelopmentValidator.ValidateRelevantRegressionSuite();
        }

        private static void ValidateAlphaPrefab(GameObject alphaPrefab)
        {
            Require(alphaPrefab != null, "Wolf Alpha prefab is missing.");
            EnemyBrain brain = alphaPrefab.GetComponent<EnemyBrain>();
            Health health = alphaPrefab.GetComponent<Health>();
            CombatStats stats = alphaPrefab.GetComponent<CombatStats>();
            TelegraphedEnemyAttack special =
                alphaPrefab.GetComponent<TelegraphedEnemyAttack>();
            EnemyLootDrop lootDrop = alphaPrefab.GetComponent<EnemyLootDrop>();

            Require(brain != null && brain.Rank == EnemyRank.Elite,
                "Wolf Alpha must use the Elite enemy rank.");
            Require(brain.DisplayName == "Вожак стаи" &&
                    brain.EnemyLevel == 3 &&
                    brain.ExperienceReward == 90 &&
                    brain.QuestTargetId == "wolf_alpha",
                "Wolf Alpha identity or progression rewards are unexpected.");
            Require(health != null && health.MaximumHealth == 160,
                "Wolf Alpha must have 160 maximum health.");
            Require(stats != null &&
                    stats.BaseAttackPower == 13 &&
                    stats.Defense == 4 &&
                    Mathf.Approximately(stats.AttackRange, 1.2f) &&
                    Mathf.Approximately(stats.AttackInterval, 1.35f),
                "Wolf Alpha combat stats are unexpected.");
            Require(special != null && special.IsValid &&
                    special.AttackName == "Мощный укус" &&
                    Mathf.Approximately(special.InitialDelay, 2.5f) &&
                    Mathf.Approximately(special.Cooldown, 6f) &&
                    Mathf.Approximately(special.WindupDuration, 1.25f) &&
                    Mathf.Approximately(special.HitRange, 1.65f) &&
                    Mathf.Approximately(special.AttackPowerMultiplier, 2f),
                "Wolf Alpha telegraphed attack data is invalid.");
            Require(special.CalculateDamageAgainst(null) == 26,
                "Wolf Alpha special must scale its authored attack power.");
            Require(lootDrop != null && lootDrop.LootTable != null &&
                    string.IsNullOrEmpty(lootDrop.QuestObjectiveTargetId),
                "Wolf Alpha must use regular loot without a quest trophy.");
        }

        private static void ValidateLootTable(LootTableDefinition lootTable)
        {
            ItemDefinition axe = AssetDatabase.LoadAssetAtPath<ItemDefinition>(WornAxePath);
            ItemDefinition armor = AssetDatabase.LoadAssetAtPath<ItemDefinition>(WornArmorPath);
            ItemDefinition potion = AssetDatabase.LoadAssetAtPath<ItemDefinition>(PotionPath);

            Require(lootTable != null && lootTable.TryValidate(out _) &&
                    lootTable.Entries.Count == 3 &&
                    Mathf.Approximately(lootTable.TotalDropChance, 1f),
                "Wolf Alpha loot table must contain one guaranteed reward.");
            Require(HasEntry(lootTable, axe, LootRarity.Uncommon, 0.5f),
                "Wolf Alpha loot must contain the worn axe at 50%.");
            Require(HasEntry(lootTable, armor, LootRarity.Uncommon, 0.3f),
                "Wolf Alpha loot must contain worn armor at 30%.");
            Require(HasEntry(lootTable, potion, LootRarity.Common, 0.2f),
                "Wolf Alpha loot must contain a healing potion at 20%.");
        }

        private static void ValidateTelegraphedAttackRuntime()
        {
            GameObject attacker = new("EliteAttackProbe");
            GameObject target = new("EliteAttackTarget");
            try
            {
                CombatStats attackerStats = attacker.AddComponent<CombatStats>();
                attackerStats.Configure(13, 4, 1.2f, 1.35f);
                GameObject warningPart =
                    GameObject.CreatePrimitive(PrimitiveType.Cube);
                warningPart.transform.SetParent(attacker.transform, false);
                UnityEngine.Object.DestroyImmediate(
                    warningPart.GetComponent<Collider>());
                TelegraphedEnemyAttack special =
                    attacker.AddComponent<TelegraphedEnemyAttack>();
                special.Configure(
                    "Мощный укус",
                    2.5f,
                    6f,
                    1.25f,
                    1.65f,
                    2f,
                    Color.red,
                    new[] { warningPart.GetComponent<Renderer>() });

                Health targetHealth = target.AddComponent<Health>();
                targetHealth.Configure(100);
                CombatStats targetStats = target.AddComponent<CombatStats>();
                targetStats.Configure(10, 3, 1f, 1f);
                LocalMessageStream messages =
                    target.AddComponent<LocalMessageStream>();

                special.ResetCycle(0f);
                Require(!special.UpdateAttack(
                        2.49f, 1f, targetHealth, targetStats, messages, "Вожак стаи"),
                    "Special attack must respect its initial delay.");
                Require(special.UpdateAttack(
                        2.5f, 1f, targetHealth, targetStats, messages, "Вожак стаи") &&
                        special.IsWindingUp,
                    "Special attack must enter a readable windup.");
                Require(special.UpdateAttack(
                        3.75f, 1f, targetHealth, targetStats, messages, "Вожак стаи") &&
                        targetHealth.CurrentHealth == 77 &&
                        messages.Entries.Any(entry => entry.Text.Contains("23 урона")),
                    "Resolved special attack must deal 23 defense-aware damage.");

                special.ResetCycle(10f);
                special.UpdateAttack(
                    12.5f, 1f, targetHealth, targetStats, messages, "Вожак стаи");
                special.UpdateAttack(
                    13.75f, 5f, targetHealth, targetStats, messages, "Вожак стаи");
                Require(targetHealth.CurrentHealth == 77 &&
                        messages.Entries.Any(entry => entry.Text.Contains("избежали")),
                    "Leaving the hit range during windup must avoid all damage.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(target);
                UnityEngine.Object.DestroyImmediate(attacker);
            }
        }

        private static void ValidateSpawnerRespawnScheduling(GameObject alphaPrefab)
        {
            GameObject spawnerObject = new("AlphaRespawnProbeSpawner");
            GameObject player = new("AlphaRespawnProbePlayer");
            EnemyBrain firstEnemy = null;
            try
            {
                EnemySpawner spawner = spawnerObject.AddComponent<EnemySpawner>();
                spawner.Configure(alphaPrefab, player.transform, 1f);

                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                MethodInfo spawnMethod = typeof(EnemySpawner).GetMethod("SpawnEnemy", flags);
                MethodInfo deathMethod = typeof(EnemySpawner).GetMethod("HandleEnemyDied", flags);
                FieldInfo readyTimeField = typeof(EnemySpawner).GetField("respawnReadyTime", flags);

                Require(spawnMethod != null &&
                        deathMethod != null &&
                        readyTimeField != null,
                    "Enemy spawner runtime hooks changed without validator coverage.");

                spawnMethod.Invoke(spawner, null);
                firstEnemy = spawner.CurrentEnemy;
                Require(firstEnemy != null && firstEnemy.Rank == EnemyRank.Elite,
                    "Spawner must create the authored elite enemy.");

                deathMethod.Invoke(spawner, new object[] { firstEnemy });
                Require(spawner.CurrentEnemy == null && spawner.IsRespawnPending,
                    "Spawner must schedule a respawn when the elite enemy dies.");

                readyTimeField.SetValue(spawner, Time.time + 0.5f);
                Require(spawner.IsRespawnPending,
                    "Spawner must keep the respawn pending until its tick consumes the timer.");
            }
            finally
            {
                if (firstEnemy != null)
                {
                    UnityEngine.Object.DestroyImmediate(firstEnemy.gameObject);
                }

                UnityEngine.Object.DestroyImmediate(player);
                UnityEngine.Object.DestroyImmediate(spawnerObject);
            }
        }

        private static void ValidateScene(
            GameObject alphaPrefab,
            WorldZoneDefinition northWilds)
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            EnemySpawner[] spawners =
                UnityEngine.Object.FindObjectsByType<EnemySpawner>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            Require(spawners.Length == 4,
                "Starter scene must contain three regular spawners and one elite spawner.");

            EnemySpawner alphaSpawner = spawners.SingleOrDefault(
                spawner => spawner.EnemyPrefab == alphaPrefab);
            Require(alphaSpawner != null &&
                    alphaSpawner.name == "WolfAlphaSpawn_Clearing" &&
                    Mathf.Approximately(alphaSpawner.RespawnDelay, 45f),
                "Wolf Alpha must use its authored clearing spawner and slow respawn.");

            GameObject clearing = GameObject.Find("EliteClearing");
            GameObject clearingZone = GameObject.Find("Zone_EliteClearing");
            EnemyTerritory eliteTerritory =
                clearingZone != null ? clearingZone.GetComponent<EnemyTerritory>() : null;
            WorldZoneVolume eliteVolume =
                clearingZone != null ? clearingZone.GetComponent<WorldZoneVolume>() : null;
            Require(clearing != null &&
                    alphaSpawner.transform.IsChildOf(clearing.transform),
                "Elite clearing must own its encounter objects.");
            Require(eliteTerritory != null &&
                    eliteTerritory != GameObject.Find("Zone_NorthCombat")
                        ?.GetComponent<EnemyTerritory>() &&
                    alphaSpawner.Territory == eliteTerritory &&
                    eliteTerritory.Contains(alphaSpawner.transform.position),
                "Wolf Alpha must use a separate territory containing its spawn.");
            Require(eliteVolume != null &&
                    eliteVolume.Definition == northWilds &&
                    eliteVolume.Contains(alphaSpawner.transform.position),
                "Elite clearing must remain part of the north-wilds combat zone.");

            Require(GameObject.Find("Boundary_CombatEast") == null &&
                    GameObject.Find("Boundary_CombatEast_South") != null &&
                    GameObject.Find("Boundary_CombatEast_North") != null &&
                    GameObject.Find("Boundary_EliteEast") != null,
                "East opening and elite clearing boundaries are incomplete.");

            PlayerZoneController controller =
                UnityEngine.Object.FindFirstObjectByType<PlayerZoneController>(
                    FindObjectsInactive.Include);
            WorldZoneVolume[] allVolumes =
                UnityEngine.Object.FindObjectsByType<WorldZoneVolume>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            Require(controller != null &&
                    controller.ZoneVolumes.Length == allVolumes.Length &&
                    allVolumes.All(volume => controller.ZoneVolumes.Contains(volume)),
                "Player zone controller must reference the elite clearing volume.");
        }

        private static bool HasEntry(
            LootTableDefinition table,
            ItemDefinition item,
            LootRarity rarity,
            float chance)
        {
            return item != null && table.Entries.Any(
                entry => entry != null &&
                         entry.Item == item &&
                         entry.Rarity == rarity &&
                         Mathf.Approximately(entry.DropChance, chance));
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 028 validation failed: {message}");
            }
        }
    }
}
