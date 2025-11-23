using System.Collections.Generic;
using UnityEngine;
using Sjur.Resources;
using Sjur.Themes;

namespace Sjur.Units
{
    /// <summary>
    /// ScriptableObject defining unit properties and stats
    /// </summary>
    [CreateAssetMenu(fileName = "NewUnit", menuName = "Sjur/Unit Data")]
    public class UnitData : ScriptableObject
    {
        [Header("Basic Info")]
        public UnitType unitType;
        public string unitName;
        [TextArea(2, 4)]
        public string description;

        [Header("Spawn Costs")]
        public List<ResourceCost> spawnCosts = new List<ResourceCost>();
        public float spawnCooldown = 2f;

        [Header("Combat Stats")]
        public float maxHealth = 100f;
        public float damage = 10f;
        public float attackRange = 1.5f;
        public float attackSpeed = 1f; // Attacks per second
        public DamageType damageType = DamageType.Physical;

        [Header("Defense Stats")]
        public float armor = 0f;
        public float magicResistance = 0f;

        [Header("Movement")]
        public float moveSpeed = 3f;

        [Header("Visual (Optional)")]
        public GameObject prefab;
        public Color unitColor = Color.white;
        public float modelScale = 1f;

        /// <summary>
        /// Calculate effective damage against target considering armor
        /// </summary>
        public float CalculateDamage(UnitData target)
        {
            float finalDamage = damage;

            // Apply armor reduction based on damage type
            if (damageType == DamageType.Physical)
            {
                float reduction = target.armor / (target.armor + 100f);
                finalDamage *= (1f - reduction);
            }
            else if (damageType == DamageType.Magic)
            {
                float reduction = target.magicResistance / (target.magicResistance + 100f);
                finalDamage *= (1f - reduction);
            }

            return finalDamage;
        }

        /// <summary>
        /// Get total spawn cost in resources
        /// </summary>
        public Dictionary<ResourceType, float> GetSpawnCostDictionary()
        {
            var costs = new Dictionary<ResourceType, float>();
            foreach (var cost in spawnCosts)
            {
                costs[cost.resourceType] = cost.amount;
            }
            return costs;
        }
    }

    /// <summary>
    /// Resource cost structure used by buildings as well
    /// </summary>
    [System.Serializable]
    public class ResourceCost
    {
        public ResourceType resourceType;
        public float amount;
    }

    /// <summary>
    /// Damage types for combat calculations
    /// </summary>
    public enum DamageType
    {
        Physical,   // Reduced by armor (melee, mounted)
        Magic,      // Reduced by magic resistance (arcane units)
        Pierce,     // Ignores some armor (ranged)
        True        // Ignores all defenses
    }
}
