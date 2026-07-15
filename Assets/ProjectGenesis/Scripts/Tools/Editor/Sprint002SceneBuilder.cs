using ProjectGenesis.Core;
using ProjectGenesis.Gameplay;
using ProjectGenesis.UI;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectGenesis.Tools.Editor
{
    public static class Sprint002SceneBuilder
    {
        private const string ScenePath = "Assets/ProjectGenesis/Scenes/StarterVillage.unity";
        private const string PlayerPrefabPath = "Assets/ProjectGenesis/Prefabs/Characters/PF_Player_Prototype.prefab";
        private const string GroundMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Starter_Ground.mat";
        private const string PlayerMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Player_Prototype.mat";
        private const string MarkerMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Move_Target.mat";
        private const string RoadMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Village_Road.mat";
        private const string BuildingMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Village_Building.mat";
        private const string BoundaryMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Village_Boundary.mat";
        private const string PropMaterialPath = "Assets/ProjectGenesis/Materials/MAT_Village_Prop.mat";
        private const string NpcMaterialPath = "Assets/ProjectGenesis/Materials/MAT_NPC_Village_Elder.mat";

        [MenuItem("Project Genesis/Sprint 002/Rebuild Starter Village Blockout")]
        public static void RebuildStarterVillageBlockout()
        {
            RebuildStarterVillage();
        }

        [MenuItem("Project Genesis/Sprint 003/Rebuild Starter Village First Contact")]
        public static void RebuildStarterVillageFirstContact()
        {
            RebuildStarterVillage();
        }

        public static void RebuildStarterVillage()
        {
            EnsureFolders();

            Material groundMaterial = CreateMaterial(GroundMaterialPath, new Color(0.28f, 0.42f, 0.28f));
            Material playerMaterial = CreateMaterial(PlayerMaterialPath, new Color(0.25f, 0.45f, 0.9f));
            Material markerMaterial = CreateMaterial(MarkerMaterialPath, new Color(1f, 0.72f, 0.12f));
            Material roadMaterial = CreateMaterial(RoadMaterialPath, new Color(0.46f, 0.38f, 0.28f));
            Material buildingMaterial = CreateMaterial(BuildingMaterialPath, new Color(0.55f, 0.45f, 0.34f));
            Material boundaryMaterial = CreateMaterial(BoundaryMaterialPath, new Color(0.36f, 0.28f, 0.18f));
            Material propMaterial = CreateMaterial(PropMaterialPath, new Color(0.48f, 0.32f, 0.18f));
            Material npcMaterial = CreateMaterial(NpcMaterialPath, new Color(0.72f, 0.64f, 0.42f));

            GameObject playerPrefab = CreatePlayerPrefab(playerMaterial);

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "StarterVillage";

            GameObject navigationRoot = new("Navigation");
            CreateGround(navigationRoot.transform, groundMaterial);
            CreateRoad("Road_Main_NorthSouth", new Vector3(0f, 0.015f, 0f), new Vector3(1.4f, 0.03f, 14f), roadMaterial);
            CreateRoad("Road_Main_EastWest", new Vector3(0f, 0.02f, 0f), new Vector3(14f, 0.03f, 1.4f), roadMaterial);

            CreateVillageBlockout(buildingMaterial, boundaryMaterial, propMaterial);

            GameObject spawnPoint = CreateSpawnPoint();
            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            player.name = "Player";
            player.transform.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);
            player.AddComponent<QuestLog>();
            PlayerInteractionController interactionController = player.AddComponent<PlayerInteractionController>();

            GameObject destinationMarker = CreateDestinationMarker(markerMaterial);
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetDestinationMarker(destinationMarker);

            Camera camera = CreateCamera(player.transform);
            playerController.SetCameraTransform(camera.transform);
            interactionController.SetGameplayCamera(camera);

            CreateVillageElder(npcMaterial);
            FirstContactUi ui = CreateFirstContactUi();
            interactionController.SetDialogueWindow(ui.DialogueWindow);
            interactionController.SetPromptView(ui.PromptView);

            CreateEventSystem();
            CreateLighting();
            AddRuntimeNavMesh(navigationRoot);

            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsureFolders()
        {
            string[] folders =
            {
                "Assets/ProjectGenesis/Materials",
                "Assets/ProjectGenesis/Prefabs/Characters",
                "Assets/ProjectGenesis/Prefabs/UI",
                "Assets/ProjectGenesis/Prefabs/World",
                "Assets/ProjectGenesis/Scenes"
            };

            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    string parent = System.IO.Path.GetDirectoryName(folder)?.Replace('\\', '/');
                    string child = System.IO.Path.GetFileName(folder);

                    if (!string.IsNullOrEmpty(parent))
                    {
                        AssetDatabase.CreateFolder(parent, child);
                    }
                }
            }
        }

        private static Material CreateMaterial(string path, Color color)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader)
                {
                    color = color
                };

                AssetDatabase.CreateAsset(material, path);
            }
            else
            {
                material.color = color;
                EditorUtility.SetDirty(material);
            }

            return material;
        }

        private static GameObject CreatePlayerPrefab(Material playerMaterial)
        {
            GameObject player = new("PF_Player_Prototype");
            player.transform.position = Vector3.zero;

            NavMeshAgent agent = player.AddComponent<NavMeshAgent>();
            agent.height = 2f;
            agent.radius = 0.35f;
            agent.baseOffset = 0f;
            agent.speed = 5f;
            agent.angularSpeed = 720f;
            agent.acceleration = 24f;
            agent.stoppingDistance = 0.25f;
            agent.enabled = false;

            player.AddComponent<PlayerController>();

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(player.transform, false);
            visual.transform.localPosition = new Vector3(0f, 1f, 0f);
            Object.DestroyImmediate(visual.GetComponent<Collider>());
            visual.GetComponent<Renderer>().sharedMaterial = playerMaterial;

            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(player, PlayerPrefabPath);
            Object.DestroyImmediate(player);

            return savedPrefab;
        }

        private static void CreateGround(Transform parent, Material groundMaterial)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground_StarterVillage";
            ground.transform.SetParent(parent, false);
            ground.transform.localScale = new Vector3(8f, 1f, 8f);
            ground.GetComponent<Renderer>().sharedMaterial = groundMaterial;
        }

        private static void CreateRoad(string name, Vector3 position, Vector3 scale, Material roadMaterial)
        {
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = name;
            road.transform.SetPositionAndRotation(position, Quaternion.identity);
            road.transform.localScale = scale;
            road.GetComponent<Renderer>().sharedMaterial = roadMaterial;
            Object.DestroyImmediate(road.GetComponent<Collider>());
        }

        private static void CreateVillageBlockout(Material buildingMaterial, Material boundaryMaterial, Material propMaterial)
        {
            CreateBuilding("Blockout_House_NorthWest", new Vector3(-5.2f, 1.25f, 4.8f), new Vector3(2.4f, 2.5f, 2.2f), buildingMaterial);
            CreateBuilding("Blockout_House_NorthEast", new Vector3(5.2f, 1.25f, 4.5f), new Vector3(2.6f, 2.5f, 2.4f), buildingMaterial);
            CreateBuilding("Blockout_House_SouthWest", new Vector3(-4.8f, 1.25f, -4.8f), new Vector3(2.5f, 2.5f, 2.5f), buildingMaterial);
            CreateBuilding("Blockout_Barn_SouthEast", new Vector3(5.2f, 1.5f, -4.9f), new Vector3(3.2f, 3f, 2.4f), buildingMaterial);

            CreateBuilding("Blockout_Well", new Vector3(1.8f, 0.45f, 1.7f), new Vector3(1.2f, 0.9f, 1.2f), propMaterial);
            CreateBuilding("Blockout_Crates_WestRoad", new Vector3(-2.7f, 0.45f, 1.1f), new Vector3(1.1f, 0.9f, 1.1f), propMaterial);
            CreateBuilding("Blockout_Cart_EastRoad", new Vector3(3.1f, 0.45f, -1.2f), new Vector3(1.5f, 0.9f, 0.8f), propMaterial);

            CreateBoundary("Boundary_North", new Vector3(0f, 0.65f, 8.2f), new Vector3(17f, 1.3f, 0.4f), boundaryMaterial);
            CreateBoundary("Boundary_South", new Vector3(0f, 0.65f, -8.2f), new Vector3(17f, 1.3f, 0.4f), boundaryMaterial);
            CreateBoundary("Boundary_East", new Vector3(8.2f, 0.65f, 0f), new Vector3(0.4f, 1.3f, 17f), boundaryMaterial);
            CreateBoundary("Boundary_West", new Vector3(-8.2f, 0.65f, 0f), new Vector3(0.4f, 1.3f, 17f), boundaryMaterial);
        }

        private static void CreateBuilding(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.SetPositionAndRotation(position, Quaternion.identity);
            block.transform.localScale = scale;
            block.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void CreateBoundary(string name, Vector3 position, Vector3 scale, Material material)
        {
            CreateBuilding(name, position, scale, material);
        }

        private static GameObject CreateSpawnPoint()
        {
            GameObject spawnPoint = new("PlayerSpawnPoint");
            spawnPoint.transform.SetPositionAndRotation(new Vector3(0f, 0.05f, -5.8f), Quaternion.identity);
            spawnPoint.AddComponent<PlayerSpawnPoint>();
            return spawnPoint;
        }

        private static GameObject CreateDestinationMarker(Material markerMaterial)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = "MoveTargetMarker";
            marker.transform.localScale = new Vector3(0.35f, 0.015f, 0.35f);
            Object.DestroyImmediate(marker.GetComponent<Collider>());
            marker.GetComponent<Renderer>().sharedMaterial = markerMaterial;
            marker.SetActive(false);
            return marker;
        }

        private static Camera CreateCamera(Transform target)
        {
            GameObject cameraObject = new("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 4.5f, -8f);

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 250f;

            cameraObject.AddComponent<AudioListener>();
            ThirdPersonCameraFollow follow = cameraObject.AddComponent<ThirdPersonCameraFollow>();
            follow.SetTarget(target);

            return camera;
        }

        private static void CreateVillageElder(Material npcMaterial)
        {
            GameObject elder = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            elder.name = "NPC_VillageElder";
            elder.transform.SetPositionAndRotation(new Vector3(-1.9f, 1f, -1.9f), Quaternion.Euler(0f, 35f, 0f));
            elder.transform.localScale = new Vector3(0.9f, 1f, 0.9f);
            elder.GetComponent<Renderer>().sharedMaterial = npcMaterial;
            elder.AddComponent<InteractableNpc>();
        }

        private static FirstContactUi CreateFirstContactUi()
        {
            GameObject canvasObject = new("UI_FirstContact");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject textObject = new("Text_Status");
            textObject.transform.SetParent(canvasObject.transform, false);
            Text text = textObject.AddComponent<Text>();
            text.text = "First Contact Prototype";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.color = new Color(1f, 0.95f, 0.82f);
            text.alignment = TextAnchor.UpperLeft;

            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(24f, -24f);
            rect.sizeDelta = new Vector2(420f, 48f);

            InteractionPromptView promptView = CreateInteractionPrompt(canvasObject.transform);
            DialogueWindow dialogueWindow = CreateDialogueWindow(canvasObject.transform);

            return new FirstContactUi(dialogueWindow, promptView);
        }

        private static InteractionPromptView CreateInteractionPrompt(Transform canvasTransform)
        {
            GameObject root = CreatePanel("InteractionPrompt", canvasTransform, new Color(0.05f, 0.06f, 0.07f, 0.82f));
            root.GetComponent<Image>().raycastTarget = false;
            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0f);
            rootRect.anchorMax = new Vector2(0.5f, 0f);
            rootRect.pivot = new Vector2(0.5f, 0f);
            rootRect.anchoredPosition = new Vector2(0f, 42f);
            rootRect.sizeDelta = new Vector2(420f, 56f);

            Text promptText = CreateText("Text_Prompt", root.transform, "Press E - Talk", 24, TextAnchor.MiddleCenter);
            promptText.color = new Color(1f, 0.95f, 0.82f);
            Stretch(promptText.GetComponent<RectTransform>(), 18f, 8f, 18f, 8f);

            InteractionPromptView promptView = root.AddComponent<InteractionPromptView>();
            promptView.Initialize(root, promptText);
            return promptView;
        }

        private static DialogueWindow CreateDialogueWindow(Transform canvasTransform)
        {
            GameObject root = CreatePanel("DialogueWindow", canvasTransform, new Color(0.06f, 0.07f, 0.08f, 0.94f));
            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0f, 0.5f);
            rootRect.anchorMax = new Vector2(0f, 0.5f);
            rootRect.pivot = new Vector2(0f, 0.5f);
            rootRect.anchoredPosition = new Vector2(28f, -20f);
            rootRect.sizeDelta = new Vector2(720f, 420f);

            Text speakerText = CreateText("Text_Speaker", root.transform, "Староста деревни", 30, TextAnchor.UpperLeft);
            speakerText.color = new Color(1f, 0.86f, 0.48f);
            SetRect(speakerText.GetComponent<RectTransform>(), new Vector2(28f, -24f), new Vector2(664f, 42f), new Vector2(0f, 1f));

            Text bodyText = CreateText("Text_Body", root.transform, "", 22, TextAnchor.UpperLeft);
            bodyText.color = new Color(0.94f, 0.94f, 0.9f);
            bodyText.lineSpacing = 1.12f;
            SetRect(bodyText.GetComponent<RectTransform>(), new Vector2(28f, -78f), new Vector2(664f, 118f), new Vector2(0f, 1f));

            Text questText = CreateText("Text_Quest", root.transform, "", 20, TextAnchor.UpperLeft);
            questText.color = new Color(0.77f, 0.91f, 1f);
            questText.lineSpacing = 1.08f;
            SetRect(questText.GetComponent<RectTransform>(), new Vector2(28f, -210f), new Vector2(664f, 86f), new Vector2(0f, 1f));

            Button acceptButton = CreateButton("Button_AcceptQuest", root.transform, "Принять поручение");
            SetRect(acceptButton.GetComponent<RectTransform>(), new Vector2(28f, 28f), new Vector2(300f, 58f), new Vector2(0f, 0f));

            Button closeButton = CreateButton("Button_CloseDialogue", root.transform, "Закрыть");
            SetRect(closeButton.GetComponent<RectTransform>(), new Vector2(348f, 28f), new Vector2(180f, 58f), new Vector2(0f, 0f));

            DialogueWindow dialogueWindow = root.AddComponent<DialogueWindow>();
            dialogueWindow.Initialize(root, speakerText, bodyText, questText, acceptButton, closeButton);
            return dialogueWindow;
        }

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            GameObject panel = new(name);
            panel.transform.SetParent(parent, false);
            Image image = panel.AddComponent<Image>();
            image.color = color;
            return panel;
        }

        private static Text CreateText(string name, Transform parent, string value, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new(name);
            textObject.transform.SetParent(parent, false);
            Text text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label)
        {
            GameObject buttonObject = CreatePanel(name, parent, new Color(0.16f, 0.25f, 0.34f, 1f));
            Button button = buttonObject.AddComponent<Button>();
            Text labelText = CreateText("Text_Label", buttonObject.transform, label, 20, TextAnchor.MiddleCenter);
            labelText.color = Color.white;
            Stretch(labelText.GetComponent<RectTransform>(), 8f, 4f, 8f, 4f);
            return button;
        }

        private static void SetRect(RectTransform rect, Vector2 anchoredPosition, Vector2 size, Vector2 anchor)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        private static void Stretch(RectTransform rect, float left, float top, float right, float bottom)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        private static void CreateLighting()
        {
            GameObject lightObject = new("Directional Light");
            lightObject.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.45f, 0.48f, 0.52f);
        }

        private static void CreateEventSystem()
        {
            GameObject eventSystemObject = new("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static void AddRuntimeNavMesh(GameObject navigationRoot)
        {
            NavMeshSurface surface = navigationRoot.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.defaultArea = 0;
            navigationRoot.AddComponent<StarterVillageRuntimeBuilder>();
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(scenePath, true)
            };
        }

        private readonly struct FirstContactUi
        {
            public FirstContactUi(DialogueWindow dialogueWindow, InteractionPromptView promptView)
            {
                DialogueWindow = dialogueWindow;
                PromptView = promptView;
            }

            public DialogueWindow DialogueWindow { get; }
            public InteractionPromptView PromptView { get; }
        }
    }
}
