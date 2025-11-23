using System.Collections.Generic;
using UnityEngine;
using Sjur.Units;

namespace Sjur.Lanes
{
    /// <summary>
    /// Manages all lanes and handles unit assignment to lanes
    /// </summary>
    public class LaneManager : MonoBehaviour
    {
        [Header("Lanes")]
        [SerializeField] private List<Lane> lanes = new List<Lane>();

        [Header("Center Objective")]
        [SerializeField] private Transform centerObjective;

        private Dictionary<int, int> unitCountPerLane = new Dictionary<int, int>();

        private void Awake()
        {
            // Initialize lane counters
            for (int i = 0; i < lanes.Count; i++)
            {
                unitCountPerLane[i] = 0;
            }
        }

        /// <summary>
        /// Get a lane by ID
        /// </summary>
        public Lane GetLane(int laneId)
        {
            if (laneId >= 0 && laneId < lanes.Count)
                return lanes[laneId];

            Debug.LogWarning($"Lane {laneId} not found!");
            return null;
        }

        /// <summary>
        /// Assign a unit to the least populated lane
        /// </summary>
        public Lane AssignUnitToLane()
        {
            if (lanes.Count == 0)
            {
                Debug.LogError("No lanes available!");
                return null;
            }

            // Find lane with fewest units
            int bestLaneId = 0;
            int minUnits = int.MaxValue;

            foreach (var kvp in unitCountPerLane)
            {
                if (kvp.Value < minUnits)
                {
                    minUnits = kvp.Value;
                    bestLaneId = kvp.Key;
                }
            }

            unitCountPerLane[bestLaneId]++;
            return lanes[bestLaneId];
        }

        /// <summary>
        /// Assign a unit to a specific lane
        /// </summary>
        public Lane AssignUnitToLane(int laneId)
        {
            Lane lane = GetLane(laneId);
            if (lane != null)
            {
                unitCountPerLane[laneId]++;
            }
            return lane;
        }

        /// <summary>
        /// Randomly assign a unit to a lane
        /// </summary>
        public Lane AssignUnitToRandomLane()
        {
            if (lanes.Count == 0)
            {
                Debug.LogError("No lanes available!");
                return null;
            }

            int randomLaneId = Random.Range(0, lanes.Count);
            return AssignUnitToLane(randomLaneId);
        }

        /// <summary>
        /// Remove unit from lane count
        /// </summary>
        public void RemoveUnitFromLane(int laneId)
        {
            if (unitCountPerLane.ContainsKey(laneId))
            {
                unitCountPerLane[laneId] = Mathf.Max(0, unitCountPerLane[laneId] - 1);
            }
        }

        /// <summary>
        /// Get spawn position for a unit on a specific lane
        /// </summary>
        public Vector3 GetSpawnPosition(int teamId, int laneId)
        {
            Lane lane = GetLane(laneId);
            if (lane != null)
            {
                Transform spawnPoint = lane.GetSpawnPoint(teamId);
                if (spawnPoint != null)
                    return spawnPoint.position;
            }

            Debug.LogWarning($"No spawn point found for team {teamId}, lane {laneId}");
            return Vector3.zero;
        }

        /// <summary>
        /// Get all waypoints for a unit's journey
        /// </summary>
        public List<Transform> GetWaypointsForUnit(int teamId, int laneId)
        {
            Lane lane = GetLane(laneId);
            if (lane != null)
            {
                return lane.GetWaypoints(teamId);
            }

            return new List<Transform>();
        }

        /// <summary>
        /// Get center objective position
        /// </summary>
        public Vector3 GetCenterObjectivePosition()
        {
            return centerObjective != null ? centerObjective.position : Vector3.zero;
        }

        /// <summary>
        /// Get total number of lanes
        /// </summary>
        public int GetLaneCount()
        {
            return lanes.Count;
        }

        /// <summary>
        /// Debug: Get unit count per lane
        /// </summary>
        public Dictionary<int, int> GetLanePopulation()
        {
            return new Dictionary<int, int>(unitCountPerLane);
        }
    }
}
