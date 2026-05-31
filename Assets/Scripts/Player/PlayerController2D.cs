using GravityFlip.Audio;
using GravityFlip.Core;
using GravityFlip.Level;
using UnityEngine;

namespace GravityFlip.Player
{
    [DefaultExecutionOrder(150)]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlatformRider2D))]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float jumpSpeed = 12f;
        [SerializeField] private float customGravity = 28f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask walkableLayers;
        [SerializeField] private float groundCheckDistance = 0.08f;
        [SerializeField] private float leaveSupportSpeedThreshold = 1f;
        [SerializeField] private float platformInteractionBlockDuration = 0.25f;

        [Header("References")]
        [SerializeField] private GravityController gravityController;
        [SerializeField] private AudioManager audioManager;

        private readonly RaycastHit2D[] groundHits = new RaycastHit2D[4];

        private Rigidbody2D body;
        private Collider2D playerCollider;
        private PlatformRider2D platformRider;
        private Vector3 initialScale;
        private float horizontalInput;
        private bool jumpRequested;

        private Vector2 GravityDirection => gravityController != null
            ? gravityController.GravityDirection
            : Vector2.down;

        private Vector2 JumpDirection => -GravityDirection;

        private bool IsOnMovingPlatform
        {
            get
            {
                if (platformRider == null)
                {
                    return false;
                }

                if (platformRider.IsInContactWithPlatform)
                {
                    return true;
                }

                MovingPlatform2D platform = platformRider.ActivePlatform;
                return platform != null && platform.IsCarrying(body);
            }
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            playerCollider = GetComponent<Collider2D>();
            platformRider = GetComponent<PlatformRider2D>();
            initialScale = transform.localScale;

            if (gravityController == null)
            {
                gravityController = FindObjectOfType<GravityController>();
            }

            if (audioManager == null)
            {
                audioManager = FindObjectOfType<AudioManager>();
            }

            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void OnEnable()
        {
            if (gravityController != null)
            {
                gravityController.GravityDirectionChanged += HandleGravityDirectionChanged;
                HandleGravityDirectionChanged(gravityController.GravityDirection);
            }
        }

        private void OnDisable()
        {
            if (gravityController != null)
            {
                gravityController.GravityDirectionChanged -= HandleGravityDirectionChanged;
            }
        }

        private void Update()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump"))
            {
                jumpRequested = true;
                if (IsOnMovingPlatform)
                {
                    BeginPlatformInteractionBlock();
                }
            }

            if (Input.GetButtonDown("Fire3"))
            {
                BeginPlatformInteractionBlock();
                gravityController?.FlipGravity();
            }
        }

        private void FixedUpdate()
        {
            if (jumpRequested)
            {
                TryJump();
                jumpRequested = false;
            }

            if (ShouldApplyPlatformRiding())
            {
                ApplyPlatformRidingGravityOnly();
            }
            else
            {
                ApplyHorizontalMovement();
            }

            ApplyCustomGravity();
        }

        private void ApplyHorizontalMovement()
        {
            Vector2 gravityVelocity = Vector2.Dot(body.velocity, GravityDirection) * GravityDirection;
            body.velocity = gravityVelocity + Vector2.right * (horizontalInput * moveSpeed);
        }

        public void ApplyPlatformStrafeAfterCarry()
        {
            Vector2 inputDelta = Vector2.right * (horizontalInput * moveSpeed * Time.fixedDeltaTime);
            if (inputDelta.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            body.MovePosition(body.position + inputDelta);
        }

        private void ApplyPlatformRidingGravityOnly()
        {
            Vector2 gravityVelocity = Vector2.Dot(body.velocity, GravityDirection) * GravityDirection;
            body.velocity = gravityVelocity;
        }

        private void TryJump()
        {
            if (!CanJump())
            {
                return;
            }

            BeginPlatformInteractionBlock();

            Vector2 horizontalVelocity = Vector2.Dot(body.velocity, Vector2.right) * Vector2.right;
            body.velocity = horizontalVelocity + JumpDirection * jumpSpeed;
            audioManager?.PlayJump();
        }

        private void ApplyCustomGravity()
        {
            body.velocity += GravityDirection * (customGravity * Time.fixedDeltaTime);
        }

        private bool CanJump()
        {
            if (HasWalkableSurfaceContact())
            {
                return true;
            }

            return IsOnMovingPlatform && !IsLeavingSupport();
        }

        private bool ShouldApplyPlatformRiding()
        {
            if (platformRider != null && platformRider.BlocksPlatformInteraction)
            {
                return false;
            }

            return IsOnMovingPlatform;
        }

        private bool IsLeavingSupport()
        {
            float leaveSpeed = Vector2.Dot(body.velocity, JumpDirection);
            return leaveSpeed > leaveSupportSpeedThreshold;
        }

        private bool HasWalkableSurfaceContact()
        {
            ContactFilter2D contactFilter = new ContactFilter2D
            {
                useTriggers = false
            };
            contactFilter.SetLayerMask(walkableLayers);

            int hitCount = playerCollider.Cast(GravityDirection, contactFilter, groundHits, groundCheckDistance);
            return hitCount > 0;
        }

        private void BeginPlatformInteractionBlock()
        {
            platformRider?.BeginPlatformInteractionBlock(platformInteractionBlockDuration);
            platformRider?.ReleaseFromPlatform();
        }

        private void HandleGravityDirectionChanged(Vector2 gravityDirection)
        {
            float yScale = gravityDirection == Vector2.up
                ? -Mathf.Abs(initialScale.y)
                : Mathf.Abs(initialScale.y);

            transform.localScale = new Vector3(initialScale.x, yScale, initialScale.z);
        }

        public void ResetTo(Vector3 position)
        {
            transform.SetParent(null);
            platformRider?.ClearPlatformContact();
            body.position = position;
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            jumpRequested = false;
            horizontalInput = 0f;
        }
    }
}
