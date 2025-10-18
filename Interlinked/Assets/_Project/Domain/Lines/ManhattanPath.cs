using System.Collections.Generic;
using Interlinked.Domain.Primitives;

namespace Interlinked.Domain.Lines
{
    public static class ManhattanPath
    {
        // Build an L-shaped path A -> corner -> B, snapped to grid
        // If preferHorizontalFirst = true, corner is (B.X, A.Y). Else (A.X, B.Y)
        public static List<Vec2> Build(Vec2 a, Vec2 b, float grid, bool preferHorizontalFirst)
        {
            var A = a.Snap(grid);
            var B = b.Snap(grid);
            var corner = preferHorizontalFirst
                ? new Vec2(B.X, A.Y)
                : new Vec2(A.X, B.Y);

            // handle colinear cases (same x or y): just two points
            if (A.X == B.X || A.Y == B.Y)
                return new List<Vec2> { A, B };

            return new List<Vec2> { A, corner, B };
        }
    }
}
