using UnityEngine;
using Interlinked.Domain.Primitives;
using Interlinked.Simulation.Stations;
using Interlinked.UI;
using UnityEngine.InputSystem;


namespace Interlinked.Presentation.Stations
{
    public sealed class StationPlacementController : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Camera worldCamera;
        [SerializeField] private GameObject stationPrefab;
        [SerializeField] private StationRegistryProvider registryProvider;
        [SerializeField] private StationRenameOverlay renameOverlay; // <-- TMP overlay

        [Header("Settings")]
        [SerializeField] private float snapGrid = 0.5f;
        [SerializeField] private LayerMask stationLayer;
        [SerializeField] private float hoverScale = 1.25f;

        private BuildInput _input;               // from Build.inputactions
        private StationRegistry Registry => registryProvider.Instance;
        private StationView _hover;              // current hover target

        private void Awake()
        {
            if (worldCamera == null) worldCamera = Camera.main;
            _input = new BuildInput();

            // Failsafe: auto-find overlay in scene if not wired
            if (renameOverlay == null)
                renameOverlay = FindObjectOfType<StationRenameOverlay>(true);
            RuntimeRenameUI.GetOrCreate();
        }

        private void OnEnable() => _input.Enable();
        private void OnDisable() => _input.Disable();

        private void Update()
        {
            var screen = _input.Build.Point.ReadValue<Vector2>();
            var world = worldCamera.ScreenToWorldPoint(
                new Vector3(screen.x, screen.y, -worldCamera.transform.position.z));

            UpdateHover(world);

            var kb = Keyboard.current;
            var modal = RuntimeRenameUI.Instance;
            if (kb != null && modal != null)
            {
                if (kb.f1Key.wasPressedThisFrame)
                    modal.Open("Station (debug)", null, _ => Debug.Log("[Rename] Confirmed (debug)"));

                if (modal.IsOpen)
                {
                    if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame)
                        modal.Close(); // confirm is handled by OK button; keep close simple here
                    else if (kb.escapeKey.wasPressedThisFrame)
                        modal.Close();
                    return; // block build inputs while modal open
                }
            }




            // --- Hover highlight (scale up when cursor is over a station) ---
            UpdateHover(world);

            // If rename popup is open, ignore placement inputs
            if (renameOverlay != null && renameOverlay.IsOpen) return;

            var pos = new Vec2(world.x, world.y);
            if (_input.Build.Snap.IsPressed()) pos = pos.Snap(snapGrid);

            if (_input.Build.Place.WasPerformedThisFrame()) PlaceAt(pos);
            if (_input.Build.Delete.WasPerformedThisFrame()) TryDeleteAt(world);
            if (_input.Build.Rename.WasPerformedThisFrame()) TryRenameAt(world, screen);
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

        

        private void UpdateHover(Vector3 world)
        {
            var hit = Physics2D.OverlapPoint(world, stationLayer);
            StationView next = null;
            if (hit) hit.TryGetComponent(out next);

            if (_hover == next) return;

            // reset previous using its BaseScale
            if (_hover != null) _hover.transform.localScale = _hover.BaseScale;

            _hover = next;

            // scale hovered relative to its BaseScale (no size drift)
            if (_hover != null) _hover.transform.localScale = _hover.BaseScale * hoverScale;
        }

        private void ClearHover()
        {
            if (_hover != null) _hover.transform.localScale = _hover.BaseScale;
            _hover = null;
        }

        private void TryRenameAt(Vector3 world, Vector2 screen)
        {
            var hit = Physics2D.OverlapPoint(world, stationLayer);
            var modal = RuntimeRenameUI.Instance;
            if (!hit || modal == null) return;

            if (hit.TryGetComponent<StationView>(out var view))
            {
                ClearHover();
                modal.Open(view.StationName, screen, (newName) =>
                {
                    Registry.Rename(view.Id, newName);
                    view.UpdateName(newName);
                });
            }
        }


    }
}
