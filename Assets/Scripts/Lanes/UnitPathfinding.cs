using System.Collections.Generic;
using UnityEngine;
using Sjur.Units;

namespace Sjur.Lanes
{
    /// <summary>
    /// Handles unit movement along lane waypoints
    /// Attached to units that should follow lane paths
    /// </summary>
    [RequireComponent(typeof(Unit))]
    public class UnitPathfinding : MonoBehaviour
    {
        private Unit unit;
        private Lane assignedLane;
        private List<Transform> waypoints;
        private int currentWaypointIndex = 0;
        private bool hasReachedCenter = false;

        [Header("Pathfinding Settings")]
        [SerializeField] private float waypointReachThreshold = 1.5f;

        public Lane AssignedLane => assignedLane;
        public int CurrentWaypointIndex => currentWaypointIndex;

        private void Awake()
        {
            unit = GetComponent<Unit>();
        }

        private void Start()
        {
            unit.OnUnitReachedDestination += OnReachedWaypoint;
        }

        private void OnDestroy()
        {
            if (unit != null)
            {
                unit.OnUnitReachedDestination -= OnReachedWaypoint;
            }
        }

        /// <summary>
        /// Initialize pathfinding with a specific lane
        /// </summary>
        public void Initialize(Lane lane, int teamId)
        {
            assignedLane = lane;
            waypoints = lane.GetWaypoints(teamId);
            currentWaypointIndex = 0;

            if (waypoints.Count > 0)
            {
                // Start moving to first waypoint (after spawn)
                MoveToNextWaypoint();
            }
        }

        private void Update()
        {
            if (unit.IsDead || waypoints == null || waypoints.Count == 0)
                return;

            // Check if we're close enough to the current waypoint
            if (currentWaypointIndex < waypoints.Count)
            {
                Transform currentWaypoint = waypoints[currentWaypointIndex];
                if (currentWaypoint != null)
                {
                    float distance = Vector3.Distance(transform.position, currentWaypoint.position);

                    if (distance < waypointReachThreshold && unit.CurrentState == UnitState.Moving)
                    {
                        OnReachedWaypoint(unit);
                    }
                }
            }
        }

        private void OnReachedWaypoint(Unit reachedUnit)
        {
            if (reachedUnit != unit) return;

            currentWaypointIndex++;

            // Check if we've reached the center objective
            if (IsCenterWaypoint(currentWaypointIndex - 1))
            {
                hasReachedCenter = true;
                // Center objective handling would go here
                // For now, continue to next waypoint
            }

            // Move to next waypoint if available
            MoveToNextWaypoint();
        }

        private void MoveToNextWaypoint()
        {
            if (currentWaypointIndex >= waypoints.Count)
            {
                // Reached final destination (enemy base)
                return;
            }

            Transform nextWaypoint = waypoints[currentWaypointIndex];
            if (nextWaypoint != null)
            {
                unit.MoveTo(nextWaypoint.position);
            }
        }

        private bool IsCenterWaypoint(int waypointIndex)
        {
            // Assuming the center is roughly in the middle of the waypoint list
            // You can make this more sophisticated based on your lane setup
            int centerIndex = waypoints.Count / 2;
            return waypointIndex == centerIndex;
        }

        /// <summary>
        /// Force unit to specific waypoint
        /// </summary>
        public void SetWaypoint(int waypointIndex)
        {
            if (waypointIndex >= 0 && waypointIndex < waypoints.Count)
            {
                currentWaypointIndex = waypointIndex;
                MoveToNextWaypoint();
            }
        }

        /// <summary>
        /// Get progress through the lane (0-1)
        /// </summary>
        public float GetLaneProgress()
        {
            if (waypoints == null || waypoints.Count == 0)
                return 0f;

            return (float)currentWaypointIndex / waypoints.Count;
        }
    }
}
