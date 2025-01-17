using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI coinText;
    public Animator playerAnimator;
    public MonoBehaviour movementScript; // Reference to the player's movement script
    public Rigidbody2D playerRigidbody; // Reference to the player's Rigidbody2D (for physics)

    private Vector3 respawnPoint;
    public string levelSelectorSceneName = "LevelSelector";

    private int lives = 3;
    private int coins = 0;

    private bool isOnCooldown = false;
    public float cooldownTime = 1f;
    private bool isInvulnerable = false;
    public float invulnerabilityTime = 2f;
    private bool isFalling = false;

    private float fallThreshold = -30f;
    public float deathAnimationDuration = 1f;

    private bool isMovementLocked = false; // New flag to block movement

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
        }
    }

    private void UpdateCoinsUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coins;
        }
    }

    public void HandleDeath()
    {
        if (isOnCooldown || isInvulnerable || isFalling) return;

        LockMovement(); // Block movement

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        lives--;

        if (lives <= 0)
        {
            SceneManager.LoadScene(levelSelectorSceneName);
        }
        else
        {
            StartCoroutine(DeathCooldown());
        }

        StartCoroutine(Cooldown());
    }

    private System.Collections.IEnumerator DeathCooldown()
    {
        yield return new WaitForSeconds(deathAnimationDuration);

        playerAnimator.ResetTrigger("Die");
        playerAnimator.SetTrigger("Idle");

        transform.position = respawnPoint;
        UpdateLivesUI();

        isInvulnerable = true;
        UnlockMovement(); // Allow movement again
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
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
        else if (other.CompareTag("Hazard") && !isInvulnerable && !isFalling)
        {
            HandleDeath();
        }
        else if (other.CompareTag("Coin"))
        {
            Coin coin = other.GetComponent<Coin>();
            if (coin != null && !coin.isPickedUp)
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
            isFalling = true;
            HandleFall();
        }
    }

    private void HandleFall()
    {
        if (lives > 0)
        {
            lives--;
            UpdateLivesUI();
            StartCoroutine(RespawnAfterFall());
        }
        else
        {
            SceneManager.LoadScene(levelSelectorSceneName);
        }
    }

    private System.Collections.IEnumerator RespawnAfterFall()
    {
        LockMovement();

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        yield return new WaitForSeconds(deathAnimationDuration);

        // Reset the animation state to Idle after death animation ends
        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("Die");
            playerAnimator.SetTrigger("Idle"); // Ensure player transitions to idle after death
        }

        transform.position = respawnPoint;
        isFalling = false;
        isInvulnerable = true;
        UnlockMovement();
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    private void LockMovement()
    {
        isMovementLocked = true;

        if (movementScript != null)
        {
            movementScript.enabled = false; // Disable movement script
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero; // Stop physics movement
        }
    }

    private void UnlockMovement()
    {
        isMovementLocked = false;

        if (movementScript != null)
        {
            movementScript.enabled = true; // Re-enable movement script
        }
    }

    void FixedUpdate()
    {
        if (isMovementLocked && playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero; // Continuously prevent movement
        }
    }
}
