using UnityEngine;
using UnityEngine.InputSystem; // new input system

public class PauseHotkey : MonoBehaviour
{
    private void Update()
    {
        // Ensure a keyboard exists and the Game view is focused
        if (Keyboard.current == null) return;

        // Space toggles pause/resume
        if (Keyboard.current.spaceKey.wasPressedThisFrame && TimeService.Instance != null)
        {
            if (TimeService.Instance.IsPaused) TimeService.Instance.Resume();
            else TimeService.Instance.Pause();
        }
    }
}
