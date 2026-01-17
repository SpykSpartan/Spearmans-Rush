using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public EnemyAIBase parentAI;
    private HashSet<GameObject> damagedTargets = new HashSet<GameObject>();

    public void EnableHitbox()
    {
        Debug.Log("AttackHitbox enabled");
        damagedTargets.Clear();
        gameObject.SetActive(true);
    }

    public void DisableHitbox()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !damagedTargets.Contains(other.gameObject))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null && parentAI != null)
            {
                damageable.TakeDamage(parentAI.attackDamage);
                damagedTargets.Add(other.gameObject);
                Debug.Log($"[{name}] Hit {other.name} for {parentAI.attackDamage} damage");
            }
        }
    }
}