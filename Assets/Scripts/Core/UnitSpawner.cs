using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sjur.Units;
using Sjur.Resources;
using Sjur.Lanes;
using Sjur.Themes;

namespace Sjur.Core
{
    /// <summary>
    /// Handles unit spawning with cost validation, cooldowns, and queue management
    /// </summary>
    public class UnitSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ResourceManager resourceManager;
        [SerializeField] private LaneManager laneManager;
        [SerializeField] private ThemeManager themeManager;
        [SerializeField] private int teamId;

        [Header("Spawn Settings")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private int maxQueueSize = 10;

        [Header("Unit Prefabs")]
        [SerializeField] private GameObject unitPrefab; // Basic unit prefab with Unit component

        private Queue<SpawnRequest> spawnQueue = new Queue<SpawnRequest>();
        private Dictionary<UnitType, float> spawnCooldowns = new Dictionary<UnitType, float>();
        private bool isSpawning = false;

        public int TeamId => teamId;
        public int QueueSize => spawnQueue.Count;

        public event Action<Unit> OnUnitSpawned;
        public event Action<UnitType> OnSpawnFailed;

        private void Awake()
        {
            InitializeCooldowns();
        }

        private void InitializeCooldowns()
        {
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                spawnCooldowns[type] = 0f;
            }
        }

        private void Update()
        {
            UpdateCooldowns();
            ProcessSpawnQueue();
        }

        private void UpdateCooldowns()
        {
            List<UnitType> types = new List<UnitType>(spawnCooldowns.Keys);
            foreach (UnitType type in types)
            {
                if (spawnCooldowns[type] > 0)
                {
                    spawnCooldowns[type] -= Time.deltaTime;
                }
            }
        }

        private void ProcessSpawnQueue()
        {
            if (isSpawning || spawnQueue.Count == 0)
                return;

            SpawnRequest request = spawnQueue.Peek();

            // Check if cooldown is ready
            if (spawnCooldowns[request.unitType] <= 0)
            {
                spawnQueue.Dequeue();
                StartCoroutine(SpawnUnitCoroutine(request));
            }
        }

        /// <summary>
        /// Request to spawn a unit
        /// </summary>
        public bool RequestSpawn(UnitType unitType, int? forceLaneId = null)
        {
            UnitData unitData = themeManager.GetUnitData(unitType);

            if (unitData == null)
            {
                Debug.LogError($"No unit data found for {unitType}");
                return false;
            }

            // Check if queue is full
            if (spawnQueue.Count >= maxQueueSize)
            {
                Debug.Log("Spawn queue is full!");
                return false;
            }

            // Check if can afford
            Dictionary<ResourceType, float> costs = unitData.GetSpawnCostDictionary();
            if (!resourceManager.CanAffordMultiple(costs))
            {
                Debug.Log($"Cannot afford {unitType}");
                OnSpawnFailed?.Invoke(unitType);
                return false;
            }

            // Spend resources
            if (!resourceManager.SpendMultiple(costs))
            {
                return false;
            }

            // Add to queue
            SpawnRequest request = new SpawnRequest
            {
                unitType = unitType,
                unitData = unitData,
                forceLaneId = forceLaneId
            };

            spawnQueue.Enqueue(request);
            Debug.Log($"Unit {unitType} added to spawn queue");

            return true;
        }

        private IEnumerator SpawnUnitCoroutine(SpawnRequest request)
        {
            isSpawning = true;

            // Assign lane
            Lane assignedLane = request.forceLaneId.HasValue
                ? laneManager.AssignUnitToLane(request.forceLaneId.Value)
                : laneManager.AssignUnitToLane();

            if (assignedLane == null)
            {
                Debug.LogError("Failed to assign lane!");
                isSpawning = false;
                yield break;
            }

            // Get spawn position
            Vector3 spawnPosition = spawnPoint != null
                ? spawnPoint.position
                : laneManager.GetSpawnPosition(teamId, assignedLane.LaneId);

            // Instantiate unit
            GameObject unitObj = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
            Unit unit = unitObj.GetComponent<Unit>();

            if (unit == null)
            {
                Debug.LogError("Unit prefab missing Unit component!");
                Destroy(unitObj);
                isSpawning = false;
                yield break;
            }

            // Initialize unit
            unit.Initialize(request.unitData, teamId);

            // Setup pathfinding
            UnitPathfinding pathfinding = unitObj.GetComponent<UnitPathfinding>();
            if (pathfinding == null)
            {
                pathfinding = unitObj.AddComponent<UnitPathfinding>();
            }
            pathfinding.Initialize(assignedLane, teamId);

            // Apply visual customization
            ApplyUnitVisuals(unitObj, request.unitData);

            // Set cooldown
            spawnCooldowns[request.unitType] = request.unitData.spawnCooldown;

            // Trigger event
            OnUnitSpawned?.Invoke(unit);

            Debug.Log($"Spawned {request.unitData.unitName} on lane {assignedLane.LaneId}");

            isSpawning = false;
        }

        private void ApplyUnitVisuals(GameObject unitObj, UnitData unitData)
        {
            // Apply color to unit
            Renderer renderer = unitObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = unitData.unitColor;
            }

            // Apply scale
            unitObj.transform.localScale = Vector3.one * unitData.modelScale;

            // If unit has a custom prefab, replace the default
            // (This would be more complex in a real implementation)
        }

        /// <summary>
        /// Check if unit type is on cooldown
        /// </summary>
        public bool IsOnCooldown(UnitType unitType)
        {
            return spawnCooldowns.ContainsKey(unitType) && spawnCooldowns[unitType] > 0;
        }

        /// <summary>
        /// Get remaining cooldown for unit type
        /// </summary>
        public float GetCooldown(UnitType unitType)
        {
            return spawnCooldowns.ContainsKey(unitType) ? spawnCooldowns[unitType] : 0f;
        }

        /// <summary>
        /// Clear spawn queue
        /// </summary>
        public void ClearQueue()
        {
            spawnQueue.Clear();
        }

        public void SetTeam(int team)
        {
            teamId = team;
        }
    }

    /// <summary>
    /// Internal class for spawn queue management
    /// </summary>
    internal class SpawnRequest
    {
        public UnitType unitType;
        public UnitData unitData;
        public int? forceLaneId;
    }
}
