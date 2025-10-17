using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform[] waypoints;   // assign Stop_A, Stop_B (in order)
    [SerializeField] private bool pingPong = true;     // reverse at ends

    [Header("Motion")]
    [SerializeField] private float speed = 3f;         // units/sec
    [SerializeField] private float arriveThreshold = 0.02f;

    [Header("Dwell")]
    [SerializeField] private float dwellSeconds = 2.0f;

    private int _index = 0;        // current target waypoint index
    private int _dir = +1;         // +1 forward, -1 reverse
    private float _dwellTimer = 0; // >0 while dwelling
    private bool _hasStarted;      // avoids teleporting on frame 0

    private void Start()
    {
        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogError("WaypointMover: need at least 2 waypoints.", this);
            enabled = false;
            return;
        }

        // snap to first waypoint on start
        transform.position = waypoints[0].position;
        _index = 1;
        _hasStarted = true;
    }

    private void Update()
    {
        float dt = TimeService.Instance ? TimeService.Instance.DeltaTime : Time.deltaTime;
        if (!_hasStarted || dt <= 0f) return;

        // dwell at stops
        if (_dwellTimer > 0f)
        {
            _dwellTimer -= dt;
            return;
        }

        // move towards current target
        Vector3 target = waypoints[_index].position;
        Vector3 pos = transform.position;
        Vector3 to = target - pos;
        float dist = to.magnitude;

        if (dist <= arriveThreshold)
        {
            // Arrived: snap, start dwell, choose next target
            transform.position = target;
            _dwellTimer = dwellSeconds;
            Debug.Log($"Arrived at {_index}, dwell {_dwellTimer}");

            // choose next index
            int next = _index + _dir;

            if (next < 0 || next >= waypoints.Length)
            {
                if (pingPong)
                {
                    _dir = -_dir;
                    next = _index + _dir;
                }
                else
                {
                    next = 0; // loop
                }
            }

            _index = next;
            return;
        }

        // advance towards target
        Vector3 step = to.normalized * speed * dt;
        if (step.magnitude > dist) step = to; // don’t overshoot
        transform.position = pos + step;
    }
}
