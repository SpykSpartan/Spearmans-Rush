using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("Current multipliers")]
    public float speedMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float healthMultiplier = 1f;
    public float dashDistanceMultiplier = 1f;
    public float timeIncreaseMultiplier = 1f;

    [Header("Progression Settings")]
    public int threshold = 500;
    public float speedStat = 0.1f;
    public float damageStat = 0.5f;
    public float healthStat = 0.1f;
    public float dashDistanceStat = 0.5f;
    public float timeStat = 0.1f;

    private const int maxUpgrades = 10;

    [SerializeField] private int _speedXP, _damageXP, _healthXP, _dashXP, _timeXP;

    [SerializeField] private int _speedUpgrades, _damageUpgrades, _healthUpgrades, _dashUpgrades, _timeUpgrades;

    void AddXP(ref int xpPool, ref float multiplier, float step, ref int upgradeCount)
    {
        if (upgradeCount >= maxUpgrades) return;

        xpPool++;
        if (xpPool >= threshold)
        {
            xpPool -= threshold;
            upgradeCount++;

            if (upgradeCount <= maxUpgrades)
                multiplier += step;
        }
    }

    public void RegisterSpeedAction() => AddXP(ref _speedXP, ref speedMultiplier, speedStat, ref _speedUpgrades);
    public void RegisterDamageAction() => AddXP(ref _damageXP, ref damageMultiplier, damageStat, ref _damageUpgrades);
    public void RegisterHealthAction() 
    {
        AddXP(ref _healthXP, ref healthMultiplier, healthStat, ref _healthUpgrades);

        healthSystem health = GetComponent<healthSystem>();
        if (health != null)
        {
            health.UpdateMaxHealth();
        }
    }
    public void RegisterDashAction() => AddXP(ref _dashXP, ref dashDistanceMultiplier, dashDistanceStat, ref _dashUpgrades);
    public void RegisterTimeIncreaseAction() => AddXP(ref _timeXP, ref timeIncreaseMultiplier, timeStat, ref _timeUpgrades);

    public int GetSpeedXP() => _speedXP;
    public int GetDamageXP() => _damageXP;
    public int GetHealthXP() => _healthXP;
    public int GetDashXP() => _dashXP;
    public int GetTimeXP() => _timeXP;

    public int GetSpeedUpgrades() => _speedUpgrades;
    public int GetDamageUpgrades() => _damageUpgrades;
    public int GetHealthUpgrades() => _healthUpgrades;
    public int GetDashUpgrades() => _dashUpgrades;
    public int GetTimeUpgrades() => _timeUpgrades;
   #region Save and Load

    public void Save(ref PlayerSaveStats data)
    {
        data.speedMod = _speedXP;
        data.speedModLevel = _speedUpgrades;
    }

    public void Load(PlayerSaveStats data)
    {
        _speedXP = data.speedMod;
        _speedUpgrades = data.speedModLevel;
    }

    #endregion
}

[System.Serializable]

public struct PlayerSaveStats
{
    public int speedMod;
    public int speedModLevel;
}