#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Interlinked.Simulation.Stations;
using Interlinked.Domain.Primitives;

public static class StationRegistrySmoke
{
    [MenuItem("Interlinked/Run StationRegistry Smoke Test")]
    public static void Run()
    {
        var reg = new StationRegistry();
        var a = reg.CreateAt(new Vec2(1.2f, 3.4f));
        var b = reg.CreateAt(new Vec2(5f, -2f));
        reg.Rename(a.Id.ToString(), "Custom A");
        bool deleted = reg.Delete(b.Id.ToString());

        Debug.Log($"Created: {a.Name} at {a.Position}. Deleted second? {deleted}");
        foreach (var s in reg.All) Debug.Log($"Remaining: {s.Id} {s.Name} {s.Position}");
        EditorUtility.DisplayDialog("Smoke Test", "StationRegistry ran. Check Console logs.", "OK");
    }
}
#endif
