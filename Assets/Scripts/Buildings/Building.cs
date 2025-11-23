using System;
using System.Collections.Generic;
using UnityEngine;
using Sjur.Resources;

namespace Sjur.Buildings
{
    /// <summary>
    /// Core building component that handles upgrades, resource generation, and health
    /// </summary>
    public class Building : MonoBehaviour
    {
        [SerializeField] private BuildingData data;
        [SerializeField] private ResourceManager resourceManager;

        private int currentLevel = 1;
        private float currentHealth;
        private bool isDestroyed = false;
        private float respawnTimer = 0f;

        public BuildingData Data => data;
        public int CurrentLevel => currentLevel;
        public float CurrentHealth => currentHealth;
        public bool IsDestroyed => isDestroyed;

        public event Action<Building> OnBuildingDestroyed;
        public event Action<Building> OnBuildingUpgraded;
        public event Action<Building> OnBuildingRespawned;

        private void Awake()
        {
            if (data != null)
            {
                currentHealth = data.maxHealth;
            }
        }

        private void Start()
        {
            if (resourceManager != null && data != null && data.baseGenerationRate > 0)
            {
                ApplyGenerationRate();
            }
        }

        private void Update()
        {
            if (isDestroyed && data.canRespawn)
            {
                respawnTimer += Time.deltaTime;
                if (respawnTimer >= data.respawnTime)
                {
                    Respawn();
                }
            }
        }

        public bool TryUpgrade()
        {
            if (currentLevel >= data.maxLevel)
            {
                Debug.Log($"{data.buildingName} is already at max level");
                return false;
            }

            if (resourceManager == null)
            {
                Debug.LogError("ResourceManager not assigned!");
                return false;
            }

            UpgradeLevel nextLevel = data.upgradeLevels.Find(u => u.level == currentLevel + 1);
            if (nextLevel == null)
            {
                Debug.LogError($"No upgrade data for level {currentLevel + 1}");
                return false;
            }

            Dictionary<ResourceType, float> costs = new Dictionary<ResourceType, float>();
            foreach (var cost in nextLevel.upgradeCosts)
            {
                costs[cost.resourceType] = cost.amount;
            }

            if (resourceManager.SpendMultiple(costs))
            {
                // Remove old generation rate
                if (data.baseGenerationRate > 0)
                {
                    float oldRate = CalculateGenerationRate(currentLevel);
                    resourceManager.RemoveGenerationRate(data.generatedResource, oldRate);
                }

                currentLevel++;

                // Add new generation rate
                if (data.baseGenerationRate > 0)
                {
                    ApplyGenerationRate();
                }

                OnBuildingUpgraded?.Invoke(this);
                Debug.Log($"{data.buildingName} upgraded to level {currentLevel}");
                return true;
            }

            return false;
        }

        private void ApplyGenerationRate()
        {
            float rate = CalculateGenerationRate(currentLevel);
            resourceManager.AddGenerationRate(data.generatedResource, rate);
        }

        private float CalculateGenerationRate(int level)
        {
            float rate = data.baseGenerationRate;

            for (int i = 2; i <= level; i++)
            {
                UpgradeLevel upgradeLevel = data.upgradeLevels.Find(u => u.level == i);
                if (upgradeLevel != null)
                {
                    rate *= upgradeLevel.generationRateMultiplier;
                }
            }

            return rate;
        }

        public void TakeDamage(float damage)
        {
            if (isDestroyed) return;

            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                DestroyBuilding();
            }
        }

        private void DestroyBuilding()
        {
            isDestroyed = true;
            currentHealth = 0;

            // Remove generation rate
            if (data.baseGenerationRate > 0 && resourceManager != null)
            {
                float rate = CalculateGenerationRate(currentLevel);
                resourceManager.RemoveGenerationRate(data.generatedResource, rate);
            }

            OnBuildingDestroyed?.Invoke(this);

            if (data.canRespawn)
            {
                respawnTimer = 0f;
                gameObject.SetActive(false);
            }
            else
            {
                // Permanent destruction - disable visual
                gameObject.SetActive(false);
            }
        }

        private void Respawn()
        {
            isDestroyed = false;
            currentHealth = data.maxHealth;
            respawnTimer = 0f;
            gameObject.SetActive(true);

            // Restore generation rate
            if (data.baseGenerationRate > 0 && resourceManager != null)
            {
                ApplyGenerationRate();
            }

            OnBuildingRespawned?.Invoke(this);
        }

        public void SetResourceManager(ResourceManager manager)
        {
            resourceManager = manager;
        }
    }
}
