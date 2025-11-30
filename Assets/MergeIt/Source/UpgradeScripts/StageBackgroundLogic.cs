using System;
using System.Collections.Generic;
using UnityEngine;

public class StageBackgroundLogic : MonoBehaviour
{
    [SerializeField] public Stage stageType;
    [SerializeField] public List<RectTransform> rewardsSpawnPositions;
    [SerializeField] private SpawnedReward _spawnedRewardsPrefab;

    private void Start()
    {
        // place all needed rewards instantly
    }

    public void AddUpgradeOnStage(UpgradeRewardDefinition reward)
    {
        throw new NotImplementedException();
    }
}
