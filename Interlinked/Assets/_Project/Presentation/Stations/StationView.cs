using UnityEngine;
using Interlinked.Domain.Stations;
using Interlinked.Domain.Primitives;

namespace Interlinked.Presentation.Stations
{
    public sealed class StationView : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private string stationName;

        public string Id => id;
        public string StationName => stationName;

        // NEW: remember original local scale so hover never corrupts size
        public Vector3 BaseScale { get; private set; }

        private void Awake()
        {
            BaseScale = transform.localScale; // store prefab’s scale (e.g., 0.2,0.2,1)
        }

        public void Bind(Station s)
        {
            id = s.Id.ToString();
            stationName = s.Name;
            transform.position = new Vector3(s.Position.X, s.Position.Y, 0f);
            name = $"Station [{stationName}]";
        }

        public void UpdatePosition(Vec2 p) =>
            transform.position = new Vector3(p.X, p.Y, 0f);

        public void UpdateName(string newName)
        {
            stationName = newName;
            name = $"Station [{stationName}]";
        }
    }
}
