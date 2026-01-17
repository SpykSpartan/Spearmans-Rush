using System.Collections.Generic;
using UnityEngine;

public class WallUnlocker : MonoBehaviour
{
    [Header("Assign the objects that need to be destroyed first")]
    public List<GameObject> targets = new List<GameObject>();

    [Header("The wall or door to destroy/unlock")]
    public GameObject wallToDestroy;

    [Header("Optional check delay (seconds)")]
    public float checkInterval = 0.5f;

    private void Start()
    {
        InvokeRepeating(nameof(CheckTargets), checkInterval, checkInterval);
    }

    private void CheckTargets()
    {
        targets.RemoveAll(target => target == null);

        if (targets.Count == 0)
        {
            UnlockWall();
        }
    }

    private void UnlockWall()
    {
        if (wallToDestroy != null)
        {
            Destroy(wallToDestroy);
            Debug.Log("All targets destroyed! Wall removed.");
        }
        else
        {
            Debug.LogWarning("No wall assigned to destroy.");
        }

        CancelInvoke(nameof(CheckTargets));
    }
}
