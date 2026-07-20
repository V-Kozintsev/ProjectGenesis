using System;
using System.Linq;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using ProjectGenesis.Saving;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class CharacterIdentityDevelopmentValidator
    {
        private const string ScenePath = "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string HumanRacePath =
            "Assets/ProjectGenesis/Data/Races/SO_Race_Human.asset";
        private const string WarriorClassPath =
            "Assets/ProjectGenesis/Data/Classes/SO_Class_Warrior.asset";

        [MenuItem("Project Genesis/Sprint 016/Validate Character Identity")]
        public static void ValidateCharacterIdentity()
        {
            CharacterRaceDefinition human = LoadRequiredAsset<CharacterRaceDefinition>(HumanRacePath);
            CharacterClassDefinition warrior =
                LoadRequiredAsset<CharacterClassDefinition>(WarriorClassPath);

            ValidateDefinitions(human, warrior);
            ValidatePlayerPrefab(human, warrior);
            ValidateProfileCompatibility();
            ValidateIdentityFallbacks(human, warrior);
            ValidateScene();
            Debug.Log("Sprint 016 character identity validation passed.");
        }

        private static void ValidateDefinitions(
            CharacterRaceDefinition human,
            CharacterClassDefinition warrior)
        {
            Require(human.IsValid, "Human race data is invalid.");
            Require(human.RaceId == "race.human", "Human race id is unexpected.");
            Require(human.DisplayName == "Человек", "Human race display name is unexpected.");
            Require(warrior.IsValid, "Warrior class data is invalid.");
            Require(warrior.ClassId == "class.warrior", "Warrior class id is unexpected.");
            Require(warrior.DisplayName == "Воин", "Warrior class display name is unexpected.");
        }

        private static void ValidatePlayerPrefab(
            CharacterRaceDefinition human,
            CharacterClassDefinition warrior)
        {
            GameObject playerPrefab = LoadRequiredAsset<GameObject>(PlayerPrefabPath);
            PlayerIdentity identity = playerPrefab.GetComponent<PlayerIdentity>();
            Require(identity != null, "Player prefab is missing PlayerIdentity.");
            Require(identity.IsValid, "Player prefab identity is invalid.");
            Require(identity.CharacterName == PlayerIdentity.DefaultCharacterName,
                "Player prefab must use the prototype default name.");
            Require(identity.Race == human, "Player prefab must reference the human race.");
            Require(identity.CharacterClass == warrior,
                "Player prefab must reference the warrior class.");

            PlayerPersistenceController persistence =
                playerPrefab.GetComponent<PlayerPersistenceController>();
            Require(persistence != null, "Player prefab is missing persistence.");
            Require(persistence.RaceCatalog != null && persistence.RaceCatalog.Count == 1,
                "Persistence must contain exactly one prototype race.");
            Require(persistence.RaceCatalog[0] == human,
                "Persistence race catalog must contain the human race.");
            Require(persistence.ClassCatalog != null && persistence.ClassCatalog.Count == 1,
                "Persistence must contain exactly one prototype class.");
            Require(persistence.ClassCatalog[0] == warrior,
                "Persistence class catalog must contain the warrior class.");
        }

        private static void ValidateProfileCompatibility()
        {
            Require(PlayerProfileData.CurrentVersion == 2,
                "Current local profile version must be 2.");
            Require(LocalJsonPlayerPersistence.IsSupportedVersion(1),
                "Existing version-1 profiles must remain supported.");
            Require(LocalJsonPlayerPersistence.IsSupportedVersion(2),
                "Current version-2 profiles must be supported.");
            Require(!LocalJsonPlayerPersistence.IsSupportedVersion(0),
                "Invalid version 0 must be rejected.");
            Require(!LocalJsonPlayerPersistence.IsSupportedVersion(3),
                "Unknown future profile versions must be rejected.");

            PlayerProfileData legacyProfile = JsonUtility.FromJson<PlayerProfileData>(
                "{\"Version\":1,\"Level\":3,\"CurrentExperience\":25}");
            Require(legacyProfile != null && legacyProfile.Version == 1,
                "A version-1 JSON profile must still deserialize.");
            Require(legacyProfile.Level == 3 && legacyProfile.CurrentExperience == 25,
                "Version-1 progression values changed during deserialization.");
            Require(string.IsNullOrEmpty(legacyProfile.CharacterName) &&
                    string.IsNullOrEmpty(legacyProfile.RaceId) &&
                    string.IsNullOrEmpty(legacyProfile.ClassId),
                "Version-1 profiles must expose missing identity fields for prefab fallback.");
        }

        private static void ValidateIdentityFallbacks(
            CharacterRaceDefinition human,
            CharacterClassDefinition warrior)
        {
            Require(PlayerIdentity.NormalizeName("  Арден  ") == "Арден",
                "Character names must be trimmed.");
            Require(PlayerIdentity.NormalizeName(" ") == PlayerIdentity.DefaultCharacterName,
                "Blank character names must use the default.");
            Require(PlayerIdentity.NormalizeName(new string('A', 30)).Length ==
                    PlayerIdentity.MaximumNameLength,
                "Long character names must be bounded.");

            GameObject probe = new("CharacterIdentityValidation");
            try
            {
                PlayerIdentity identity = probe.AddComponent<PlayerIdentity>();
                identity.Configure(PlayerIdentity.DefaultCharacterName, human, warrior);
                identity.RestoreIdentity(null, null, null);
                Require(identity.IsValid,
                    "Version-1 identity fallback must preserve valid prefab defaults.");
                Require(identity.Race == human && identity.CharacterClass == warrior,
                    "Missing saved ids must preserve the authored race and class.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }
        }

        private static void ValidateScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            CharacterIdentityView[] identityViews =
                UnityEngine.Object.FindObjectsByType<CharacterIdentityView>(FindObjectsSortMode.None);
            Require(identityViews.Length == 1,
                "Starter scene must contain exactly one character identity view.");
            Require(identityViews.Single().Identity != null,
                "Character identity view must reference the player identity.");

            SkillHotbarView[] hotbars =
                UnityEngine.Object.FindObjectsByType<SkillHotbarView>(FindObjectsSortMode.None);
            Require(hotbars.Length == 1,
                "Starter scene must preserve the Sprint 015 skill hotbar.");
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
                throw new InvalidOperationException($"Sprint 016 validation failed: {message}");
            }
        }
    }
}
