using UnityEngine;
using System.Collections;

public class RespawnManager : MonoBehaviour
{
    public Transform player;
    public FastTravelManager fastTravelManager;
    public string respawnPointName = "DefaultSpawn";

    private healthSystem playerHealth;
    public GameObject deathMenuUI;

    private void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("RespawnManager: Player not assigned and no GameObject with tag 'Player' found.");
        }
    }

    private void Start()
    {
        if (player == null) return;

        playerHealth = player.GetComponent<healthSystem>();
        if (playerHealth != null)
        {
            playerHealth.OnDeath += () => StartCoroutine(HandlePlayerDeath());
        }
        else
        {
            Debug.LogError("RespawnManager: Player does not have a healthSystem component!");
        }

        deathMenuUI.SetActive(false);
        LockCursor(true);
    }

    private IEnumerator HandlePlayerDeath()
    {
        yield return new WaitForSeconds(5f);

        Time.timeScale = 0f;
        deathMenuUI.SetActive(true);
        LockCursor(false);
    }

    public void Respawn()
    {
        Time.timeScale = 1f;
        deathMenuUI.SetActive(false);

        player.gameObject.SetActive(true);

        playerHealth.UpdateMaxHealth();
        playerHealth.ForceSetHealth(playerHealth.maxHealth);

        fastTravelManager.FastTravelTo(respawnPointName);

        LockCursor(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        LockCursor(true);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void LockCursor(bool shouldLock)
    {
        Cursor.visible = !shouldLock;
        Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
