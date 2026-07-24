using System;
using System.Collections.Generic;
using ProjectGenesis.Data;
using ProjectGenesis.UI;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class MerchantShop : MonoBehaviour
    {
        [SerializeField] private string shopName = "Торговец";
        [SerializeField] private ItemDefinition[] stock = Array.Empty<ItemDefinition>();

        public string ShopName => string.IsNullOrWhiteSpace(shopName) ? "Торговец" : shopName;
        public IReadOnlyList<ItemDefinition> Stock => stock;

        public void Configure(string displayName, params ItemDefinition[] stockItems)
        {
            shopName = string.IsNullOrWhiteSpace(displayName) ? "Торговец" : displayName.Trim();
            stock = stockItems ?? Array.Empty<ItemDefinition>();
        }

        public void OpenFor(GameObject player, MerchantShopView shopView)
        {
            if (player == null || shopView == null)
            {
                return;
            }

            shopView.Open(
                ShopName,
                stock,
                player.GetComponent<PlayerWallet>(),
                player.GetComponent<PlayerInventory>(),
                player.GetComponent<LocalMessageStream>());
        }
    }
}
