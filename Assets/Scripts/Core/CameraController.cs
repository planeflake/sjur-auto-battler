using UnityEngine;

namespace Sjur.Core
{
    /// <summary>
    /// Isometric camera controller with panning and zoom
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float edgeScrollSpeed = 15f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minZoom = 10f;
        [SerializeField] private float maxZoom = 50f;

        [Header("Isometric Angle")]
        [SerializeField] private float isometricAngle = 45f;
        [SerializeField] private float rotationY = 45f;

        [Header("Bounds")]
        [SerializeField] private bool useBounds = true;
        [SerializeField] private Vector2 minBounds = new Vector2(-50, -50);
        [SerializeField] private Vector2 maxBounds = new Vector2(50, 50);

        [Header("Edge Scrolling")]
        [SerializeField] private bool enableEdgeScrolling = true;
        [SerializeField] private float edgeScrollBorder = 10f;

        [Header("Keyboard Controls")]
        [SerializeField] private bool enableKeyboardPanning = true;

        private Camera cam;
        private Vector3 targetPosition;
        private float currentZoom;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            targetPosition = transform.position;

            // Set initial isometric rotation
            transform.rotation = Quaternion.Euler(isometricAngle, rotationY, 0f);

            // Initialize zoom
            if (cam.orthographic)
            {
                currentZoom = cam.orthographicSize;
            }
            else
            {
                currentZoom = transform.position.y;
            }
        }

        private void Update()
        {
            HandleKeyboardInput();
            HandleEdgeScrolling();
            HandleZoom();
            MoveCamera();
        }

        private void HandleKeyboardInput()
        {
            if (!enableKeyboardPanning) return;

            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                move += Vector3.forward;

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                move += Vector3.back;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                move += Vector3.left;

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                move += Vector3.right;

            // Adjust movement for isometric angle
            move = Quaternion.Euler(0, rotationY, 0) * move;

            targetPosition += move.normalized * moveSpeed * Time.deltaTime;
        }

        private void HandleEdgeScrolling()
        {
            if (!enableEdgeScrolling) return;

            Vector3 move = Vector3.zero;
            Vector3 mousePos = Input.mousePosition;

            if (mousePos.x < edgeScrollBorder)
                move += Vector3.left;

            if (mousePos.x > Screen.width - edgeScrollBorder)
                move += Vector3.right;

            if (mousePos.y < edgeScrollBorder)
                move += Vector3.back;

            if (mousePos.y > Screen.height - edgeScrollBorder)
                move += Vector3.forward;

            // Adjust movement for isometric angle
            move = Quaternion.Euler(0, rotationY, 0) * move;

            targetPosition += move.normalized * edgeScrollSpeed * Time.deltaTime;
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0)
            {
                currentZoom -= scroll * zoomSpeed;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

                if (cam.orthographic)
                {
                    cam.orthographicSize = currentZoom;
                }
                else
                {
                    // For perspective camera, adjust Y position
                    targetPosition.y = currentZoom;
                }
            }
        }

        private void MoveCamera()
        {
            // Apply bounds
            if (useBounds)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
                targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.y, maxBounds.y);
            }

            // Smooth camera movement
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
        }

        /// <summary>
        /// Focus camera on a specific position
        /// </summary>
        public void FocusOn(Vector3 position)
        {
            targetPosition = new Vector3(position.x, transform.position.y, position.z);
        }

        /// <summary>
        /// Set camera bounds
        /// </summary>
        public void SetBounds(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;
            useBounds = true;
        }

        /// <summary>
        /// Enable/disable camera movement
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            enableKeyboardPanning = enabled;
            enableEdgeScrolling = enabled;
            this.enabled = enabled;
        }
    }
}
