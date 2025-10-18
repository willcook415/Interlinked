using System.Collections.Generic;
using Interlinked.Domain.Primitives;
using Interlinked.Domain.Stations;

namespace Interlinked.Simulation.Stations
{
    public sealed class StationRegistry
    {
        private readonly Dictionary<string, Station> _byId = new();
        private int _nameCounter = 0;
        public IEnumerable<Station> All => _byId.Values;

        public Station CreateAt(Vec2 pos)
        {
            var id = StationId.New();
            var name = NextAutoName();
            var s = new Station(id, name, pos);
            _byId[id.ToString()] = s;
            return s;
        }

        public bool Delete(string id) => _byId.Remove(id);

        public void Rename(string id, string newName)
        {
            if (_byId.TryGetValue(id, out var s)) s.Rename(newName);
        }

        private string NextAutoName()
        {
            string Seq(int n)
            {
                var letters = "";
                do { letters = (char)('A' + (n % 26)) + letters; n = n / 26 - 1; }
                while (n >= 0);
                return letters;
            }
            return $"Station {Seq(_nameCounter++)}";
        }
    }
}
