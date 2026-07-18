using ProjectGenesis.Saving;
using UnityEditor;
using UnityEngine;

namespace ProjectGenesis.Tools.Editor
{
    public static class LocalProfileDevelopmentTools
    {
        [MenuItem("Project Genesis/Development/Clear Local Prototype Profile")]
        public static void ClearLocalPrototypeProfile()
        {
            if (LocalJsonPlayerPersistence.DeleteProfile())
            {
                Debug.Log($"Local prototype profile cleared: {LocalJsonPlayerPersistence.ProfilePath}");
            }
        }
    }
}
