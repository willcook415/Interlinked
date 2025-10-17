using UnityEngine;

public class TimeTester : MonoBehaviour
{
    private void Update()
    {
        if (TimeService.Instance != null)
            Debug.Log($"Time: {TimeService.Instance.ElapsedTime:F2}s");
    }
}
