using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPoint;  // The player's current respawn position
    public string levelSelectorSceneName = "LevelSelector"; // Name of the Level Selector scene

    public int maxLives = 3; // Maximum lives per level
    private int currentLives;

    public Text livesText; // Reference to the UI Text element for lives display

    private bool isOnCooldown = false; // Cooldown flag
    public float deathCooldown = 1.0f; // Cooldown duration in seconds

    void Start()
    {
        // Set the initial respawn point to the player's starting position
        respawnPoint = transform.position;

        // Initialize lives
        currentLives = maxLives;

        // Display the initial lives count
        UpdateLivesUI();
    }

    void Update()
    {
        // Check if the player falls below a certain Y level
        if (transform.position.y < -30)
        {
            HandleDeath();
        }
    }

    public void Respawn()
    {
        // Move the player to the last checkpoint (respawn point)
        transform.position = respawnPoint;
        Debug.Log("Respawning player to: " + respawnPoint);
    }

    private void HandleDeath()
    {
        // Check if the player is on cooldown
        if (isOnCooldown)
        {
            Debug.Log("Player is on cooldown. Ignoring death.");
            return;
        }

        // Start the cooldown
        StartCoroutine(DeathCooldown());

        // Decrement lives
        currentLives--;

        if (currentLives > 0)
        {
            Debug.Log("Player died. Lives remaining: " + currentLives);
            UpdateLivesUI();
            Respawn();
        }
        else
        {
            Debug.Log("No lives remaining. Returning to level selector.");
            UpdateLivesUI(); // Update UI to show 0 lives before transition
            ReturnToLevelSelector();
        }
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            // Update the UI text to reflect current lives
            livesText.text = "Lives: " + currentLives;
            Debug.Log("Updating Lives UI to: " + currentLives);
        }
        else
        {
            Debug.LogError("LivesText is not assigned in the Inspector! Please assign the LivesText UI element.");
        }
    }

    private void ReturnToLevelSelector()
    {
        // Load the Level Selector scene
        SceneManager.LoadScene(levelSelectorSceneName);
    }

    private System.Collections.IEnumerator DeathCooldown()
    {
        // Set the cooldown flag to true
        isOnCooldown = true;

        // Wait for the cooldown duration
        yield return new WaitForSeconds(deathCooldown);

        // Reset the cooldown flag
        isOnCooldown = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player collided with a checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            if (checkpoint != null && checkpoint.isLastCheckpoint)
            {
                // If it's the last checkpoint, handle animation and transition
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
