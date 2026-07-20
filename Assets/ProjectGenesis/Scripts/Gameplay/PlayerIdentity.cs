using System;
using ProjectGenesis.Data;
using UnityEngine;

namespace ProjectGenesis.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class PlayerIdentity : MonoBehaviour
    {
        public const string DefaultCharacterName = "Путник";
        public const int MaximumNameLength = 24;

        [SerializeField] private string characterName = DefaultCharacterName;
        [SerializeField] private CharacterRaceDefinition race;
        [SerializeField] private CharacterClassDefinition characterClass;

        public event Action<PlayerIdentity> Changed;

        public string CharacterName => characterName;
        public CharacterRaceDefinition Race => race;
        public CharacterClassDefinition CharacterClass => characterClass;
        public bool IsValid =>
            !string.IsNullOrWhiteSpace(characterName) &&
            characterName.Length <= MaximumNameLength &&
            race != null &&
            race.IsValid &&
            characterClass != null &&
            characterClass.IsValid;

        public void Configure(
            string initialName,
            CharacterRaceDefinition initialRace,
            CharacterClassDefinition initialClass)
        {
            characterName = NormalizeName(initialName, DefaultCharacterName);
            race = initialRace;
            characterClass = initialClass;
            Changed?.Invoke(this);
        }

        public void RestoreIdentity(
            string savedName,
            CharacterRaceDefinition savedRace,
            CharacterClassDefinition savedClass)
        {
            characterName = NormalizeName(savedName, characterName);

            if (savedRace != null && savedRace.IsValid)
            {
                race = savedRace;
            }

            if (savedClass != null && savedClass.IsValid)
            {
                characterClass = savedClass;
            }

            Changed?.Invoke(this);
        }

        public bool HasClass(string classId)
        {
            return characterClass != null &&
                   !string.IsNullOrWhiteSpace(classId) &&
                   characterClass.ClassId == classId;
        }

        public bool TrySetCharacterName(string requestedName)
        {
            if (!IsAcceptableName(requestedName))
            {
                return false;
            }

            characterName = NormalizeName(requestedName);
            Changed?.Invoke(this);
            return true;
        }

        public static bool IsAcceptableName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            int length = value.Trim().Length;
            return length > 0 && length <= MaximumNameLength;
        }

        public static string NormalizeName(string value, string fallback = DefaultCharacterName)
        {
            string normalized = string.IsNullOrWhiteSpace(value)
                ? fallback
                : value.Trim();

            if (string.IsNullOrWhiteSpace(normalized))
            {
                normalized = DefaultCharacterName;
            }

            return normalized.Length <= MaximumNameLength
                ? normalized
                : normalized.Substring(0, MaximumNameLength);
        }
    }
}
