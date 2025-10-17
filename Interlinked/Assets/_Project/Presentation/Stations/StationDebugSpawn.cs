using UnityEngine;
using Interlinked.Simulation.Stations;
using Interlinked.Domain.Primitives;

namespace Interlinked.Presentation.Stations
{
    /// <summary>
    /// TEMP: spawns a single Station on Play using the real Registry.
    /// Remove once placement is wired to input.
    /// </summary>
    public sealed class StationDebugSpawn : MonoBehaviour
    {
        [SerializeField] private StationRegistryProvider registryProvider;
        [SerializeField] private GameObject stationPrefab;

        private void Start()
        {
            if (registryProvider == null || stationPrefab == null)
            {
                Debug.LogError("StationDebugSpawn not wired: assign RegistryProvider and Station Prefab.");
                return;
            }

            // Create a station at (0,0) snapped by domain later
            var s = registryProvider.Instance.CreateAt(new Vec2(0f, 0f));
            var go = Instantiate(stationPrefab);
            var view = go.GetComponent<StationView>();
            view.Bind(s);
        }
    }
}
