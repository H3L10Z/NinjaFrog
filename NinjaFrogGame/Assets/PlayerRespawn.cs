using UnityEngine;
using TMPro; // Ensure TextMeshPro is imported
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI coinText;
    public Animator playerAnimator; // Reference to the player's Animator

    private Vector3 respawnPoint; // The player's respawn position
    public string levelSelectorSceneName = "LevelSelector"; // Level selector scene

    private int lives = 3;
    private int coins = 0;

    private bool isOnCooldown = false; // Cooldown flag
    public float cooldownTime = 1f; // Cooldown duration
    private bool isInvulnerable = false; // Invulnerability flag
    public float invulnerabilityTime = 2f; // Time in seconds for invulnerability after respawn
    private bool isFalling = false; // Flag to track if the player is falling

    private float fallThreshold = -20f; // Y position where the player dies if they fall below

    void Start()
    {
        respawnPoint = transform.position;
        UpdateLivesUI();
        UpdateCoinsUI();
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
            Debug.Log("Updating Lives UI to: " + lives);
        }
    }

    private void UpdateCoinsUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coins;
            Debug.Log("Updating Coins UI to: " + coins);
        }
    }

    public void HandleDeath()
    {
        if (isOnCooldown || isInvulnerable || isFalling) return; // Prevent multiple deaths

        Debug.Log("Player hit a hazard. Triggering HandleDeath.");

        // Trigger death animation
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die"); // Make sure you set up a "Die" trigger in the Animator
        }

        lives--;

        if (lives <= 0)
        {
            SceneManager.LoadScene(levelSelectorSceneName); // Load level selector when no lives left
        }
        else
        {
            // Start cooldown before respawning
            StartCoroutine(DeathCooldown());
        }

        StartCoroutine(Cooldown());
    }

    private System.Collections.IEnumerator DeathCooldown()
    {
        // Play death animation for a short time before respawning
        float deathAnimationDuration = 1f; // Adjust this time to match the death animation length
        yield return new WaitForSeconds(deathAnimationDuration); // Wait for the death animation to play

        // Reset death trigger and transition to idle animation
        playerAnimator.ResetTrigger("Die"); // Reset the "Die" trigger to stop death animation
        playerAnimator.SetTrigger("Idle"); // Transition to idle or another state after death animation

        // Respawn the player at the last checkpoint
        transform.position = respawnPoint;
        UpdateLivesUI();

        // Enable invulnerability for a short time after respawning
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime); // Wait for the invulnerability time to end
        isInvulnerable = false; // Disable invulnerability after the time is over
    }

    private System.Collections.IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            respawnPoint = other.transform.position;

            Animator checkpointAnimator = other.GetComponentInChildren<Animator>();
            if (checkpointAnimator != null)
            {
                checkpointAnimator.SetTrigger("ActivateFlag");
            }
        }
        else if (other.CompareTag("Hazard") && !isInvulnerable && !isFalling) // Check if player is invulnerable
        {
            HandleDeath(); // Only trigger death if player is not invulnerable
        }
        else if (other.CompareTag("Coin"))
        {
            Coin coin = other.GetComponent<Coin>();
            if (coin != null && !coin.isPickedUp) // Prevent multiple pickups
            {
                coin.PickUp();
                coins++;
                UpdateCoinsUI();
            }
        }
    }

    void Update()
    {
        // Check if the player falls below the fall threshold (Y position)
        if (transform.position.y < fallThreshold && !isFalling)
        {
            isFalling = true; // Mark the player as falling
            HandleFall(); // Call HandleFall instead of HandleDeath for falling
        }
    }

    // New method to handle falling
    private void HandleFall()
    {
        if (lives > 0)
        {
            lives--; // Decrease lives when the player falls
            UpdateLivesUI();
            StartCoroutine(RespawnAfterFall());
        }
        else
        {
            SceneManager.LoadScene(levelSelectorSceneName); // Go to level selector if no lives are left
        }
    }

    // Coroutine to respawn the player after falling
    private System.Collections.IEnumerator RespawnAfterFall()
    {
        // Play death animation briefly
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        // Wait for a short time before respawning
        float fallRespawnDelay = 1f; // Delay before respawning
        yield return new WaitForSeconds(fallRespawnDelay);

        // Reset death animation and respawn player
        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("Die");
            playerAnimator.SetTrigger("Idle"); // Transition to idle state after death
        }

        transform.position = respawnPoint; // Respawn the player at the last checkpoint
        isFalling = false; // Reset falling state
        isInvulnerable = true; // Set invulnerability after respawn
        yield return new WaitForSeconds(invulnerabilityTime); // Wait for invulnerability time
        isInvulnerable = false; // Disable invulnerability after the time is over
    }
}
