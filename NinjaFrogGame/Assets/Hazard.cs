using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collides with the hazard
        if (collision.CompareTag("Player"))
        {
            PlayerRespawn playerRespawn = collision.GetComponent<PlayerRespawn>();
            if (playerRespawn != null)
            {
                Debug.Log("Player hit a hazard. Triggering HandleDeath.");
                playerRespawn.SendMessage("HandleDeath");
            }
            else
            {
                Debug.LogError("PlayerRespawn script is not attached to the Player object!");
            }
        }
    }
}
