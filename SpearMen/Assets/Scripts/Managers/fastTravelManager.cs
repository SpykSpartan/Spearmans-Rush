using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTravelManager : MonoBehaviour
{
    [System.Serializable]
    public class FastTravelPoint
    {
        public string name;
        public Transform location;
        public bool isActive;
    }

    public List<FastTravelPoint> travelPoints = new List<FastTravelPoint>();
    public Transform player;

    

    private void Update()
    {
        HandleFastTravelInput();
    }

    private void HandleFastTravelInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) FastTravelToIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) FastTravelToIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) FastTravelToIndex(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) FastTravelToIndex(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) FastTravelToIndex(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) FastTravelToIndex(5);
    }

    public void FastTravelToIndex(int index)
    {
        if (index < 0 || index >= travelPoints.Count)
        {
            Debug.LogWarning($"No fast travel point assigned to index {index}");
            return;
        }

        var point = travelPoints[index];
        if (!point.isActive)
        {
            Debug.LogWarning($"Fast travel point '{point.name}' is inactive.");
            return;
        }

        TeleportPlayer(point.location.position);
        Debug.Log($"Teleported to: {point.name}");
    }

    public void FastTravelTo(string pointName)
    {
        var point = travelPoints.Find(p => p.name == pointName);
        if (point != null && point.isActive)
        {
            TeleportPlayer(point.location.position);
            Debug.Log($"Teleported to: {point.name}");
        }
        else
        {
            Debug.LogWarning($"Cannot fast travel to '{pointName}' — does not exist or is inactive.");
        }
    }

    private void TeleportPlayer(Vector3 targetPosition)
{
    // Try disabling CharacterController if it exists
    CharacterController cc = player.GetComponent<CharacterController>();
    if (cc != null)
    {
        cc.enabled = false;
        player.position = targetPosition;
        cc.enabled = true;
        return;
    }

    // Try Rigidbody (non-kinematic)
    Rigidbody rb = player.GetComponent<Rigidbody>();
    if (rb != null && rb.isKinematic == false)
    {
        rb.velocity = Vector3.zero;
        rb.MovePosition(targetPosition);
        return;
    }

    // Fallback to just setting position
    player.position = targetPosition;
}

    public void ActivatePoint(string pointName)
    {
        var point = travelPoints.Find(p => p.name == pointName);
        if (point != null)
        {
            point.isActive = true;
            Debug.Log($"Activated fast travel point: {point.name}");
        }
    }

    public void DeactivatePoint(string pointName)
    {
        var point = travelPoints.Find(p => p.name == pointName);
        if (point != null)
        {
            point.isActive = false;
            Debug.Log($"Deactivated fast travel point: {point.name}");
        }
    }
}