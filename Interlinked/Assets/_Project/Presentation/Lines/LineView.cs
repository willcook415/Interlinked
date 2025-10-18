using System.Collections.Generic;
using UnityEngine;
using Interlinked.Domain.Primitives;

namespace Interlinked.Presentation.Lines
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class LineView : MonoBehaviour
    {
        LineRenderer _lr;

        private void Awake()
        {
            _lr = GetComponent<LineRenderer>();
            _lr.positionCount = 0;
            _lr.alignment = LineAlignment.TransformZ;
            _lr.widthMultiplier = 0.1f;
            _lr.numCornerVertices = 2;
            _lr.numCapVertices = 2;
        }

        public void SetPoints(IReadOnlyList<Vec2> points)
        {
            _lr.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
                _lr.SetPosition(i, new Vector3(points[i].X, points[i].Y, 0));
        }

        public void SetMaterial(Material mat) => _lr.material = mat;
    }
}
