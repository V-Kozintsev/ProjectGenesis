using System;
using System.IO;
using UnityEngine;

namespace ProjectGenesis.Saving
{
    public sealed class LocalJsonPlayerPersistence : IPlayerPersistence
    {
        public const string ProfileFileName = "prototype-player-profile.json";

        public static string ProfilePath => Path.Combine(Application.persistentDataPath, ProfileFileName);

        public bool TryLoad(out PlayerProfileData profile)
        {
            profile = null;

            try
            {
                if (!File.Exists(ProfilePath))
                {
                    return false;
                }

                string json = File.ReadAllText(ProfilePath);
                profile = JsonUtility.FromJson<PlayerProfileData>(json);
                return profile != null && IsSupportedVersion(profile.Version);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not load the local prototype profile: {exception.Message}");
                profile = null;
                return false;
            }
        }

        public bool Save(PlayerProfileData profile)
        {
            if (profile == null)
            {
                return false;
            }

            try
            {
                profile.Version = PlayerProfileData.CurrentVersion;
                string directory = Path.GetDirectoryName(ProfilePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonUtility.ToJson(profile, true);
                string temporaryPath = ProfilePath + ".tmp";
                File.WriteAllText(temporaryPath, json);

                if (File.Exists(ProfilePath))
                {
                    File.Delete(ProfilePath);
                }

                File.Move(temporaryPath, ProfilePath);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not save the local prototype profile: {exception.Message}");
                return false;
            }
        }

        public static bool DeleteProfile()
        {
            try
            {
                if (File.Exists(ProfilePath))
                {
                    File.Delete(ProfilePath);
                }

                string temporaryPath = ProfilePath + ".tmp";
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not clear the local prototype profile: {exception.Message}");
                return false;
            }
        }

        public static bool IsSupportedVersion(int version)
        {
            return version >= 1 && version <= PlayerProfileData.CurrentVersion;
        }
    }
}
