using System.Collections.Generic;
using Interlinked.Domain.Primitives;
using Interlinked.Domain.Stations;

namespace Interlinked.Domain.Lines
{
    public sealed class Line
    {
        public LineId Id { get; }
        public string Name { get; private set; }
        public StationId A { get; }
        public StationId B { get; }
        // Path points in order (A -> corner -> B) in world space (domain Vec2)
        public IReadOnlyList<Vec2> Points => _points;
        private readonly List<Vec2> _points;

        public Line(LineId id, string name, StationId a, StationId b, List<Vec2> points)
        {
            Id = id; Name = name; A = a; B = b; _points = points;
        }

        public void Rename(string name) => Name = name;
    }
}
