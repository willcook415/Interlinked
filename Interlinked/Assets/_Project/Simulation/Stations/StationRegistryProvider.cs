using UnityEngine;

namespace Interlinked.Simulation.Stations
{
    public sealed class StationRegistryProvider : MonoBehaviour
    {
        public StationRegistry Instance { get; private set; }
        private void Awake() => Instance = new StationRegistry();
    }
}
