using ProjectGenesis.Core;
using ProjectGenesis.Gameplay;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
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

        [MenuItem("Project Genesis/Sprint 002/Rebuild Starter Village Blockout")]
        public static void RebuildStarterVillageBlockout()
        {
            EnsureFolders();

            Material groundMaterial = CreateMaterial(GroundMaterialPath, new Color(0.28f, 0.42f, 0.28f));
            Material playerMaterial = CreateMaterial(PlayerMaterialPath, new Color(0.25f, 0.45f, 0.9f));
            Material markerMaterial = CreateMaterial(MarkerMaterialPath, new Color(1f, 0.72f, 0.12f));
            Material roadMaterial = CreateMaterial(RoadMaterialPath, new Color(0.46f, 0.38f, 0.28f));
            Material buildingMaterial = CreateMaterial(BuildingMaterialPath, new Color(0.55f, 0.45f, 0.34f));
            Material boundaryMaterial = CreateMaterial(BoundaryMaterialPath, new Color(0.36f, 0.28f, 0.18f));
            Material propMaterial = CreateMaterial(PropMaterialPath, new Color(0.48f, 0.32f, 0.18f));

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

            GameObject destinationMarker = CreateDestinationMarker(markerMaterial);
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetDestinationMarker(destinationMarker);

            Camera camera = CreateCamera(player.transform);
            playerController.SetCameraTransform(camera.transform);

            CreateSprintPlaceholderUi();
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

        private static void CreateSprintPlaceholderUi()
        {
            GameObject canvasObject = new("UI_Sprint002_Placeholder");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject textObject = new("Text_Status");
            textObject.transform.SetParent(canvasObject.transform, false);
            Text text = textObject.AddComponent<Text>();
            text.text = "Starter Village Blockout";
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
    }
}
