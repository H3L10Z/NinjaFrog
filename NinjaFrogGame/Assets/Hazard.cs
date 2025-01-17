using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Ensure the player is not in the middle of a death animation or invulnerable state
            PlayerRespawn playerRespawn = collision.GetComponent<PlayerRespawn>();
            if (playerRespawn != null && !playerRespawn.IsInvulnerable)
            {
                Debug.Log("Player hit a hazard. Triggering HandleDeath.");
                playerRespawn.HandleDeath();
            }
        }
    }
}


