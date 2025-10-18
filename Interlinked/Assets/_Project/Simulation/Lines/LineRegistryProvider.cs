using UnityEngine;

namespace Interlinked.Simulation.Lines
{
    public sealed class LineRegistryProvider : MonoBehaviour
    {
        public LineRegistry Instance { get; private set; }
        private void Awake() => Instance = new LineRegistry();
    }
}
