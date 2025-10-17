using UnityEngine;

public class TimeService : MonoBehaviour
{
    public static TimeService Instance { get; private set; }

    public float DeltaTime { get; private set; }
    public float ElapsedTime { get; private set; }
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // keeps it alive between scenes
    }

    private void Update()
    {
        if (IsPaused)
        {
            DeltaTime = 0f;
            return;
        }

        DeltaTime = Time.deltaTime;
        ElapsedTime += DeltaTime;
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;
}
