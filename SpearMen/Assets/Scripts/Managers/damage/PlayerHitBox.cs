using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public int damage = 15;
    public float modDamage;
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    private bool isActive = false;

    [SerializeField] private PlayerStat statSystem;

    [SerializeField] private Transform pivotTransform;

    public void ActivateHitbox()
    {
        damagedEnemies.Clear();
        isActive = true;

        if (Camera.main != null && pivotTransform != null)
        {
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            if (camForward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(camForward);
                pivotTransform.rotation = targetRotation;
            }
        }

        gameObject.SetActive(true);
    }

    public void DeactivateHitbox()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (!other.CompareTag("Enemy")) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !damagedEnemies.Contains(other.gameObject))
        {
            modDamage = Mathf.Round(damage + statSystem.damageMultiplier);
            int finalDamage = (int)modDamage;
            damageable.TakeDamage(finalDamage);
            damagedEnemies.Add(other.gameObject);
            statSystem.RegisterDamageAction();
            Debug.Log($"Player hit {other.name} for {finalDamage} damage");
        }
    }
}
