using System;
using UnityEngine;
using Sjur.Buildings;
using Sjur.Units;
using Sjur.Combat;
using Sjur.Themes;

namespace Sjur.Core
{
    /// <summary>
    /// Manages the center objective that teams fight over for bonuses
    /// </summary>
    public class CenterObjective : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Building building;
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private ThemeManager themeManager;

        [Header("Status")]
        [SerializeField] private bool isCaptured = false;
        [SerializeField] private int capturingTeam = -1;

        public bool IsCaptured => isCaptured;
        public int CapturingTeam => capturingTeam;

        public event Action<int> OnObjectiveCaptured;
        public event Action OnObjectiveRespawned;

        private void Awake()
        {
            if (building == null)
            {
                building = GetComponent<Building>();
            }
        }

        private void Start()
        {
            if (building != null)
            {
                building.OnBuildingDestroyed += OnObjectiveDestroyed;
                building.OnBuildingRespawned += OnObjectiveRespawn;
            }
        }

        private void OnDestroy()
        {
            if (building != null)
            {
                building.OnBuildingDestroyed -= OnObjectiveDestroyed;
                building.OnBuildingRespawned -= OnObjectiveRespawn;
            }
        }

        private void OnObjectiveDestroyed(Building destroyedBuilding)
        {
            if (isCaptured) return; // Already captured

            // Find which team destroyed it based on nearby units
            int destroyingTeam = DetermineDestroyingTeam();

            if (destroyingTeam >= 0)
            {
                CaptureObjective(destroyingTeam);
            }
        }

        private int DetermineDestroyingTeam()
        {
            // Check for nearby units to determine which team gets credit
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);

            int team0Units = 0;
            int team1Units = 0;

            foreach (var col in colliders)
            {
                Unit unit = col.GetComponent<Unit>();
                if (unit != null && !unit.IsDead)
                {
                    if (unit.TeamId == 0)
                        team0Units++;
                    else if (unit.TeamId == 1)
                        team1Units++;
                }
            }

            // Team with more units gets credit
            if (team0Units > team1Units)
                return 0;
            else if (team1Units > team0Units)
                return 1;

            return -1; // No clear winner
        }

        private void CaptureObjective(int teamId)
        {
            isCaptured = true;
            capturingTeam = teamId;

            Debug.Log($"Team {teamId} captured the center objective!");

            // Apply bonus to team
            if (themeManager != null && combatManager != null)
            {
                ThemeData theme = themeManager.CurrentTheme;
                if (theme != null)
                {
                    combatManager.ApplyCenterObjectiveBonus(teamId, theme.bonusType, theme.bonusAmount);
                    themeManager.ApplyCenterObjectiveBonus(teamId);
                }
            }

            OnObjectiveCaptured?.Invoke(teamId);
        }

        private void OnObjectiveRespawn(Building respawnedBuilding)
        {
            isCaptured = false;
            capturingTeam = -1;

            Debug.Log("Center objective has respawned!");
            OnObjectiveRespawned?.Invoke();
        }

        /// <summary>
        /// Manually trigger damage to objective (for testing)
        /// </summary>
        public void DealDamage(float damage)
        {
            if (building != null)
            {
                building.TakeDamage(damage);
            }
        }

        /// <summary>
        /// Get current health percentage
        /// </summary>
        public float GetHealthPercentage()
        {
            if (building != null)
            {
                return building.CurrentHealth / building.Data.maxHealth;
            }
            return 0f;
        }
    }
}
