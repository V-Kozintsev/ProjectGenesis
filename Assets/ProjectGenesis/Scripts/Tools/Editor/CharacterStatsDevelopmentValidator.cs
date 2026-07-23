using System;
using System.Linq;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class CharacterStatsDevelopmentValidator
    {
        private const string ScenePath = "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string WarriorClassPath =
            "Assets/ProjectGenesis/Data/Classes/SO_Class_Warrior.asset";
        private const string HeavyStrikePath =
            "Assets/ProjectGenesis/Data/Skills/SO_Skill_HeavyStrike.asset";

        [MenuItem("Project Genesis/Sprint 018/Validate Character Stats")]
        public static void ValidateCharacterStats()
        {
            CharacterClassDefinition warrior =
                LoadRequiredAsset<CharacterClassDefinition>(WarriorClassPath);
            SkillDefinition heavyStrike = LoadRequiredAsset<SkillDefinition>(HeavyStrikePath);

            ValidateAuthoredData(warrior, heavyStrike);
            ValidateStatComposition(heavyStrike);
            ValidatePlayerPrefab(warrior, heavyStrike);
            ValidateScene();
            Debug.Log("Sprint 018 character stats validation passed.");
        }

        private static void ValidateAuthoredData(
            CharacterClassDefinition warrior,
            SkillDefinition heavyStrike)
        {
            Require(warrior.IsValid, "Warrior class data is invalid.");
            Require(warrior.ClassId == "class.warrior", "Warrior class id is unexpected.");
            Require(warrior.MaximumHealthBonus == 10,
                "Warrior must contribute 10 maximum health.");
            Require(warrior.AttackPowerBonus == 2,
                "Warrior must contribute 2 attack power.");

            Require(heavyStrike.IsValid, "Heavy Strike data is invalid.");
            Require(!string.IsNullOrWhiteSpace(heavyStrike.Description),
                "Heavy Strike must have a player-facing description.");
            Require(Mathf.Approximately(heavyStrike.AttackPowerMultiplier, 1.7f),
                "Heavy Strike must scale from 170% of current attack power.");
            Require(Mathf.Approximately(heavyStrike.Range, 1.55f),
                "Heavy Strike range changed unexpectedly.");
            Require(Mathf.Approximately(heavyStrike.Cooldown, 4f),
                "Heavy Strike cooldown changed unexpectedly.");
        }

        private static void ValidateStatComposition(SkillDefinition heavyStrike)
        {
            Require(PlayerProgression.CalculateMaximumHealth(90, 10, 1, 10) == 100,
                "Level-1 maximum health must be 90 base plus 10 warrior health.");
            Require(PlayerProgression.CalculateMaximumHealth(90, 10, 2, 10) == 110,
                "Level-2 maximum health must preserve the 10-point level growth.");

            GameObject attackerObject = new("CharacterStatsAttackerValidation");
            GameObject targetObject = new("CharacterStatsTargetValidation");

            try
            {
                CombatStats attacker = attackerObject.AddComponent<CombatStats>();
                attacker.Configure(12, 3, 1.35f, 0.8f);
                attacker.SetClassAttackBonus(2);

                CombatStats target = targetObject.AddComponent<CombatStats>();
                target.Configure(1, 2, 1f, 1f);

                Require(attacker.AttackPower == 14,
                    "Level-1 unarmed attack must be 12 base plus 2 warrior attack.");
                Require(attacker.CalculateDamageAgainst(target) == 12,
                    "Level-1 unarmed basic damage against 2 defense must be 12.");
                Require(attacker.CalculateScaledDamageAgainst(
                        target,
                        heavyStrike.AttackPowerMultiplier) == 22,
                    "Level-1 unarmed Heavy Strike against 2 defense must be 22.");

                attacker.SetEquipmentAttackBonus(4);
                Require(attacker.AttackPower == 18,
                    "The Rusty Sword must raise level-1 attack from 14 to 18.");
                Require(attacker.CalculateDamageAgainst(target) == 16,
                    "Sword basic damage against 2 defense must be 16.");
                Require(attacker.CalculateScaledDamageAgainst(
                        target,
                        heavyStrike.AttackPowerMultiplier) == 29,
                    "The Rusty Sword must raise Heavy Strike damage from 22 to 29.");

                attacker.SetProgressionAttackBonus(2);
                Require(attacker.AttackPower == 20,
                    "Level 2 with the Rusty Sword must have 20 attack power.");
                Require(attacker.CalculateScaledDamageAgainst(
                        target,
                        heavyStrike.AttackPowerMultiplier) == 32,
                    "Level-2 sword Heavy Strike against 2 defense must be 32.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(attackerObject);
                UnityEngine.Object.DestroyImmediate(targetObject);
            }
        }

        private static void ValidatePlayerPrefab(
            CharacterClassDefinition warrior,
            SkillDefinition heavyStrike)
        {
            GameObject playerPrefab = LoadRequiredAsset<GameObject>(PlayerPrefabPath);
            Health health = playerPrefab.GetComponent<Health>();
            CombatStats stats = playerPrefab.GetComponent<CombatStats>();
            PlayerProgression progression = playerPrefab.GetComponent<PlayerProgression>();
            PlayerIdentity identity = playerPrefab.GetComponent<PlayerIdentity>();
            PlayerSkillController skills = playerPrefab.GetComponent<PlayerSkillController>();

            Require(health != null && health.MaximumHealth == 90,
                "Player prefab base maximum health must be 90 before class bonuses.");
            Require(stats != null && stats.BaseAttackPower == 12 && stats.Defense == 3,
                "Player prefab base combat stats must remain 12 attack and 3 defense.");
            Require(progression != null && progression.BaseMaximumHealth == 90,
                "Player progression must use 90 base maximum health.");
            Require(progression.HealthPerLevel == 10 && progression.AttackPowerPerLevel == 2,
                "Player level growth must remain 10 health and 2 attack.");
            Require(identity != null && identity.CharacterClass == warrior,
                "Player prefab must keep the warrior class.");
            Require(skills != null && skills.GetSkill(0) == heavyStrike,
                "Player quick slot 1 must keep Heavy Strike.");
        }

        private static void ValidateScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            CharacterStatsView[] views =
                UnityEngine.Object.FindObjectsByType<CharacterStatsView>(FindObjectsSortMode.None);
            Require(views.Length == 1,
                "Starter scene must contain exactly one character stats view.");

            CharacterStatsView view = views.Single();
            Require(view.WindowRoot != null && view.CombatStats != null && view.Progression != null,
                "Character stats view references are incomplete.");
            Require(FindChild(view.transform, "Button_OpenCharacterStats") != null,
                "Character stats open button is missing.");
            Require(FindChild(view.WindowRoot.transform, "Button_CloseCharacterStats") != null,
                "Character stats close button is missing.");
            Require(FindChild(view.WindowRoot.transform, "Text_StatsAttackBreakdown") != null,
                "Character stats attack breakdown is missing.");
            Require(FindChild(view.WindowRoot.transform, "Text_StatsHeavyStrike") != null,
                "Character stats Heavy Strike power is missing.");

            CharacterEntryView entry =
                UnityEngine.Object.FindFirstObjectByType<CharacterEntryView>();
            Require(entry != null && entry.GameplayBehaviourCount >= 9,
                "Character entry must gate the new stats view before gameplay.");
            Require(UnityEngine.Object.FindObjectsByType<SkillHotbarView>(
                    FindObjectsSortMode.None).Length == 1,
                "Starter scene must preserve the skill hotbar.");

            DraggableWindow[] draggableWindows =
                UnityEngine.Object.FindObjectsByType<DraggableWindow>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            Require(draggableWindows.Length >= 3,
                "Stats, inventory, and quest windows must remain draggable.");
            string[] draggableNames = draggableWindows
                .Select(draggable => draggable.TargetWindow != null
                    ? draggable.TargetWindow.name
                    : string.Empty)
                .OrderBy(name => name)
                .ToArray();
            Require(draggableNames.Contains("CharacterStatsWindow") &&
                    draggableNames.Contains("InventoryWindow") &&
                    draggableNames.Contains("QuestJournalWindow"),
                "Required draggable window targets are incomplete.");

            SkillTooltipView[] tooltips =
                UnityEngine.Object.FindObjectsByType<SkillTooltipView>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            Require(tooltips.Length == 1 && tooltips[0].TooltipRoot != null,
                "Heavy Strike must have exactly one configured tooltip.");
            Require(tooltips[0].SlotIndex == 0,
                "Heavy Strike tooltip must describe quick slot 1.");
            Require(UnityEngine.Object.FindObjectsByType<EnemySpawner>(
                    FindObjectsSortMode.None).Length >= 3,
                "Starter scene must preserve its three regular enemy spawners.");
        }

        private static Transform FindChild(Transform parent, string childName)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == childName)
                {
                    return child;
                }
            }

            return null;
        }

        private static T LoadRequiredAsset<T>(string path)
            where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            Require(asset != null, $"Required asset was not found at '{path}'.");
            return asset;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"Sprint 018 validation failed: {message}");
            }
        }
    }
}
