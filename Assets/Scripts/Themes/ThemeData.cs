using System.Collections.Generic;
using UnityEngine;
using Sjur.Units;
using Sjur.Buildings;

namespace Sjur.Themes
{
    /// <summary>
    /// ScriptableObject defining a complete theme/faction with all units, buildings, and bonuses
    /// This enables genre-agnostic gameplay (Fantasy, Sci-Fi, Historical, etc.)
    /// </summary>
    [CreateAssetMenu(fileName = "NewTheme", menuName = "Sjur/Theme Data")]
    public class ThemeData : ScriptableObject
    {
        [Header("Theme Identity")]
        public string themeName;
        public string genre; // Fantasy, Sci-Fi, Historical, etc.
        [TextArea(3, 6)]
        public string description;
        public Sprite themeIcon;

        [Header("Visual Theme")]
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.gray;
        public Material unitMaterial;
        public Material buildingMaterial;

        [Header("Buildings")]
        public BuildingData loggingCamp;
        public BuildingData mine;
        public BuildingData mageTower;
        public BuildingData baseStructure;
        public BuildingData centerObjective;

        [Header("Units")]
        public UnitData meleeUnit;
        public UnitData rangedUnit;
        public UnitData armouredUnit;
        public UnitData mountedUnit;

        [Header("Center Objective Bonus")]
        public CenterBonusType bonusType;
        public float bonusAmount = 10f;
        [TextArea(2, 4)]
        public string bonusDescription;

        [Header("Audio (Optional)")]
        public AudioClip themeMusic;
        public AudioClip unitSpawnSound;
        public AudioClip buildingConstructSound;

        /// <summary>
        /// Get all available units for this theme
        /// </summary>
        public List<UnitData> GetAllUnits()
        {
            var units = new List<UnitData>();
            if (meleeUnit != null) units.Add(meleeUnit);
            if (rangedUnit != null) units.Add(rangedUnit);
            if (armouredUnit != null) units.Add(armouredUnit);
            if (mountedUnit != null) units.Add(mountedUnit);
            return units;
        }

        /// <summary>
        /// Get all buildings for this theme
        /// </summary>
        public List<BuildingData> GetAllBuildings()
        {
            var buildings = new List<BuildingData>();
            if (loggingCamp != null) buildings.Add(loggingCamp);
            if (mine != null) buildings.Add(mine);
            if (mageTower != null) buildings.Add(mageTower);
            if (baseStructure != null) buildings.Add(baseStructure);
            if (centerObjective != null) buildings.Add(centerObjective);
            return buildings;
        }
    }

    /// <summary>
    /// Types of bonuses awarded for capturing the center objective
    /// </summary>
    public enum CenterBonusType
    {
        None,
        IncreaseArmor,          // Dwarven Workshop example
        IncreaseRangedDamage,   // Elven Fletcher example
        IncreaseMeleeDamage,
        IncreaseMovementSpeed,
        IncreaseAttackSpeed,
        ReduceUnitCost,
        IncreaseResourceGeneration,
        UnlockSpecialUnit
    }
}
