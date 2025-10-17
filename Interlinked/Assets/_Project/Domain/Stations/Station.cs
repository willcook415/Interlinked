using Interlinked.Domain.Primitives;

namespace Interlinked.Domain.Stations
{
    public sealed class Station
    {
        public StationId Id { get; }
        public string Name { get; private set; }
        public Vec2 Position { get; private set; }

        public Station(StationId id, string name, Vec2 position)
        {
            Id = id; Name = name; Position = position;
        }

        public void Rename(string newName) => Name = newName;
        public void Move(Vec2 newPos) => Position = newPos;
    }
}
