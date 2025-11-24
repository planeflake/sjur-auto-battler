using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Sjur.Units;
using Sjur.Themes;
using Sjur.Resources;
using BuildingData = Sjur.Buildings.BuildingData;
using BuildingType = Sjur.Buildings.BuildingType;
using UpgradeLevel = Sjur.Buildings.UpgradeLevel;

namespace Sjur.Core.Editor
{
    /// <summary>
    /// Editor utility to generate default game data assets
    /// Creates a complete Fantasy theme with all units and buildings
    /// </summary>
    public static class DefaultDataGenerator
    {
        [MenuItem("Sjur/Generate Default Fantasy Theme")]
        public static void GenerateDefaultTheme()
        {
            // Ensure directories exist
            EnsureDirectoryExists("Assets/Data");
            EnsureDirectoryExists("Assets/Data/Themes");
            EnsureDirectoryExists("Assets/Data/Units");
            EnsureDirectoryExists("Assets/Data/Buildings");

            // Generate all data
            var units = GenerateUnitData();
            var buildings = GenerateBuildingData();
            var theme = GenerateThemeData(units, buildings);

            Debug.Log("✅ Default Fantasy Theme generated successfully!");
            Debug.Log($"Theme: {AssetDatabase.GetAssetPath(theme)}");

            // Select the theme in the project window
            Selection.activeObject = theme;
            EditorGUIUtility.PingObject(theme);
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parentFolder = Path.GetDirectoryName(path).Replace('\\', '/');
                string newFolder = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parentFolder, newFolder);
            }
        }

        private static UnitData[] GenerateUnitData()
        {
            UnitData[] units = new UnitData[4];

            // Melee Unit - Knight
            units[0] = ScriptableObject.CreateInstance<UnitData>();
            units[0].unitType = UnitType.Melee;
            units[0].unitName = "Knight";
            units[0].description = "Heavy melee warrior with high health and armor";
            units[0].spawnCosts.Add(new Units.ResourceCost { resourceType = ResourceType.Gold, amount = 50 });
            units[0].maxHealth = 150;
            units[0].damage = 15;
            units[0].attackRange = 1.5f;
            units[0].attackSpeed = 1f;
            units[0].damageType = DamageType.Physical;
            units[0].armor = 10;
            units[0].magicResistance = 0;
            units[0].moveSpeed = 3f;
            units[0].spawnCooldown = 3f;
            units[0].unitColor = new Color(0.8f, 0.8f, 0.9f); // Light blue-gray
            CreateAsset(units[0], "Assets/Data/Units/Knight_Data.asset");

            // Ranged Unit - Archer
            units[1] = ScriptableObject.CreateInstance<UnitData>();
            units[1].unitType = UnitType.Ranged;
            units[1].unitName = "Archer";
            units[1].description = "Long-range attacker with pierce damage";
            units[1].spawnCosts.Add(new Units.ResourceCost { resourceType = ResourceType.Gold, amount = 40 });
            units[1].spawnCosts.Add(new Units.ResourceCost { resourceType = ResourceType.Iron, amount = 10 });
            units[1].maxHealth = 80;
            units[1].damage = 12;
            units[1].attackRange = 8f;
            units[1].attackSpeed = 1.5f;
            units[1].damageType = DamageType.Pierce;
            units[1].armor = 0;
            units[1].magicResistance = 0;
            units[1].moveSpeed = 3f;
            units[1].spawnCooldown = 2.5f;
            units[1].unitColor = new Color(0.4f, 0.7f, 0.4f); // Green
            CreateAsset(units[1], "Assets/Data/Units/Archer_Data.asset");

            // Armoured Unit - Defender
            units[2] = ScriptableObject.CreateInstance<UnitData>();
            units[2].unitType = UnitType.Armoured;
            units[2].unitName = "Defender";
            units[2].description = "Heavily armored tank with high defense";
            units[2].spawnCosts.Add(new Units.ResourceCost { resourceType = ResourceType.Gold, amount = 30 });
            units[2].spawnCosts.Add(new Units.ResourceCost { resourceType = ResourceType.Iron, amount = 40 });
            units[2].maxHealth = 250;
            units[2].damage = 8;
            units[2].attackRange = 1.5f;
            units[2].attackSpeed = 0.8f;
            units[2].damageType = DamageType.Physical;
            units[2].armor = 25;
            units[2].magicResistance = 5;
            units[2].moveSpeed = 2f;
            units[2].spawnCooldown = 4f;
            units[2].unitColor = new Color(0.6f, 0.6f, 0.6f); // Gray
            CreateAsset(units[2], "Assets/Data/Units/Defender_Data.asset");

            // Mounted Unit - Cavalry
            units[3] = ScriptableObject.CreateInstance<UnitData>();
            units[3].unitType = UnitType.Mounted;
            units[3].unitName = "Cavalry";
            units[3].description = "Fast mounted unit with high damage and speed";
            units[3].spawnCosts.Add(new Units.ResourceCost { resourceType = ResourceType.Gold, amount = 60 });
            units[3].spawnCosts.Add(new Units.ResourceCost { resourceType = ResourceType.Iron, amount = 20 });
            units[3].maxHealth = 120;
            units[3].damage = 20;
            units[3].attackRange = 2f;
            units[3].attackSpeed = 1.2f;
            units[3].damageType = DamageType.Physical;
            units[3].armor = 5;
            units[3].magicResistance = 0;
            units[3].moveSpeed = 6f;
            units[3].spawnCooldown = 5f;
            units[3].unitColor = new Color(0.8f, 0.6f, 0.3f); // Brown
            CreateAsset(units[3], "Assets/Data/Units/Cavalry_Data.asset");

            return units;
        }

        private static BuildingData[] GenerateBuildingData()
        {
            BuildingData[] buildings = new BuildingData[5];

            // Logging Camp
            buildings[0] = ScriptableObject.CreateInstance<BuildingData>();
            buildings[0].buildingType = BuildingType.LoggingCamp;
            buildings[0].buildingName = "Logging Camp";
            buildings[0].description = "Generates Gold over time. Can be upgraded for increased production.";
            buildings[0].generatedResource = ResourceType.Gold;
            buildings[0].baseGenerationRate = 2f;
            buildings[0].maxLevel = 3;
            buildings[0].upgradeLevels.Add(new UpgradeLevel
            {
                level = 2,
                upgradeCosts = new List<Buildings.ResourceCost>
                {
                    new Buildings.ResourceCost { resourceType = ResourceType.Gold, amount = 100 }
                },
                generationRateMultiplier = 1.5f
            });
            buildings[0].upgradeLevels.Add(new UpgradeLevel
            {
                level = 3,
                upgradeCosts = new List<Buildings.ResourceCost>
                {
                    new Buildings.ResourceCost { resourceType = ResourceType.Gold, amount = 200 }
                },
                generationRateMultiplier = 2f
            });
            CreateAsset(buildings[0], "Assets/Data/Buildings/LoggingCamp_Data.asset");

            // Mine
            buildings[1] = ScriptableObject.CreateInstance<BuildingData>();
            buildings[1].buildingType = BuildingType.Mine;
            buildings[1].buildingName = "Mine";
            buildings[1].description = "Generates Iron over time. Can be upgraded for increased production.";
            buildings[1].constructionCosts.Add(new Buildings.ResourceCost { resourceType = ResourceType.Gold, amount = 100 });
            buildings[1].generatedResource = ResourceType.Iron;
            buildings[1].baseGenerationRate = 1.5f;
            buildings[1].maxLevel = 3;
            buildings[1].upgradeLevels.Add(new UpgradeLevel
            {
                level = 2,
                upgradeCosts = new List<Buildings.ResourceCost>
                {
                    new Buildings.ResourceCost { resourceType = ResourceType.Gold, amount = 150 }
                },
                generationRateMultiplier = 1.5f
            });
            buildings[1].upgradeLevels.Add(new UpgradeLevel
            {
                level = 3,
                upgradeCosts = new List<Buildings.ResourceCost>
                {
                    new Buildings.ResourceCost { resourceType = ResourceType.Gold, amount = 250 },
                    new Buildings.ResourceCost { resourceType = ResourceType.Iron, amount = 50 }
                },
                generationRateMultiplier = 2f
            });
            CreateAsset(buildings[1], "Assets/Data/Buildings/Mine_Data.asset");

            // Mage Tower
            buildings[2] = ScriptableObject.CreateInstance<BuildingData>();
            buildings[2].buildingType = BuildingType.MageTower;
            buildings[2].buildingName = "Mage Tower";
            buildings[2].description = "Generates Arcane energy. Requires Gold and Iron to construct.";
            buildings[2].constructionCosts.Add(new Buildings.ResourceCost { resourceType = ResourceType.Gold, amount = 200 });
            buildings[2].constructionCosts.Add(new Buildings.ResourceCost { resourceType = ResourceType.Iron, amount = 100 });
            buildings[2].generatedResource = ResourceType.Arcane;
            buildings[2].baseGenerationRate = 1f;
            buildings[2].maxLevel = 3;
            buildings[2].upgradeLevels.Add(new UpgradeLevel
            {
                level = 2,
                upgradeCosts = new List<Buildings.ResourceCost>
                {
                    new Buildings.ResourceCost { resourceType = ResourceType.Gold, amount = 300 },
                    new Buildings.ResourceCost { resourceType = ResourceType.Iron, amount = 100 }
                },
                generationRateMultiplier = 1.5f
            });
            buildings[2].upgradeLevels.Add(new UpgradeLevel
            {
                level = 3,
                upgradeCosts = new List<Buildings.ResourceCost>
                {
                    new Buildings.ResourceCost { resourceType = ResourceType.Gold, amount = 500 },
                    new Buildings.ResourceCost { resourceType = ResourceType.Iron, amount = 200 }
                },
                generationRateMultiplier = 2f
            });
            CreateAsset(buildings[2], "Assets/Data/Buildings/MageTower_Data.asset");

            // Base
            buildings[3] = ScriptableObject.CreateInstance<BuildingData>();
            buildings[3].buildingType = BuildingType.Base;
            buildings[3].buildingName = "Base";
            buildings[3].description = "Your main base. Destroying the enemy base wins the game!";
            buildings[3].maxHealth = 5000;
            buildings[3].canRespawn = false;
            CreateAsset(buildings[3], "Assets/Data/Buildings/Base_Data.asset");

            // Center Objective
            buildings[4] = ScriptableObject.CreateInstance<BuildingData>();
            buildings[4].buildingType = BuildingType.CenterObjective;
            buildings[4].buildingName = "Ancient Monument";
            buildings[4].description = "Capture this monument to gain powerful bonuses for your team!";
            buildings[4].maxHealth = 1000;
            buildings[4].canRespawn = false;
            buildings[4].respawnTime = 60f;
            CreateAsset(buildings[4], "Assets/Data/Buildings/CenterObjective_Data.asset");

            return buildings;
        }

        private static ThemeData GenerateThemeData(UnitData[] units, BuildingData[] buildings)
        {
            ThemeData theme = ScriptableObject.CreateInstance<ThemeData>();

            theme.themeName = "Fantasy";
            theme.genre = "Fantasy";
            theme.description = "Classic medieval fantasy theme with knights, archers, and magic. " +
                              "Build your economy with logging camps and mines, then train powerful units to dominate the battlefield!";

            // Colors
            theme.primaryColor = new Color(0.2f, 0.4f, 0.8f); // Blue
            theme.secondaryColor = new Color(0.9f, 0.75f, 0.3f); // Gold

            // Assign buildings
            theme.loggingCamp = buildings[0];
            theme.mine = buildings[1];
            theme.mageTower = buildings[2];
            theme.baseStructure = buildings[3];
            theme.centerObjective = buildings[4];

            // Assign units
            theme.meleeUnit = units[0];
            theme.rangedUnit = units[1];
            theme.armouredUnit = units[2];
            theme.mountedUnit = units[3];

            // Center objective bonus
            theme.bonusType = CenterBonusType.IncreaseArmor;
            theme.bonusAmount = 10f;
            theme.bonusDescription = "Capturing the Ancient Monument grants +10 armor to all your units, " +
                                   "making them significantly more durable in combat!";

            CreateAsset(theme, "Assets/Data/Themes/Fantasy_Theme.asset");

            return theme;
        }

        private static void CreateAsset(ScriptableObject asset, string path)
        {
            // Delete existing asset if it exists
            if (File.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
            }

            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"Created: {path}");
        }
    }
}
