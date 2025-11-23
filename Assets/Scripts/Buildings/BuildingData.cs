using System.Collections.Generic;
using UnityEngine;
using Sjur.Resources;

namespace Sjur.Buildings
{
    /// <summary>
    /// ScriptableObject defining building properties and upgrade paths
    /// </summary>
    [CreateAssetMenu(fileName = "NewBuilding", menuName = "Sjur/Building Data")]
    public class BuildingData : ScriptableObject
    {
        [Header("Basic Info")]
        public BuildingType buildingType;
        public string buildingName;
        [TextArea(2, 4)]
        public string description;

        [Header("Construction Costs")]
        public List<ResourceCost> constructionCosts = new List<ResourceCost>();

        [Header("Resource Generation")]
        public ResourceType generatedResource;
        public float baseGenerationRate = 1f;

        [Header("Upgrade System")]
        public int maxLevel = 3;
        public List<UpgradeLevel> upgradeLevels = new List<UpgradeLevel>();

        [Header("Combat Stats (for Base/Objective)")]
        public float maxHealth = 100f;
        public bool canRespawn = false;
        public float respawnTime = 60f;
    }

    [System.Serializable]
    public class ResourceCost
    {
        public ResourceType resourceType;
        public float amount;
    }

    [System.Serializable]
    public class UpgradeLevel
    {
        public int level;
        public List<ResourceCost> upgradeCosts;
        public float generationRateMultiplier = 1.5f; // Each level increases by 50%
    }
}
