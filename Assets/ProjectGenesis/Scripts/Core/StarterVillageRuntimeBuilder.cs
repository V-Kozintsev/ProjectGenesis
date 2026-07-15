using Unity.AI.Navigation;
using UnityEngine;

namespace ProjectGenesis.Core
{
    [RequireComponent(typeof(NavMeshSurface))]
    public sealed class StarterVillageRuntimeBuilder : MonoBehaviour
    {
        private NavMeshSurface surface;

        private void Awake()
        {
            surface = GetComponent<NavMeshSurface>();
            surface.BuildNavMesh();
        }
    }
}
