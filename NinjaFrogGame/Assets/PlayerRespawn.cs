using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI gameOverText;
    
    [Header("Player Components")]
    public Animator animator;
    public PlayerMovement playerMovement;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    [Header("Respawn Settings")]
    public string levelSelectorScene = "LevelSelector";
    public float respawnDelay = 1f;
    public float invincibilityDuration = 2f;
    public float fallThreshold = -30f;
    public float invincibilityFlashRate = 0.15f;
    public float gameOverDelay = 2.5f;

    [Header("Game Over Animation Settings")]
    public float textFadeInDuration = 1f;
    public float textScaleInDuration = 0.5f;
    public float initialTextScale = 1.5f;
    public float finalTextScale = 1f;
    public Color textColor = Color.red;

    private Vector3 checkpointPosition;
    private int lives = 3;
    private int coins = 0;
    private bool isRespawning = false;
    private bool isInvincible = false;

    public bool IsInvulnerable { get { return isInvincible; } }

    private void Start()
    {
        checkpointPosition = transform.position;
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
            gameOverText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        }
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

        // Trigger death animation
        if (animator != null)
        {
            animator.SetBool("IsJumping", false);
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Die");
        }

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            // Show and animate "YOU DIED" text
            if (gameOverText != null)
            {
                yield return StartCoroutine(ShowGameOverText());
            }
            
            // Wait for the full animation and delay
            yield return new WaitForSeconds(gameOverDelay);
            
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
            animator.Play("Player_Idle");
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
        
        // Start the flashing coroutine
        StartCoroutine(FlashWhileInvincible());
        
        yield return new WaitForSeconds(invincibilityDuration);
        
        isInvincible = false;
        
        // Ensure sprite is fully visible when invincibility ends
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    private IEnumerator FlashWhileInvincible()
    {
        if (spriteRenderer == null) yield break;

        Color normalColor = Color.white;
        Color invincibleColor = new Color(1f, 1f, 1f, 0.5f);

        while (isInvincible)
        {
            spriteRenderer.color = invincibleColor;
            yield return new WaitForSeconds(invincibilityFlashRate);
            spriteRenderer.color = normalColor;
            yield return new WaitForSeconds(invincibilityFlashRate);
        }
    }

    private IEnumerator ShowGameOverText()
    {
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = "YOU DIED";
        
        // Reset text properties
        gameOverText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        gameOverText.transform.localScale = Vector3.one * initialTextScale;

        // Create a parallel animation for fade and scale
        StartCoroutine(FadeInText());
        StartCoroutine(ScaleText());

        // Wait for both animations to complete
        yield return new WaitForSeconds(Mathf.Max(textFadeInDuration, textScaleInDuration));
    }

    private IEnumerator FadeInText()
    {
        float elapsedTime = 0f;
        Color startColor = new Color(textColor.r, textColor.g, textColor.b, 0f);
        Color targetColor = new Color(textColor.r, textColor.g, textColor.b, 1f);

        while (elapsedTime < textFadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / textFadeInDuration;
            
            // Use smooth step for more dramatic fade
            float alpha = Mathf.SmoothStep(0f, 1f, normalizedTime);
            gameOverText.color = Color.Lerp(startColor, targetColor, alpha);
            
            yield return null;
        }

        gameOverText.color = targetColor;
    }

    private IEnumerator ScaleText()
    {
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.one * initialTextScale;
        Vector3 targetScale = Vector3.one * finalTextScale;

        while (elapsedTime < textScaleInDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / textScaleInDuration;
            
            // Use smooth step for more dramatic scaling
            float scaleProgress = Mathf.SmoothStep(0f, 1f, normalizedTime);
            gameOverText.transform.localScale = Vector3.Lerp(startScale, targetScale, scaleProgress);
            
            yield return null;
        }

        gameOverText.transform.localScale = targetScale;
    }

    private void UpdateUI()
    {
        if (livesText != null) livesText.text = "Lives: " + lives;
        if (coinText != null) coinText.text = "Cherry: " + coins;
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