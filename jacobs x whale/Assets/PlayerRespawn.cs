using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPoint;  // The player's current respawn position

    void Start()
    {
        // Set the initial respawn point to the player's starting position
        respawnPoint = transform.position;
    }

    void Update()
    {
        // Check if the player falls below a certain Y level
        if (transform.position.y < -30)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        // Move the player to the last checkpoint (respawn point)
        transform.position = respawnPoint;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player collided with a checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            // Update the respawn point to the checkpoint's position
            respawnPoint = other.transform.position;

            // Find the Animator on the checkpoint and play the activation animation
            Animator checkpointAnimator = other.GetComponentInChildren<Animator>();
            if (checkpointAnimator != null)
            {
                checkpointAnimator.SetTrigger("ActivateFlag");
            }
        }
    }
}
