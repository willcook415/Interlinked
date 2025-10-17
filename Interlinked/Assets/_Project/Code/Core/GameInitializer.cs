using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    private IEnumerator Start()
    {
        var load = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
        while (!load.isDone) yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameSceneName));
        yield return SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}
