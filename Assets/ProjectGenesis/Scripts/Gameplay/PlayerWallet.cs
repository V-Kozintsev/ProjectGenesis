using System;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class PlayerWallet : MonoBehaviour
    {
        [SerializeField, Min(0)] private int startingGold = 35;
        [SerializeField, Min(0)] private int gold = 35;

        public event Action<PlayerWallet> Changed;

        public int StartingGold => startingGold;
        public int Gold => gold;

        private void OnValidate()
        {
            startingGold = Mathf.Max(0, startingGold);
            gold = Mathf.Max(0, gold);
        }

        public void ConfigureStartingGold(int amount)
        {
            startingGold = Mathf.Max(0, amount);
            gold = startingGold;
            Changed?.Invoke(this);
        }

        public void RestoreGold(int amount)
        {
            gold = Mathf.Max(0, amount);
            Changed?.Invoke(this);
        }

        public bool CanAfford(int amount)
        {
            return amount >= 0 && gold >= amount;
        }

        public bool TrySpend(int amount)
        {
            if (!CanAfford(amount))
            {
                return false;
            }

            gold -= amount;
            Changed?.Invoke(this);
            return true;
        }

        public void AddGold(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            gold += amount;
            Changed?.Invoke(this);
        }
    }
}
