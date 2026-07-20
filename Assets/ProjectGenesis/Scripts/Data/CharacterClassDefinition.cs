using UnityEngine;

namespace ProjectGenesis.Data
{
    [CreateAssetMenu(
        fileName = "SO_Class_New",
        menuName = "Project Genesis/Character Class Definition")]
    public sealed class CharacterClassDefinition : ScriptableObject
    {
        [SerializeField] private string classId = "class.new";
        [SerializeField] private string displayName = "New Class";

        public string ClassId => classId;
        public string DisplayName => displayName;
        public bool IsValid =>
            !string.IsNullOrWhiteSpace(classId) &&
            !string.IsNullOrWhiteSpace(displayName);

        public void Configure(string id, string className)
        {
            classId = string.IsNullOrWhiteSpace(id) ? "class.new" : id.Trim();
            displayName = string.IsNullOrWhiteSpace(className) ? "New Class" : className.Trim();
        }
    }
}
