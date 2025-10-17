using System;

namespace Interlinked.Domain.Stations
{
    public readonly struct StationId
    {
        public readonly Guid Value;
        public StationId(Guid value) => Value = value;
        public static StationId New() => new StationId(Guid.NewGuid());
        public override string ToString() => Value.ToString("N");
    }
}
