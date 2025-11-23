using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sjur.Resources
{
    /// <summary>
    /// Manages player resources (Gold, Iron, Arcane)
    /// Handles generation, spending, and resource queries
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        [Header("Starting Resources")]
        [SerializeField] private float startingGold = 100f;
        [SerializeField] private float startingIron = 50f;
        [SerializeField] private float startingArcane = 0f;

        private Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();
        private Dictionary<ResourceType, float> generationRates = new Dictionary<ResourceType, float>();

        public event Action<ResourceType, float> OnResourceChanged;

        private void Awake()
        {
            InitializeResources();
        }

        private void InitializeResources()
        {
            resources[ResourceType.Gold] = startingGold;
            resources[ResourceType.Iron] = startingIron;
            resources[ResourceType.Arcane] = startingArcane;

            generationRates[ResourceType.Gold] = 0f;
            generationRates[ResourceType.Iron] = 0f;
            generationRates[ResourceType.Arcane] = 0f;
        }

        private void Update()
        {
            GenerateResources();
        }

        private void GenerateResources()
        {
            float deltaTime = Time.deltaTime;

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                if (generationRates[type] > 0)
                {
                    AddResource(type, generationRates[type] * deltaTime);
                }
            }
        }

        public void AddResource(ResourceType type, float amount)
        {
            resources[type] += amount;
            OnResourceChanged?.Invoke(type, resources[type]);
        }

        public bool SpendResource(ResourceType type, float amount)
        {
            if (resources[type] >= amount)
            {
                resources[type] -= amount;
                OnResourceChanged?.Invoke(type, resources[type]);
                return true;
            }
            return false;
        }

        public bool CanAfford(ResourceType type, float amount)
        {
            return resources[type] >= amount;
        }

        public bool CanAffordMultiple(Dictionary<ResourceType, float> costs)
        {
            foreach (var cost in costs)
            {
                if (!CanAfford(cost.Key, cost.Value))
                    return false;
            }
            return true;
        }

        public bool SpendMultiple(Dictionary<ResourceType, float> costs)
        {
            if (!CanAffordMultiple(costs))
                return false;

            foreach (var cost in costs)
            {
                SpendResource(cost.Key, cost.Value);
            }
            return true;
        }

        public float GetResource(ResourceType type)
        {
            return resources[type];
        }

        public void AddGenerationRate(ResourceType type, float rate)
        {
            generationRates[type] += rate;
        }

        public void RemoveGenerationRate(ResourceType type, float rate)
        {
            generationRates[type] = Mathf.Max(0, generationRates[type] - rate);
        }

        public float GetGenerationRate(ResourceType type)
        {
            return generationRates[type];
        }
    }
}
