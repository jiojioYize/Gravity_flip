using GravityFlip.Core;
using UnityEngine;

namespace GravityFlip.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float jumpSpeed = 12f;
        [SerializeField] private float customGravity = 28f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask walkableLayers;
        [SerializeField] private float groundCheckDistance = 0.08f;

        [Header("References")]
        [SerializeField] private GravityController gravityController;

        private readonly RaycastHit2D[] groundHits = new RaycastHit2D[4];

        private Rigidbody2D body;
        private Collider2D playerCollider;
        private Vector3 initialScale;
        private float horizontalInput;
        private bool jumpRequested;

        private Vector2 GravityDirection => gravityController != null
            ? gravityController.GravityDirection
            : Vector2.down;

        private Vector2 JumpDirection => -GravityDirection;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            playerCollider = GetComponent<Collider2D>();
            initialScale = transform.localScale;

            if (gravityController == null)
            {
                gravityController = FindObjectOfType<GravityController>();
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

            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
            {
                jumpRequested = true;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                gravityController?.FlipGravity();
            }
        }

        private void FixedUpdate()
        {
            ApplyHorizontalMovement();

            if (jumpRequested)
            {
                TryJump();
                jumpRequested = false;
            }

            ApplyCustomGravity();
        }

        private void ApplyHorizontalMovement()
        {
            Vector2 gravityVelocity = Vector2.Dot(body.velocity, GravityDirection) * GravityDirection;
            body.velocity = gravityVelocity + Vector2.right * (horizontalInput * moveSpeed);
        }

        private void TryJump()
        {
            if (!IsGrounded())
            {
                return;
            }

            Vector2 horizontalVelocity = Vector2.Dot(body.velocity, Vector2.right) * Vector2.right;
            body.velocity = horizontalVelocity + JumpDirection * jumpSpeed;
        }

        private void ApplyCustomGravity()
        {
            body.velocity += GravityDirection * (customGravity * Time.fixedDeltaTime);
        }

        private bool IsGrounded()
        {
            ContactFilter2D contactFilter = new ContactFilter2D
            {
                useTriggers = false
            };
            contactFilter.SetLayerMask(walkableLayers);

            int hitCount = playerCollider.Cast(GravityDirection, contactFilter, groundHits, groundCheckDistance);
            return hitCount > 0;
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
            body.position = position;
            body.velocity = Vector2.zero;
            body.angularVelocity = 0f;
            jumpRequested = false;
            horizontalInput = 0f;
        }
    }
}
