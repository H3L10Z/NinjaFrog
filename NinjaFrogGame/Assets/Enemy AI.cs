using UnityEngine;

public class EnemySlimeAI : MonoBehaviour
{
    public float jumpForce = 5f;
    public float jumpCooldown = 1.5f;
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isChasing = false;
    private bool isGrounded = false;
    private float jumpTimer = 0f;

    // Add reference to PlayerRespawn script
    public PlayerRespawn playerRespawn; // Reference to the PlayerRespawn script to handle player death

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Ensure the player has the "Player" tag.

        if (playerRespawn == null)
        {
            Debug.LogWarning("PlayerRespawn reference not set on the slime!");
        }
    }

    void Update()
    {
        // Check if grounded (simplified with raycast)
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

        // Handle jumping behavior
        jumpTimer += Time.deltaTime;

        if (jumpTimer >= jumpCooldown && isGrounded)
        {
            if (isChasing)
            {
                JumpTowardsPlayer();
            }
            else
            {
                RandomJump();
            }
            jumpTimer = 0f;
        }

        // Detect player
        DetectPlayer();

        // Set animator states
        UpdateAnimations();
    }

    void RandomJump()
    {
        float randomDirection = Random.Range(-1f, 1f); // Random horizontal direction
        Vector2 jumpDirection = new Vector2(randomDirection, 1f).normalized;
        rb.velocity = new Vector2(0, 0); // Reset any lingering velocity
        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
    }

    void JumpTowardsPlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 jumpDirection = new Vector2(directionToPlayer.x, 1f).normalized;
        rb.velocity = new Vector2(0, 0); // Reset any lingering velocity
        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }
    }

    void UpdateAnimations()
    {
        if (!isGrounded)
        {
            animator.SetBool("isJumping", rb.velocity.y > 0);
            animator.SetBool("isFalling", rb.velocity.y < 0);
        }
        else
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
    }

    // Detect when the slime collides with the player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // If the slime collides with the player, trigger death
            Debug.Log("Slime touched player! Triggering death.");
            
            // Call the HandleDeath method from the PlayerRespawn script
            if (playerRespawn != null)
            {
                playerRespawn.HandleDeath(); // Handle player death immediately
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range for debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
