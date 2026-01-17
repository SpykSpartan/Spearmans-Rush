using System.Collections.Generic;
using UnityEngine;

public class PersistMap : MonoBehaviour
{
    public static PersistMap Instance;

    [Header("Zone Settings")]
    public Zone baseZone;

    private List<Zone> allZones = new List<Zone>();
    private Zone currentZone;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Zone[] zones = FindObjectsOfType<Zone>(true);
        allZones = new List<Zone>(zones);

        foreach (var z in allZones)
            z.SetActive(false);

        if (baseZone != null)
        {
            baseZone.SetActive(true);
            currentZone = baseZone;
            Debug.Log($"[PersistMap] Base zone activated: {baseZone.zoneID}");
        }
        else
        {
            if (allZones.Count > 0)
            {
                allZones[0].SetActive(true);
                currentZone = allZones[0];
                Debug.Log($"[PersistMap] No base zone assigned, activated first zone: {currentZone.zoneID}");
            }
        }
    }

    private void RegisterAllZones()
    {
        Zone[] zones = FindObjectsOfType<Zone>(true);
        allZones = new List<Zone>(zones);

        foreach (Zone z in allZones)
        {
            if (currentZone == null || z != currentZone)
                z.SetActive(false);
        }
    }

    public void PlayerEnteredZone(string zoneID)
    {
        Zone enteredZone = allZones.Find(z => z.zoneID == zoneID);
        if (enteredZone != null)
        {
            LoadZoneGroup(enteredZone);
        }
        else
        {
            Debug.LogWarning($"[PersistMap] Zone ID not found: {zoneID}");
        }
    }

    public void LoadZoneGroup(Zone triggerZone)
    {
        currentZone = triggerZone;

        foreach (Zone z in allZones)
            z.SetActive(false);

        triggerZone.SetActive(true);

        foreach (Zone z in triggerZone.zonesToLoadTogether)
        {
            if (z != null)
                z.SetActive(true);
        }

        Debug.Log($"[PersistMap] Loaded zone {triggerZone.zoneID} and {triggerZone.zonesToLoadTogether.Count} linked zones.");
    }

    public Zone GetCurrentZone()
    {
        return currentZone;
    }
}
