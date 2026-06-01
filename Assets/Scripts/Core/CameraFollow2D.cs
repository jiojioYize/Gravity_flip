using GravityFlip.Player;
using UnityEngine;

namespace GravityFlip.Core
{
    public sealed class CameraFollow2D : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Follow")]
        [SerializeField] private bool followX = true;
        [SerializeField] private bool followY;
        [SerializeField] private bool lockVerticalPosition = true;
        [SerializeField] private float fixedWorldY;
        [SerializeField] private bool useRelativeHorizontalFollow = true;
        [SerializeField] private Vector2 offset = new Vector2(0f, 0f);
        [SerializeField] private bool smoothFollow = true;
        [SerializeField] private float smoothTime = 0.12f;

        [Header("Bounds (optional)")]
        [SerializeField] private bool useBounds;
        [SerializeField] private Vector2 minPosition;
        [SerializeField] private Vector2 maxPosition;

        private float initialCameraX;
        private float initialTargetX;
        private float lockedWorldY;
        private bool hasCapturedInitialPositions;
        private Vector3 smoothVelocity;

        private void Awake()
        {
            if (target == null)
            {
                PlayerController2D player = FindObjectOfType<PlayerController2D>();
                if (player != null)
                {
                    target = player.transform;
                }
            }

            CaptureInitialPositions();
        }

        private void Start()
        {
            CaptureInitialPositions();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            if (!hasCapturedInitialPositions)
            {
                CaptureInitialPositions();
            }

            float desiredX = transform.position.x;
            if (followX)
            {
                if (useRelativeHorizontalFollow)
                {
                    float horizontalDelta = target.position.x - initialTargetX;
                    desiredX = initialCameraX + horizontalDelta + offset.x;
                }
                else
                {
                    desiredX = target.position.x + offset.x;
                }
            }

            float desiredY = transform.position.y;
            if (lockVerticalPosition)
            {
                desiredY = lockedWorldY;
            }
            else if (followY)
            {
                desiredY = target.position.y + offset.y;
            }

            if (useBounds)
            {
                desiredX = Mathf.Clamp(desiredX, minPosition.x, maxPosition.x);
                desiredY = Mathf.Clamp(desiredY, minPosition.y, maxPosition.y);
            }

            Vector3 desiredPosition = new Vector3(desiredX, desiredY, transform.position.z);

            if (smoothFollow)
            {
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    desiredPosition,
                    ref smoothVelocity,
                    smoothTime);
            }
            else
            {
                transform.position = desiredPosition;
            }
        }

        private void CaptureInitialPositions()
        {
            lockedWorldY = fixedWorldY != 0f ? fixedWorldY : transform.position.y;
            initialCameraX = transform.position.x;
            initialTargetX = target != null ? target.position.x : initialCameraX;
            hasCapturedInitialPositions = true;
        }
    }
}
