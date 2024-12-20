using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform player;          // Reference to the player's transform
    public float parallaxFactor;      // How much parallax effect is applied (1.0 means no parallax, 0 means no movement)
    public float parallaxSpeed;       // Speed at which the background layer moves (higher is faster)

    private Vector3 previousPlayerPosition;

    void Start()
    {
        // Store the player's initial position
        previousPlayerPosition = player.position;
    }

    void Update()
    {
        // Calculate how far the player has moved since last frame
        float movementDelta = player.position.x - previousPlayerPosition.x;

        // Apply parallax effect based on the player's movement
        Vector3 newPosition = transform.position;
        newPosition.x += movementDelta * parallaxFactor * parallaxSpeed;

        // Update the position of the background
        transform.position = newPosition;

        // Update the player's previous position for the next frame
        previousPlayerPosition = player.position;
    }
}
