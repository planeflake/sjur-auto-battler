using System;
using System.Collections.Generic;
using UnityEngine;
using Sjur.Units;

namespace Sjur.Combat
{
    /// <summary>
    /// Manages combat calculations, damage modifiers, and team bonuses
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        private Dictionary<int, TeamBonuses> teamBonuses = new Dictionary<int, TeamBonuses>();

        public event Action<Unit, Unit, float> OnDamageDealt;

        private void Awake()
        {
            // Initialize team bonuses
            teamBonuses[0] = new TeamBonuses();
            teamBonuses[1] = new TeamBonuses();
        }

        /// <summary>
        /// Calculate final damage with all bonuses applied
        /// </summary>
        public float CalculateDamage(Unit attacker, Unit target)
        {
            if (attacker == null || target == null) return 0f;

            float baseDamage = attacker.Data.CalculateDamage(target.Data);

            // Apply team bonuses
            if (teamBonuses.TryGetValue(attacker.TeamId, out TeamBonuses bonuses))
            {
                baseDamage = ApplyBonuses(baseDamage, attacker.Data, bonuses);
            }

            OnDamageDealt?.Invoke(attacker, target, baseDamage);

            return baseDamage;
        }

        private float ApplyBonuses(float baseDamage, UnitData attackerData, TeamBonuses bonuses)
        {
            float finalDamage = baseDamage;

            // Apply damage type bonuses
            switch (attackerData.damageType)
            {
                case DamageType.Physical:
                    finalDamage += bonuses.meleeDamageBonus;
                    break;
                case DamageType.Pierce:
                    finalDamage += bonuses.rangedDamageBonus;
                    break;
            }

            // Apply percentage bonuses
            finalDamage *= (1f + bonuses.damageMultiplier);

            return finalDamage;
        }

        /// <summary>
        /// Apply center objective bonus to team
        /// </summary>
        public void ApplyCenterObjectiveBonus(int teamId, Themes.CenterBonusType bonusType, float amount)
        {
            if (!teamBonuses.ContainsKey(teamId))
            {
                teamBonuses[teamId] = new TeamBonuses();
            }

            TeamBonuses bonuses = teamBonuses[teamId];

            switch (bonusType)
            {
                case Themes.CenterBonusType.IncreaseArmor:
                    bonuses.armorBonus += amount;
                    Debug.Log($"Team {teamId} gained +{amount} armor!");
                    break;

                case Themes.CenterBonusType.IncreaseRangedDamage:
                    bonuses.rangedDamageBonus += amount;
                    Debug.Log($"Team {teamId} gained +{amount} ranged damage!");
                    break;

                case Themes.CenterBonusType.IncreaseMeleeDamage:
                    bonuses.meleeDamageBonus += amount;
                    Debug.Log($"Team {teamId} gained +{amount} melee damage!");
                    break;

                case Themes.CenterBonusType.IncreaseMovementSpeed:
                    bonuses.movementSpeedBonus += amount;
                    Debug.Log($"Team {teamId} gained +{amount} movement speed!");
                    break;

                case Themes.CenterBonusType.IncreaseAttackSpeed:
                    bonuses.attackSpeedBonus += amount;
                    Debug.Log($"Team {teamId} gained +{amount}% attack speed!");
                    break;
            }
        }

        /// <summary>
        /// Get team bonuses for UI display
        /// </summary>
        public TeamBonuses GetTeamBonuses(int teamId)
        {
            if (teamBonuses.TryGetValue(teamId, out TeamBonuses bonuses))
            {
                return bonuses;
            }
            return new TeamBonuses();
        }

        /// <summary>
        /// Apply bonuses to a newly spawned unit
        /// </summary>
        public void ApplyBonusesToUnit(Unit unit)
        {
            if (!teamBonuses.TryGetValue(unit.TeamId, out TeamBonuses bonuses))
                return;

            // Note: In a real implementation, you'd modify the unit's stats
            // This would require adding stat modifier support to the Unit class
            // For now, we're tracking bonuses centrally in CombatManager
        }
    }

    /// <summary>
    /// Tracks all combat bonuses for a team
    /// </summary>
    [System.Serializable]
    public class TeamBonuses
    {
        public float armorBonus = 0f;
        public float meleeDamageBonus = 0f;
        public float rangedDamageBonus = 0f;
        public float movementSpeedBonus = 0f;
        public float attackSpeedBonus = 0f;
        public float damageMultiplier = 0f; // Percentage bonus (0.1 = 10% increase)
        public float resourceGenerationBonus = 0f;
        public bool hasSpecialUnitUnlocked = false;
    }
}
