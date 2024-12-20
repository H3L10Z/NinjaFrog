using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPoint;  // The player's current respawn position
    public string levelSelectorSceneName = "LevelSelector"; // Name of the Level Selector scene

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
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            if (checkpoint != null && checkpoint.isLastCheckpoint)
            {
                // If it's the last checkpoint, trigger animation and then transition
                HandleLastCheckpoint(other.GetComponentInChildren<Animator>());
                return;
            }

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

    private void HandleLastCheckpoint(Animator checkpointAnimator)
    {
        // Play the checkpoint's animation if it exists
        if (checkpointAnimator != null)
        {
            checkpointAnimator.SetTrigger("ActivateFlag");
        }

        // Delay the transition to the Level Selector after animation
        StartCoroutine(LastCheckpointDelay());
    }

    private System.Collections.IEnumerator LastCheckpointDelay()
    {
        // Wait for 2-3 seconds (adjust this duration to match the animation length)
        float animationDuration = 2.8f; // Adjust this to match your animation length
        yield return new WaitForSeconds(animationDuration);

        // Load the Level Selector scene
        SceneManager.LoadScene(levelSelectorSceneName);
    }
}
