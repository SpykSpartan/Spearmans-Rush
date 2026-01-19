using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SquadController : MonoBehaviour
{
    public enum StrategyType { AggressivePush, FlankAndStrike, Surround, HoldPosition }

    public StrategyType currentStrategy;
    public List<EnemyAIBase> squadMembers = new List<EnemyAIBase>();
    public Transform player;

   private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        foreach (EnemyAIBase ai in squadMembers)
        {
            ai.player = player;
            ai.EnableTransitions(true);
        }

        StartCoroutine(ApplyInitialStrategy());
    }

    private IEnumerator ApplyInitialStrategy()
    {
        yield return null;
        ApplyStrategy(currentStrategy);
    }

    public void SetStrategy(StrategyType newStrategy)
    {
        if (newStrategy != currentStrategy)
        {
            ApplyStrategy(newStrategy);
            Debug.Log($"Squad applying strategy: {newStrategy}");
        }
    }

    public void ApplyStrategy(StrategyType strategy)
    {
        currentStrategy = strategy;

        switch (strategy)
        {
            case StrategyType.AggressivePush:
                foreach (var ai in squadMembers)
                    ai.SetFormationOffset(Vector3.zero);
                break;

            case StrategyType.FlankAndStrike:
                for (int i = 0; i < squadMembers.Count; i++)
                {
                    float side = i % 2 == 0 ? 1 : -1;
                    Vector3 offset = new Vector3(side * 3f, 0, 2f);
                    squadMembers[i].SetFormationOffset(offset);
                }
                break;

            case StrategyType.Surround:
                float angleStep = 360f / squadMembers.Count;
                for (int i = 0; i < squadMembers.Count; i++)
                {
                    float angle = i * angleStep * Mathf.Deg2Rad;
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 3f;
                    squadMembers[i].SetFormationOffset(offset);
                }
                break;

            case StrategyType.HoldPosition:
                foreach (var ai in squadMembers)
                    ai.SetHoldingPattern();
                break;
        }
    }
}
