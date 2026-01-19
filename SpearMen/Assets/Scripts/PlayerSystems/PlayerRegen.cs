using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRegen : MonoBehaviour
{
    public float moveThreshold = 0.1f;
    private bool isRegenerating = false;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private CharacterController controller;

    [SerializeField] private PlayerStat statSystem;

    private healthSystem health;

    private void Start()
    {
        health = GetComponent<healthSystem>();

        if (health != null)
        {
            health.OnDeath += HandleDeath;
        }

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    private void HandleDeath()
    {
        Debug.Log("Player has died. Game Over.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isRegenerating)
        {
            StartCoroutine(StartRegen());
        }
    }

    bool IsNotMoving()
    {
        float threshold = moveThreshold;

        bool rbStill = true;
        if (rb != null)
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rbStill = flatVel.magnitude < threshold;
        }

        bool ccStill = true;
        if (controller != null)
        {
            Vector3 flatVel = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
            ccStill = flatVel.magnitude < threshold;
        }

        return rbStill && ccStill;
    }


    IEnumerator StartRegen()
    {
        isRegenerating = true;
        Debug.Log("Regen starting in 3 seconds...");
        yield return new WaitForSeconds(3f);

        if (IsNotMoving())
        {
            Debug.Log("Health is regenerating...");
            health?.Heal(15);
            statSystem.RegisterHealthAction();
        }
        else
        {
            Debug.Log("Movement detected, regen cancelled.");
        }

        isRegenerating = false;
    }
}
