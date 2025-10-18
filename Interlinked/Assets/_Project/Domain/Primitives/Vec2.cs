using System; // <-- add this

namespace Interlinked.Domain.Primitives
{
    public readonly struct Vec2
    {
        public readonly float X;
        public readonly float Y;

        public Vec2(float x, float y) { X = x; Y = y; }

        public Vec2 Snap(float grid)
        {
            // Round using System.Math (double), then cast back to float
            var sx = (float)Math.Round(X / grid) * grid;
            var sy = (float)Math.Round(Y / grid) * grid;
            return new Vec2(sx, sy);
        }

        public override string ToString() => $"({X:0.###}, {Y:0.###})";
    }
}
