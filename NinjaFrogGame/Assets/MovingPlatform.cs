using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] points; // Array of waypoints
    public float speed = 2f;   // Speed of the platform
    private int currentPointIndex = 0; // Current waypoint index
    private bool movingForward = true; // Determines direction of movement

    void Update()
    {
        // Ensure there are at least 2 waypoints
        if (points.Length < 2)
        {
            Debug.LogWarning("You need at least 2 waypoints for the platform to move.");
            return;
        }

        // Move the platform towards the current waypoint
        transform.position = Vector2.MoveTowards(transform.position, points[currentPointIndex].position, speed * Time.deltaTime);

        // Check if the platform has reached the current waypoint
        if (Vector2.Distance(transform.position, points[currentPointIndex].position) < 0.1f)
        {
            // Determine the next waypoint or reverse direction
            if (movingForward)
            {
                currentPointIndex++; // Move to the next waypoint
                if (currentPointIndex >= points.Length) // If it's beyond the last point
                {
                    movingForward = false; // Reverse direction
                    currentPointIndex = points.Length - 2; // Go to the second-to-last point
                }
            }
            else
            {
                currentPointIndex--; // Move to the previous waypoint
                if (currentPointIndex < 0) // If it's before the first point
                {
                    movingForward = true; // Reverse direction
                    currentPointIndex = 1; // Go to the second point
                }
            }
        }
    }
}




