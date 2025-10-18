using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Interlinked.Domain.Primitives;
using Interlinked.Domain.Stations;
using Interlinked.Domain.Lines;
using Interlinked.Simulation.Stations;
using Interlinked.Simulation.Lines;
using Interlinked.Presentation.Stations;  // <-- add this


namespace Interlinked.Presentation.Lines
{
    public sealed class LineDraftController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Camera worldCamera;
        [SerializeField] private StationRegistryProvider stationRegistryProvider;
        [SerializeField] private LineRegistryProvider lineRegistryProvider;
        [SerializeField] private Material ghostMaterial;
        [SerializeField] private Material finalMaterial;
        [SerializeField] private LayerMask stationLayer;

        [Header("Settings")]
        [SerializeField] private float snapGrid = 0.5f;
        [SerializeField] private float width = 0.1f;
        [SerializeField] private bool horizontalFirst = true;

        private BuildInput _input;
        private StationRegistry Stations => stationRegistryProvider.Instance;
        private LineRegistry Lines => lineRegistryProvider.Instance;

        private StationView _start;     // chosen start station view
        private GameObject _ghostGO;    // live ghost object
        private LineView _ghostView;

        private bool _lineMode;

        private void Awake()
        {
            if (worldCamera == null) worldCamera = Camera.main;
            _input = new BuildInput();
        }
        private void OnEnable() { _input.Enable(); }
        private void OnDisable() { _input.Disable(); CancelDraft(); }

        private void Update()
        {
            // Toggle line mode
            if (_input.Build.LineMode.WasPerformedThisFrame())
                _lineMode = !_lineMode;

            if (!_lineMode) { CancelDraft(); return; }

            var screen = _input.Build.Point.ReadValue<Vector2>();
            var world = worldCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -worldCamera.transform.position.z));
            var cursor = new Vec2(world.x, world.y);

            if (_input.Build.Delete.WasPerformedThisFrame() || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CancelDraft();
                return;
            }

            if (_start == null)
            {
                // waiting for start station
                if (_input.Build.Place.WasPerformedThisFrame())
                {
                    var hit = Physics2D.OverlapPoint(world, stationLayer);
                    if (hit && hit.TryGetComponent<StationView>(out var view))
                    {
                        _start = view;
                        EnsureGhost();
                    }
                }
                return;
            }

            // update ghost from _start to cursor
            EnsureGhost();
            var a = ToVec2(_start.transform.position);
            var pathToCursor = ManhattanPath.Build(a, cursor, snapGrid, horizontalFirst);
            _ghostView.SetPoints(pathToCursor);

            if (_input.Build.Place.WasPerformedThisFrame())
            {
                // need an end station to commit
                var hit = Physics2D.OverlapPoint(world, stationLayer);
                if (hit && hit.TryGetComponent<StationView>(out var end))
                {
                    CommitLine(_start, end);
                    CancelDraft(); // ready for next line
                }
            }
        }

        private void CommitLine(StationView aView, StationView bView)
        {
            var aPos = ToVec2(aView.transform.position);
            var bPos = ToVec2(bView.transform.position);
            var pts = ManhattanPath.Build(aPos, bPos, snapGrid, horizontalFirst);

            // Domain + sim
            var id = LineId.New();
            var name = Lines.NextName();
            var line = new Interlinked.Domain.Lines.Line(id, name,
                new StationId(new System.Guid(aView.Id)),
                new StationId(new System.Guid(bView.Id)),
                pts);
            Lines.Create(line);

            // Presentation
            var go = new GameObject($"Line [{name}]");
            var lv = go.AddComponent<LineView>();
            var lr = go.GetComponent<LineRenderer>();
            lr.widthMultiplier = width;
            lv.SetMaterial(finalMaterial);
            lv.SetPoints(pts);
        }

        private void EnsureGhost()
        {
            if (_ghostGO != null) return;
            _ghostGO = new GameObject("LineGhost");
            _ghostView = _ghostGO.AddComponent<LineView>();
            var lr = _ghostGO.GetComponent<LineRenderer>();
            lr.widthMultiplier = width;
            _ghostView.SetMaterial(ghostMaterial);
        }

        private void CancelDraft()
        {
            _start = null;
            if (_ghostGO != null) Destroy(_ghostGO);
            _ghostGO = null;
            _ghostView = null;
        }

        private static Vec2 ToVec2(Vector3 v) => new Vec2(v.x, v.y);
    }
}
