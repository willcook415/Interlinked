using System.Collections.Generic;
using Interlinked.Domain.Lines;

namespace Interlinked.Simulation.Lines
{
    public sealed class LineRegistry
    {
        private readonly Dictionary<string, Line> _byId = new();
        private int _nameCounter = 1;
        public IEnumerable<Line> All => _byId.Values;

        public Line Create(Line line)
        {
            _byId[line.Id.ToString()] = line;
            return line;
        }

        public string NextName() => $"Line {_nameCounter++}";
    }
}
