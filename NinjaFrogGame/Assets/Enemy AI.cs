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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Ensure the player has the "Player" tag.
    }

    void Update()
    {
        // Check if grounded (simplified with raycast)
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, LayerMask.GetMask("Ground"));

        Debug.Log("Is Grounded: " + isGrounded);


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

    private void OnDrawGizmosSelected()
    {
        // Draw detection range for debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}