using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class zoneManager : MonoBehaviour
{
    public static zoneManager Instance;

    [SerializeField] private Zone startingZone;

    private List<Zone> allZones = new List<Zone>();
    private Zone currentZone;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            RegisterAllZones();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterAllZones();

        if (startingZone != null)
            LoadZoneGroup(startingZone);
    }

    private void Start()
    {
        if (startingZone != null)
        {
            Debug.Log($"[ZoneManager] Starting in zone: {startingZone.zoneID}");
            LoadZoneGroup(startingZone);
        }
        else
        {
            Debug.LogWarning("[ZoneManager] No starting zone assigned.");
        }
    }

    private void RegisterAllZones()
    {
        Zone[] zones = FindObjectsOfType<Zone>(true);
        allZones = new List<Zone>(zones);

        foreach (Zone zone in allZones)
        {
            if (zone != startingZone)
                zone.SetActive(false);
        }
    }

    public void PlayerEnteredZone(string zoneID)
    {
        Zone enteredZone = allZones.Find(z => z.zoneID == zoneID);
        if (enteredZone != null)
        {
            Debug.Log($"[ZoneManager] Player entered zone: {zoneID}");
            LoadZoneGroup(enteredZone);
        }
        else
        {
            Debug.LogWarning($"[ZoneManager] Zone ID not found: {zoneID}");
        }
    }

    private void LoadZoneGroup(Zone triggerZone)
    {
        currentZone = triggerZone;

        foreach (Zone z in allZones)
        {
            if (z != startingZone)
                z.SetActive(false);
        }

        triggerZone.SetActive(true);

        foreach (Zone z in triggerZone.zonesToLoadTogether)
        {
            if (z != null)
                z.SetActive(true);
        }

        Debug.Log($"[ZoneManager] Loaded zone {triggerZone.zoneID} and {triggerZone.zonesToLoadTogether.Count} linked zones.");
    }
}
