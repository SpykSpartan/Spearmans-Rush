using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoneTrigger : MonoBehaviour
{
    public string zoneID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[ZoneTrigger] Player entered zone trigger: {zoneID}");
            zoneManager.Instance.PlayerEnteredZone(zoneID);
        }
    }
}
