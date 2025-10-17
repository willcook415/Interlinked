#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CleanupMissingPrefabs
{
    [MenuItem("Interlinked/Cleanup/Remove Missing Prefab Instances (Current Scene)")]
    public static void RemoveMissingPrefabInstances()
    {
        var scene = SceneManager.GetActiveScene();
        int removed = 0;

        foreach (var root in scene.GetRootGameObjects())
            Traverse(root.transform, t =>
            {
                var status = PrefabUtility.GetPrefabInstanceStatus(t.gameObject);
                if (status == PrefabInstanceStatus.MissingAsset)
                {
                    Debug.LogWarning($"[Cleanup] Removing missing prefab instance: {t.name}", t);
                    Object.DestroyImmediate(t.gameObject);
                    removed++;
                    return false; // stop traversing this branch (we just destroyed it)
                }
                return true;
            });

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log($"[Cleanup] Done. Removed {removed} missing prefab instance(s) in scene '{scene.name}'.");
    }

    private static void Traverse(Transform tr, System.Func<Transform, bool> visit)
    {
        if (!visit(tr)) return;
        for (int i = tr.childCount - 1; i >= 0; i--)
            Traverse(tr.GetChild(i), visit);
    }
}
#endif
