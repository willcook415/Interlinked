using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    private IEnumerator Start()
    {
        // Load the main Game scene additively
        var load = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
        while (!load.isDone) yield return null;

        // Make Game the active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameSceneName));

        // Unload Bootstrap to keep things clean
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
