using UnityEngine;

public class WinZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered Win Zone — Triggering WinGame()");

            if (gameManager.Instance != null)
            {
                gameManager.Instance.WinGame();
            }
            else
            {
                Debug.LogError("No GameManager instance found!");
            }
        }
    }
}