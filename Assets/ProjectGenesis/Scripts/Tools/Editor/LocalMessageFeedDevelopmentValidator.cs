using System;
using System.Linq;
using ProjectGenesis.Gameplay;
using ProjectGenesis.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectGenesis.Tools.Editor
{
    public static class LocalMessageFeedDevelopmentValidator
    {
        private const string ScenePath =
            "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath =
            "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";

        [MenuItem("Project Genesis/Sprint 024/Validate Local Message Feed")]
        public static void ValidateLocalMessageFeed()
        {
            ValidateTypedBoundedStream();
            ValidatePlayerPrefab();
            ValidateScene();
            Debug.Log("Sprint 024 local message feed validation passed.");
        }

        [MenuItem("Project Genesis/Sprint 024/Validate Relevant Regression Suite")]
        public static void ValidateRelevantRegressionSuite()
        {
            ValidateLocalMessageFeed();
            CharacterEquipmentDevelopmentValidator.ValidateRelevantRegressionSuite();
            Debug.Log("Sprint 024 relevant regression suite passed.");
        }

        [MenuItem("Project Genesis/Sprint 024/Publish Play Mode Test Messages")]
        public static void PublishPlayModeTestMessages()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Enter Play Mode before publishing Sprint 024 test messages.");
                return;
            }

            LocalMessageStream stream =
                UnityEngine.Object.FindFirstObjectByType<LocalMessageStream>();
            if (stream == null)
            {
                Debug.LogWarning("Could not find the local message stream.");
                return;
            }

            stream.Publish(LocalMessageCategory.System, "Проверка системного сообщения.");
            stream.Publish(LocalMessageCategory.Loot, "Проверка сообщения о предмете.");
            stream.Publish(LocalMessageCategory.Combat, "Проверка боевого сообщения.");
            stream.Publish(
                LocalMessageCategory.Announcement,
                "Проверка локального объявления прототипа.");
            Debug.Log("Sprint 024 test messages published.");
        }

        private static void ValidateTypedBoundedStream()
        {
            GameObject probe = new("LocalMessageStreamValidation");
            try
            {
                LocalMessageStream stream = probe.AddComponent<LocalMessageStream>();
                stream.ConfigureHistoryCapacity(10);
                int publishedCount = 0;
                stream.MessagePublished += _ => publishedCount++;

                Require(!stream.Publish(LocalMessageCategory.System, "   "),
                    "Blank messages must be rejected.");
                for (int index = 0; index < 12; index++)
                {
                    LocalMessageCategory category = index % 2 == 0
                        ? LocalMessageCategory.Loot
                        : LocalMessageCategory.Combat;
                    Require(stream.Publish(category, $"Message {index + 1}"),
                        "A valid typed message must be accepted.");
                }

                Require(publishedCount == 12 && stream.Entries.Count == 10,
                    "The stream must publish every accepted event and bound its history.");
                Require(stream.Entries[0].Sequence == 3 &&
                        stream.Entries[^1].Sequence == 12,
                    "History trimming must preserve order and monotonic sequence numbers.");
                Require(stream.Entries.All(entry =>
                        LocalMessageStream.Matches(entry.Category, LocalMessageFilter.All)),
                    "The All filter must include every message category.");
                Require(LocalMessageStream.Matches(
                            LocalMessageCategory.Announcement,
                            LocalMessageFilter.Announcement) &&
                        !LocalMessageStream.Matches(
                            LocalMessageCategory.Announcement,
                            LocalMessageFilter.Combat),
                    "Category filters must not mix announcements with combat.");

                stream.Clear();
                Require(stream.Entries.Count == 0,
                    "Clearing the local session history must remove all entries.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }
        }

        private static void ValidatePlayerPrefab()
        {
            GameObject player = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            Require(player != null, "Player prefab was not found.");
            Require(player.GetComponent<LocalMessageStream>() != null,
                "Player prefab must own one session-local message stream.");
            Require(player.GetComponents<LocalMessageStream>().Length == 1,
                "Player prefab must not contain duplicate message streams.");
        }

        private static void ValidateScene()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            LocalMessageFeedView[] views =
                UnityEngine.Object.FindObjectsByType<LocalMessageFeedView>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            Require(views.Length == 1, "Starter scene must contain one local message feed.");

            LocalMessageFeedView view = views[0];
            Require(view.WindowRoot != null && view.ReopenButton != null &&
                    view.MessagesText != null && view.MessageScroll != null &&
                    view.ChatInput != null && view.SendButton != null &&
                    view.MessageStream != null,
                "Message feed references must be wired.");
            Require(FindChild(view.WindowRoot.transform, "DragHandle") != null,
                "The message window must be draggable by its header.");
            Require(FindChild(view.WindowRoot.transform, "Button_MessageFilterAll") != null &&
                    FindChild(view.WindowRoot.transform, "Button_MessageFilterSystem") != null &&
                    FindChild(view.WindowRoot.transform, "Button_MessageFilterLoot") != null &&
                    FindChild(view.WindowRoot.transform, "Button_MessageFilterCombat") != null &&
                    FindChild(view.WindowRoot.transform, "Button_MessageFilterChat") != null &&
                    FindChild(view.WindowRoot.transform, "Button_MessageFilterAnnouncement") != null,
                "The message feed must expose its prototype category filters.");
            Require(view.ChatInput.characterLimit == 160 &&
                    view.ChatInput.lineType == InputField.LineType.SingleLine &&
                    view.ChatInput.customCaretColor && view.ChatInput.caretWidth >= 2 &&
                    view.ChatInput.caretBlinkRate > 0f,
                "Local chat input must be constrained and show a visible blinking caret.");
            Require(view.MessageScroll.content == view.MessagesText.rectTransform &&
                    view.MessageScroll.vertical && !view.MessageScroll.horizontal &&
                    view.MessageScroll.verticalScrollbar != null &&
                    view.MessageScroll.verticalScrollbar.handleRect != null,
                "Message history must use a vertical ScrollRect with a draggable scrollbar.");
        }

        private static Transform FindChild(Transform root, string childName)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
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
                throw new InvalidOperationException(
                    $"Sprint 024 validation failed: {message}");
            }
        }
    }
}
