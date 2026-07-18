using System;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class PlayerProgression : MonoBehaviour
    {
        [SerializeField, Min(1)] private int level = 1;
        [SerializeField, Min(0)] private int currentExperience;

        public event Action<PlayerProgression> ExperienceChanged;

        public int Level => level;
        public int CurrentExperience => currentExperience;

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            currentExperience += amount;
            ExperienceChanged?.Invoke(this);
        }
    }
}
