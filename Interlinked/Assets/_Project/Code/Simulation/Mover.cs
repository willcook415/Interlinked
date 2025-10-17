using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float speed = 3f;   // units per second
    [SerializeField] private Vector2 direction = Vector2.right;
    [SerializeField] private float leftX = -6f;
    [SerializeField] private float rightX = 6f;

    private void Update()
    {
        var dt = TimeService.Instance ? TimeService.Instance.DeltaTime : Time.deltaTime;
        transform.position += (Vector3)(direction.normalized * speed * dt);

        // simple bounce at screen edges
        if (transform.position.x > rightX) { transform.position = new Vector3(rightX, transform.position.y, 0); direction = Vector2.left; }
        if (transform.position.x < leftX) { transform.position = new Vector3(leftX, transform.position.y, 0); direction = Vector2.right; }
    }
}
