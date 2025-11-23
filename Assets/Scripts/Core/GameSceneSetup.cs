using UnityEngine;
using Sjur.Lanes;

namespace Sjur.Core
{
    /// <summary>
    /// Helper script to quickly set up a game scene with placeholders
    /// Can be run in editor or at runtime
    /// </summary>
    public class GameSceneSetup : MonoBehaviour
    {
        [Header("Scene Configuration")]
        [SerializeField] private float laneSpacing = 5f;
        [SerializeField] private float mapLength = 50f;
        [SerializeField] private int numberOfLanes = 5;

        [Header("Placeholder Materials")]
        [SerializeField] private Material team0Material;
        [SerializeField] private Material team1Material;
        [SerializeField] private Material centerMaterial;
        [SerializeField] private Material laneMaterial;

        [ContextMenu("Setup Game Scene")]
        public void SetupScene()
        {
            Debug.Log("Setting up game scene with placeholders...");

            CreateLanes();
            CreateBases();
            CreateCenterObjective();
            CreateCamera();

            Debug.Log("Scene setup complete!");
        }

        private void CreateLanes()
        {
            GameObject lanesParent = new GameObject("Lanes");

            float centerLaneOffset = -(numberOfLanes - 1) * laneSpacing / 2f;

            for (int i = 0; i < numberOfLanes; i++)
            {
                GameObject laneObj = new GameObject($"Lane_{i}");
                laneObj.transform.parent = lanesParent.transform;

                Lane lane = laneObj.AddComponent<Lane>();

                // Calculate lane X position
                float laneX = centerLaneOffset + (i * laneSpacing);

                // Create waypoints for team 0 (going forward)
                CreateWaypointsForLane(laneObj.transform, 0, laneX);

                // Create waypoints for team 1 (going backward)
                CreateWaypointsForLane(laneObj.transform, 1, laneX);

                // Create visual lane path
                CreateLaneVisual(laneObj.transform, laneX);
            }
        }

        private void CreateWaypointsForLane(Transform laneParent, int teamId, float laneX)
        {
            GameObject waypointsParent = new GameObject($"Waypoints_Team{teamId}");
            waypointsParent.transform.parent = laneParent;

            int numWaypoints = 5; // Start, quarter, center, three-quarter, end
            float direction = teamId == 0 ? 1f : -1f;
            float startZ = teamId == 0 ? -mapLength / 2f : mapLength / 2f;

            for (int i = 0; i < numWaypoints; i++)
            {
                GameObject waypoint = new GameObject($"Waypoint_{i}");
                waypoint.transform.parent = waypointsParent.transform;

                float t = i / (float)(numWaypoints - 1);
                float z = startZ + (direction * mapLength * t);

                waypoint.transform.position = new Vector3(laneX, 0, z);
            }
        }

        private void CreateLaneVisual(Transform laneParent, float laneX)
        {
            GameObject laneGround = GameObject.CreatePrimitive(PrimitiveType.Plane);
            laneGround.name = "LaneGround";
            laneGround.transform.parent = laneParent;
            laneGround.transform.position = new Vector3(laneX, -0.1f, 0);
            laneGround.transform.localScale = new Vector3(0.5f, 1f, mapLength / 10f);

            if (laneMaterial != null)
            {
                laneGround.GetComponent<Renderer>().material = laneMaterial;
            }
        }

        private void CreateBases()
        {
            // Team 0 Base (bottom)
            GameObject team0Base = GameObject.CreatePrimitive(PrimitiveType.Cube);
            team0Base.name = "Team0_Base";
            team0Base.transform.position = new Vector3(0, 1.5f, -mapLength / 2f - 5f);
            team0Base.transform.localScale = new Vector3(10f, 3f, 5f);

            if (team0Material != null)
            {
                team0Base.GetComponent<Renderer>().material = team0Material;
            }

            // Team 1 Base (top)
            GameObject team1Base = GameObject.CreatePrimitive(PrimitiveType.Cube);
            team1Base.name = "Team1_Base";
            team1Base.transform.position = new Vector3(0, 1.5f, mapLength / 2f + 5f);
            team1Base.transform.localScale = new Vector3(10f, 3f, 5f);

            if (team1Material != null)
            {
                team1Base.GetComponent<Renderer>().material = team1Material;
            }
        }

        private void CreateCenterObjective()
        {
            GameObject centerObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            centerObj.name = "CenterObjective";
            centerObj.transform.position = new Vector3(0, 2f, 0);
            centerObj.transform.localScale = new Vector3(3f, 2f, 3f);

            if (centerMaterial != null)
            {
                centerObj.GetComponent<Renderer>().material = centerMaterial;
            }
        }

        private void CreateCamera()
        {
            GameObject cameraObj = GameObject.Find("Main Camera");
            if (cameraObj == null)
            {
                cameraObj = new GameObject("Main Camera");
                cameraObj.tag = "MainCamera";
                cameraObj.AddComponent<Camera>();
            }

            // Set isometric view
            cameraObj.transform.position = new Vector3(0, 30, -20);
            cameraObj.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

            // Add camera controller
            if (cameraObj.GetComponent<CameraController>() == null)
            {
                cameraObj.AddComponent<CameraController>();
            }
        }

        [ContextMenu("Clear Scene Objects")]
        public void ClearScene()
        {
            // Helper to clean up generated objects
            GameObject lanes = GameObject.Find("Lanes");
            if (lanes != null) DestroyImmediate(lanes);

            GameObject team0Base = GameObject.Find("Team0_Base");
            if (team0Base != null) DestroyImmediate(team0Base);

            GameObject team1Base = GameObject.Find("Team1_Base");
            if (team1Base != null) DestroyImmediate(team1Base);

            GameObject centerObj = GameObject.Find("CenterObjective");
            if (centerObj != null) DestroyImmediate(centerObj);

            Debug.Log("Scene objects cleared!");
        }
    }
}
