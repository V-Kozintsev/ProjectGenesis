using System;
using System.Linq;
using ProjectGenesis.Gameplay;
using ProjectGenesis.Saving;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.Tools.Editor
{
    public static class CharacterEntryDevelopmentValidator
    {
        private const string ScenePath = "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";

        [MenuItem("Project Genesis/Sprint 017/Validate Character Entry")]
        public static void ValidateCharacterEntry()
        {
            ValidateProfileLifecycle();
            ValidateNameRules();
            ValidatePlayerPrefab();
            ValidateScene();
            Debug.Log("Sprint 017 character entry validation passed.");
        }

        private static void ValidateProfileLifecycle()
        {
            Require(PlayerProfileData.CurrentVersion == 4,
                "Current profile version must be 4.");
            Require(LocalJsonPlayerPersistence.IsSupportedVersion(1),
                "Version-1 profiles must remain supported.");
            Require(LocalJsonPlayerPersistence.IsSupportedVersion(2),
                "Version-2 profiles must remain supported.");
            Require(LocalJsonPlayerPersistence.IsSupportedVersion(3),
                "Version-3 profiles must be supported.");
            Require(LocalJsonPlayerPersistence.IsSupportedVersion(4),
                "Version-4 item-instance profiles must be supported.");
            Require(!LocalJsonPlayerPersistence.IsSupportedVersion(5),
                "Unknown future profile versions must be rejected.");

            Require(PlayerPersistenceController.ResolveHasCreatedCharacter(
                    new PlayerProfileData { Version = 1 }),
                "A version-1 profile must be treated as an existing character.");
            Require(PlayerPersistenceController.ResolveHasCreatedCharacter(
                    new PlayerProfileData { Version = 2 }),
                "A version-2 profile must be treated as an existing character.");
            Require(!PlayerPersistenceController.ResolveHasCreatedCharacter(
                    new PlayerProfileData { Version = 3, HasCreatedCharacter = false }),
                "A new version-3 profile must remain in creation state.");
            Require(PlayerPersistenceController.ResolveHasCreatedCharacter(
                    new PlayerProfileData { Version = 3, HasCreatedCharacter = true }),
                "A created version-3 profile must enter selection state.");
            Require(!PlayerPersistenceController.ResolveHasCreatedCharacter(null),
                "A missing profile must not be treated as a created character.");
        }

        private static void ValidateNameRules()
        {
            Require(!PlayerIdentity.IsAcceptableName(null),
                "A missing character name must be rejected.");
            Require(!PlayerIdentity.IsAcceptableName("   "),
                "A blank character name must be rejected.");
            Require(PlayerIdentity.IsAcceptableName("  Арден  "),
                "A valid trimmed character name must be accepted.");
            Require(!PlayerIdentity.IsAcceptableName(new string('A', 25)),
                "A character name longer than 24 characters must be rejected.");
        }

        private static void ValidatePlayerPrefab()
        {
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            Require(playerPrefab != null, "Player prefab was not found.");
            Require(playerPrefab.GetComponent<PlayerIdentity>() != null,
                "Player prefab must preserve PlayerIdentity.");
            Require(playerPrefab.GetComponent<PlayerPersistenceController>() != null,
                "Player prefab must preserve PlayerPersistenceController.");
            Require(playerPrefab.GetComponent<PlayerSkillController>() != null,
                "Player prefab must preserve PlayerSkillController.");
        }

        private static void ValidateScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            CharacterEntryView[] entries =
                UnityEngine.Object.FindObjectsByType<CharacterEntryView>(FindObjectsSortMode.None);
            Require(entries.Length == 1,
                "Starter scene must contain exactly one character entry view.");

            CharacterEntryView entry = entries.Single();
            Require(entry.Root != null && entry.Persistence != null,
                "Character entry view references are incomplete.");
            Require(entry.GameplayBehaviourCount >= 7,
                "Character entry must gate the expected gameplay and UI behaviours.");
            Require(entry.Root.transform.GetSiblingIndex() ==
                    entry.Root.transform.parent.childCount - 1,
                "Character entry overlay must remain the top Canvas child.");

            InputField[] inputs = entry.Root.GetComponentsInChildren<InputField>(true);
            Require(inputs.Length == 1,
                "Character entry must contain exactly one name input.");
            Require(inputs[0].characterLimit == PlayerIdentity.MaximumNameLength,
                "Character name input must enforce the 24-character limit.");
            Require(FindChild(entry.Root.transform, "Button_CreateCharacter") != null,
                "Character creation button is missing.");
            Require(FindChild(entry.Root.transform, "Button_EnterCharacter") != null,
                "Character selection play button is missing.");

            Require(UnityEngine.Object.FindObjectsByType<CharacterIdentityView>(
                    FindObjectsSortMode.None).Length == 1,
                "Starter scene must preserve the Sprint 016 identity view.");
            Require(UnityEngine.Object.FindObjectsByType<SkillHotbarView>(
                    FindObjectsSortMode.None).Length == 1,
                "Starter scene must preserve the Sprint 015 skill hotbar.");
            Require(UnityEngine.Object.FindObjectsByType<EnemySpawner>(
                    FindObjectsSortMode.None).Length == 3,
                "Starter scene must preserve all three enemy spawners.");
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

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"Sprint 017 validation failed: {message}");
            }
        }
    }
}
