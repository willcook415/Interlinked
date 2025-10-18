using System;

namespace Interlinked.Domain.Lines
{
    public readonly struct LineId
    {
        public readonly Guid Value;
        public LineId(Guid value) => Value = value;
        public static LineId New() => new LineId(Guid.NewGuid());
        public override string ToString() => Value.ToString("N");
    }
}
