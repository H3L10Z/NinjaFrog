using UnityEngine;

public class Coin : MonoBehaviour
{
    private Animator animator;
    public float destroyDelay = 0.5f; // Time to wait before destroying the coin after pickup
    private Collider2D coinCollider; // Reference to the coin's collider
    public bool isPickedUp = false; // Flag to prevent multiple pickups

    void Start()
    {
        animator = GetComponent<Animator>();
        coinCollider = GetComponent<Collider2D>();

        if (animator == null)
        {
            Debug.LogWarning("No Animator component found on the coin!");
        }

        if (coinCollider == null)
        {
            Debug.LogWarning("No Collider2D component found on the coin!");
        }
    }

    public void PickUp()
    {
        if (isPickedUp) return; // Prevent multiple pickups

        isPickedUp = true; // Mark the coin as picked up

        Debug.Log("Coin picked up!");

        // Disable the collider to prevent additional triggers
        if (coinCollider != null)
        {
            coinCollider.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Pickup");
            Debug.Log("Playing Pickup animation.");
        }

        // Destroy the coin after the animation
        StartCoroutine(DestroyAfterAnimation());
    }

    private System.Collections.IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
