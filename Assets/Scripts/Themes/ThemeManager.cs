using System;
using UnityEngine;

namespace Sjur.Themes
{
    /// <summary>
    /// Manages the current active theme and applies theme-specific settings
    /// Handles theme switching for genre flexibility
    /// </summary>
    public class ThemeManager : MonoBehaviour
    {
        [Header("Theme Configuration")]
        [SerializeField] private ThemeData currentTheme;
        [SerializeField] private ThemeData[] availableThemes;

        public ThemeData CurrentTheme => currentTheme;

        public event Action<ThemeData> OnThemeChanged;

        private void Awake()
        {
            if (currentTheme != null)
            {
                ApplyTheme(currentTheme);
            }
        }

        /// <summary>
        /// Switch to a new theme by name
        /// </summary>
        public bool SwitchTheme(string themeName)
        {
            ThemeData newTheme = Array.Find(availableThemes, t => t.themeName == themeName);

            if (newTheme != null)
            {
                currentTheme = newTheme;
                ApplyTheme(newTheme);
                return true;
            }

            Debug.LogWarning($"Theme '{themeName}' not found!");
            return false;
        }

        /// <summary>
        /// Switch to a new theme directly
        /// </summary>
        public void SwitchTheme(ThemeData newTheme)
        {
            if (newTheme != null)
            {
                currentTheme = newTheme;
                ApplyTheme(newTheme);
            }
        }

        /// <summary>
        /// Apply the theme's visual and gameplay settings
        /// </summary>
        private void ApplyTheme(ThemeData theme)
        {
            Debug.Log($"Applying theme: {theme.themeName} ({theme.genre})");

            // Apply visual settings (materials, colors)
            // This will be expanded when we have actual visual components

            // Play theme music if available
            if (theme.themeMusic != null)
            {
                // TODO: Integrate with audio manager
                Debug.Log($"Playing theme music: {theme.themeMusic.name}");
            }

            OnThemeChanged?.Invoke(theme);
        }

        /// <summary>
        /// Get unit data by type from current theme
        /// </summary>
        public UnitData GetUnitData(UnitType unitType)
        {
            if (currentTheme == null)
            {
                Debug.LogError("No theme selected!");
                return null;
            }

            return unitType switch
            {
                UnitType.Melee => currentTheme.meleeUnit,
                UnitType.Ranged => currentTheme.rangedUnit,
                UnitType.Armoured => currentTheme.armouredUnit,
                UnitType.Mounted => currentTheme.mountedUnit,
                _ => null
            };
        }

        /// <summary>
        /// Get building data by type from current theme
        /// </summary>
        public Buildings.BuildingData GetBuildingData(Buildings.BuildingType buildingType)
        {
            if (currentTheme == null)
            {
                Debug.LogError("No theme selected!");
                return null;
            }

            return buildingType switch
            {
                Buildings.BuildingType.LoggingCamp => currentTheme.loggingCamp,
                Buildings.BuildingType.Mine => currentTheme.mine,
                Buildings.BuildingType.MageTower => currentTheme.mageTower,
                Buildings.BuildingType.Base => currentTheme.baseStructure,
                Buildings.BuildingType.CenterObjective => currentTheme.centerObjective,
                _ => null
            };
        }

        /// <summary>
        /// Apply center objective bonus to player
        /// </summary>
        public void ApplyCenterObjectiveBonus(int teamId)
        {
            if (currentTheme == null) return;

            Debug.Log($"Team {teamId} captured center! Bonus: {currentTheme.bonusType} (+{currentTheme.bonusAmount})");

            // TODO: Apply actual bonus effects
            // This will be implemented when we have the player/team manager
        }
    }

    /// <summary>
    /// Unit type enum (referenced in ThemeData)
    /// </summary>
    public enum UnitType
    {
        Melee,
        Ranged,
        Armoured,
        Mounted
    }
}
