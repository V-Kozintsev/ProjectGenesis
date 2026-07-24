using System;
using ProjectGenesis.Data;
using ProjectGenesis.Gameplay;
using ProjectGenesis.Saving;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class MerchantShopDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";

        [MenuItem("Project Genesis/Sprint 032/Validate Merchant Shop Foundation")]
        public static void ValidateMerchantShopFoundation()
        {
            ValidateRuntimeBuyAndSell();
            ValidateSceneWiring();
            Debug.Log("Sprint 032 merchant shop foundation validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 032/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateMerchantShopFoundation();
            MapFoundationDevelopmentValidator.ValidateRelevantRegressionSuite();
            Debug.Log("Sprint 032 relevant regression suite passed.");
        }

        private static void ValidateRuntimeBuyAndSell()
        {
            ItemDefinition potion = ScriptableObject.CreateInstance<ItemDefinition>();
            potion.Configure(
                "test.potion",
                "Test Potion",
                ItemType.Consumable,
                healing: 10,
                buy: 12,
                sell: 3);

            GameObject player = new("MerchantShopRuntimePlayer");
            GameObject ui = new("MerchantShopRuntimeUi");
            try
            {
                PlayerInventory inventory = player.AddComponent<PlayerInventory>();
                PlayerWallet wallet = player.AddComponent<PlayerWallet>();
                wallet.RestoreGold(20);
                LocalMessageStream messages = player.AddComponent<LocalMessageStream>();
                MerchantShopView shopView = ui.AddComponent<MerchantShopView>();
                shopView.Open(
                    "Test Shop",
                    new[] { potion },
                    wallet,
                    inventory,
                    messages);

                Require(shopView.TryBuy(potion), "Buying an affordable stocked item must succeed.");
                Require(wallet.Gold == 8, "Buying must spend the item buy price.");
                Require(inventory.Count == 1, "Buying must add one item instance to the bag.");

                ItemInstance boughtItem = inventory.GetItemAt(0);
                Require(shopView.TrySell(boughtItem), "Selling a bag item must succeed.");
                Require(wallet.Gold == 11, "Selling must add the item sell price.");
                Require(inventory.Count == 0, "Selling must remove the concrete item instance.");

                PlayerProfileData profile = new();
                Require(profile.Version == PlayerProfileData.CurrentVersion &&
                        PlayerProfileData.CurrentVersion >= 8,
                    "Profile version must include persisted gold.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(player);
                UnityEngine.Object.DestroyImmediate(ui);
                UnityEngine.Object.DestroyImmediate(potion);
            }
        }

        private static void ValidateSceneWiring()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject player = GameObject.Find("Player");
            Require(player != null, "Player is missing from the starter village scene.");
            Require(player.GetComponent<PlayerWallet>() != null,
                "Player must have a wallet for gold.");
            Require(FindInactiveGameObject("Text_InventoryGold") != null,
                "Inventory window must show the player's gold.");

            PlayerPersistenceController persistence =
                player.GetComponent<PlayerPersistenceController>();
            Require(persistence != null, "Player persistence controller is missing.");
            Require(persistence.ItemCatalog != null && persistence.ItemCatalog.Count >= 4,
                "Persistence item catalog must include shop and loot items.");

            MerchantShopView shopView = UnityEngine.Object.FindFirstObjectByType<MerchantShopView>(
                FindObjectsInactive.Include);
            Require(shopView != null, "Merchant shop UI is missing.");

            InteractableNpc merchant = GameObject.Find("NPC_VillageMerchant")
                ?.GetComponent<InteractableNpc>();
            Require(merchant != null, "Village merchant NPC is missing.");
            MerchantShop shop = merchant.GetComponent<MerchantShop>();
            Require(shop != null, "Village merchant must have a shop component.");
            Require(shop.Stock != null && shop.Stock.Count >= 2,
                "Village merchant must sell at least two starter items.");

            bool hasPotion = false;
            foreach (ItemDefinition item in shop.Stock)
            {
                if (item == null)
                {
                    continue;
                }

                Require(item.BuyPrice > 0, $"{item.DisplayName} must have a buy price.");
                Require(item.SellPrice > 0, $"{item.DisplayName} must have a sell price.");
                if (item.ItemId == "consumable.minor_healing_potion")
                {
                    hasPotion = true;
                }
            }

            Require(hasPotion, "Merchant stock must include the minor healing potion.");
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(
                    $"Sprint 032 validation failed: {message}");
            }
        }

        private static GameObject FindInactiveGameObject(string objectName)
        {
            Transform[] transforms = UnityEngine.Object.FindObjectsByType<Transform>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            foreach (Transform transform in transforms)
            {
                if (transform.name == objectName)
                {
                    return transform.gameObject;
                }
            }

            return null;
        }
    }
}
