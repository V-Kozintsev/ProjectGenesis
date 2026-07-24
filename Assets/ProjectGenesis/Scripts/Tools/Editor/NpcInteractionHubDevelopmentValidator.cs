using System;
using ProjectGenesis.Gameplay;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class NpcInteractionHubDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";

        [MenuItem("Project Genesis/Sprint 033/Validate NPC Interaction Hub")]
        public static void ValidateNpcInteractionHub()
        {
            ValidateSceneWiring();
            Debug.Log("Sprint 033 NPC interaction hub validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 033/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateNpcInteractionHub();
            MerchantShopDevelopmentValidator.ValidateRelevantRegressionSuite();
            Debug.Log("Sprint 033 relevant regression suite passed.");
        }

        private static void ValidateSceneWiring()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            NpcInteractionView hub = UnityEngine.Object.FindFirstObjectByType<NpcInteractionView>(
                FindObjectsInactive.Include);
            Require(hub != null, "NPC interaction hub is missing from the scene.");
            Require(hub.QuestButton != null, "NPC interaction hub must have a quest button.");
            Require(hub.TradeButton != null, "NPC interaction hub must have a trade button.");

            GameObject player = GameObject.Find("Player");
            Require(player != null, "Player is missing from the starter village scene.");
            PlayerInteractionController interaction =
                player.GetComponent<PlayerInteractionController>();
            Require(interaction != null, "Player interaction controller is missing.");
            Require(interaction.NpcInteractionView == hub,
                "Player interaction controller must use the NPC interaction hub.");

            InteractableNpc elder = GameObject.Find("NPC_VillageElder")
                ?.GetComponent<InteractableNpc>();
            Require(elder != null && elder.ResolveQuestDefinition(
                    player.GetComponent<QuestLog>()) != null,
                "Village Elder must still expose a quest through the hub.");

            InteractableNpc merchant = GameObject.Find("NPC_VillageMerchant")
                ?.GetComponent<InteractableNpc>();
            Require(merchant != null, "Village merchant NPC is missing.");
            Require(merchant.GetComponent<MerchantShop>() != null,
                "Village merchant must still expose trade through the hub.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 033 validation failed: {message}");
            }
        }
    }
}
