using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI coinText;

    [Header("Player Components")]
    public Animator animator;
    public PlayerMovement playerMovement;
    public Rigidbody2D rb;

    [Header("Respawn Settings")]
    public string levelSelectorScene = "LevelSelector";
    public float respawnDelay = 1f;
    public float invincibilityDuration = 2f;
    public float fallThreshold = -30f;

    private Vector3 checkpointPosition;
    private int lives = 3;
    private int coins = 0;
    private bool isRespawning = false;
    private bool isInvincible = false;

    public bool IsInvulnerable { get { return isInvincible; } }

    private void Start()
    {
        checkpointPosition = transform.position;
        UpdateUI();
    }

    private void Update()
    {
        if (transform.position.y < fallThreshold && !isRespawning)
        {
            HandleDeath();
        }
    }

    public void HandleDeath()
    {
        if (isRespawning || isInvincible) return;
        
        StartCoroutine(HandleDeathRoutine());
    }

    private IEnumerator HandleDeathRoutine()
    {
        isRespawning = true;
        
        // Disable movement and physics
        if (playerMovement != null) 
        {
            playerMovement.enabled = false;
        }
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Trigger death animation using the correct parameter name
        if (animator != null)
        {
            animator.SetBool("IsJumping", false);  // Reset jump state
            animator.SetFloat("Speed", 0f);        // Reset speed
            animator.SetTrigger("Die");           // Correct trigger for death animation
        }

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            SceneManager.LoadScene(levelSelectorScene);
            yield break;
        }

        // Wait for death animation
        yield return new WaitForSeconds(respawnDelay);

        // Reset position
        transform.position = checkpointPosition;

        // Re-enable physics
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Reset animation state
        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.Play("Player_Idle"); // Match your idle animation state name
        }

        // Re-enable movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        StartCoroutine(InvincibilityPeriod());
        isRespawning = false;
    }

    private IEnumerator InvincibilityPeriod()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void UpdateUI()
    {
        if (livesText != null) livesText.text = "Lives: " + lives;
        if (coinText != null) coinText.text = "Coins: " + coins;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            checkpointPosition = other.transform.position;
            var checkpointAnim = other.GetComponentInChildren<Animator>();
            if (checkpointAnim != null)
            {
                checkpointAnim.SetTrigger("ActivateFlag");
            }
        }
        else if (other.CompareTag("Hazard") && !isInvincible && !isRespawning)
        {
            HandleDeath();
        }
        else if (other.CompareTag("Coin"))
        {
            var coin = other.GetComponent<Coin>();
            if (coin != null && !coin.isPickedUp)
            {
                coin.PickUp();
                coins++;
                UpdateUI();
            }
        }
    }
}
