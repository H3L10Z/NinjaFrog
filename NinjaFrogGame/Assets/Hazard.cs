using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit a hazard. Triggering HandleDeath.");
            collision.GetComponent<PlayerRespawn>()?.HandleDeath();
        }
    }
}

