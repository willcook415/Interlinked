using UnityEngine;
using Interlinked.Domain.Stations;
using Interlinked.Domain.Primitives;

namespace Interlinked.Presentation.Stations
{
    /// <summary>
    /// Thin view binder: keeps the GameObject in sync with a domain Station.
    /// </summary>
    public sealed class StationView : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private string stationName;

        public string Id => id;
        public string StationName => stationName;

        public void Bind(Station s)
        {
            id = s.Id.ToString();
            stationName = s.Name;
            transform.position = new Vector3(s.Position.X, s.Position.Y, 0f);
            name = $"Station [{stationName}]";
        }

        public void UpdatePosition(Vec2 p)
        {
            transform.position = new Vector3(p.X, p.Y, 0f);
        }

        public void UpdateName(string newName)
        {
            stationName = newName;
            name = $"Station [{stationName}]";
        }
    }
}
