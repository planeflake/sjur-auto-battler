using System.Collections.Generic;
using UnityEngine;

namespace Sjur.Lanes
{
    /// <summary>
    /// Represents a single lane with waypoints from spawn to center to enemy base
    /// </summary>
    public class Lane : MonoBehaviour
    {
        [Header("Lane Configuration")]
        [SerializeField] private int laneId;
        [SerializeField] private List<Transform> waypointsTeam0; // Waypoints for team 0 going to team 1's base
        [SerializeField] private List<Transform> waypointsTeam1; // Waypoints for team 1 going to team 0's base

        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color team0Color = Color.blue;
        [SerializeField] private Color team1Color = Color.red;

        public int LaneId => laneId;

        /// <summary>
        /// Get waypoints for a specific team
        /// </summary>
        public List<Transform> GetWaypoints(int teamId)
        {
            return teamId == 0 ? waypointsTeam0 : waypointsTeam1;
        }

        /// <summary>
        /// Get next waypoint for a unit
        /// </summary>
        public Transform GetNextWaypoint(int teamId, int currentWaypointIndex)
        {
            List<Transform> waypoints = GetWaypoints(teamId);

            if (currentWaypointIndex < 0 || currentWaypointIndex >= waypoints.Count - 1)
                return null;

            return waypoints[currentWaypointIndex + 1];
        }

        /// <summary>
        /// Get first waypoint (spawn point)
        /// </summary>
        public Transform GetSpawnPoint(int teamId)
        {
            List<Transform> waypoints = GetWaypoints(teamId);
            return waypoints.Count > 0 ? waypoints[0] : null;
        }

        /// <summary>
        /// Get final waypoint (enemy base)
        /// </summary>
        public Transform GetEndPoint(int teamId)
        {
            List<Transform> waypoints = GetWaypoints(teamId);
            return waypoints.Count > 0 ? waypoints[waypoints.Count - 1] : null;
        }

        /// <summary>
        /// Check if position is near a waypoint
        /// </summary>
        public bool IsNearWaypoint(Vector3 position, Transform waypoint, float threshold = 1f)
        {
            if (waypoint == null) return false;
            return Vector3.Distance(position, waypoint.position) < threshold;
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            // Draw team 0 path
            DrawWaypointPath(waypointsTeam0, team0Color);

            // Draw team 1 path
            DrawWaypointPath(waypointsTeam1, team1Color);
        }

        private void DrawWaypointPath(List<Transform> waypoints, Color color)
        {
            if (waypoints == null || waypoints.Count < 2) return;

            Gizmos.color = color;

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    Gizmos.DrawWireSphere(waypoints[i].position, 0.5f);
                }
            }

            // Draw last waypoint
            if (waypoints[waypoints.Count - 1] != null)
            {
                Gizmos.DrawWireSphere(waypoints[waypoints.Count - 1].position, 0.5f);
            }
        }
    }
}
