using UnityEngine;
using Interlinked.Domain.Primitives;
using Interlinked.Simulation.Stations;

namespace Interlinked.Presentation.Stations
{
    public sealed class StationPlacementController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Camera worldCamera;
        [SerializeField] private GameObject stationPrefab;
        [SerializeField] private StationRegistryProvider registryProvider;

        [Header("Settings")]
        [SerializeField] private float snapGrid = 0.5f;
        [SerializeField] private LayerMask stationLayer;

        private BuildInput _input; // <-- from the asset we just generated
        private StationRegistry Registry => registryProvider.Instance;

        private void Awake()
        {
            if (worldCamera == null) worldCamera = Camera.main;
            _input = new BuildInput();
        }

        private void OnEnable() => _input.Enable();
        private void OnDisable() => _input.Disable();

        private void Update()
        {
            var screen = _input.Build.Point.ReadValue<UnityEngine.Vector2>();
            var world = worldCamera.ScreenToWorldPoint(
                new Vector3(screen.x, screen.y, -worldCamera.transform.position.z));

            var pos = new Vec2(world.x, world.y);
            if (_input.Build.Snap.IsPressed())
                pos = pos.Snap(snapGrid);

            if (_input.Build.Place.WasPerformedThisFrame())
                PlaceAt(pos);

            if (_input.Build.Delete.WasPerformedThisFrame())
                TryDeleteAt(world);

            if (_input.Build.Rename.WasPerformedThisFrame())
                TryRenameAt(world);
        }

        private void PlaceAt(Vec2 pos)
        {
            var s = Registry.CreateAt(pos);
            var go = Instantiate(stationPrefab);
            var view = go.GetComponent<StationView>();
            view.Bind(s);
        }

        private void TryDeleteAt(Vector3 world)
        {
            var hit = Physics2D.OverlapPoint(world, stationLayer);
            if (hit && hit.TryGetComponent<StationView>(out var view))
            {
                Registry.Delete(view.Id);
                Destroy(view.gameObject);
            }
        }

        private void TryRenameAt(Vector3 world)
        {
#if UNITY_EDITOR
            var hit = Physics2D.OverlapPoint(world, stationLayer);
            if (hit && hit.TryGetComponent<StationView>(out var view))
            {
                var ok = UnityEditor.EditorUtility.DisplayDialog(
                    "Rename Station",
                    $"Current: {view.StationName}\n(Stub rename for now)",
                    "Append (Renamed)", "Cancel");
                if (ok)
                {
                    var newName = view.StationName + " (Renamed)";
                    Registry.Rename(view.Id, newName);
                    view.UpdateName(newName);
                }
            }
#endif
        }
    }
}
