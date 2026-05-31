using System.Collections.Generic;
using GravityFlip.Core;
using GravityFlip.Player;
using UnityEngine;

namespace GravityFlip.Level
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class MovingPlatform2D : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private Vector2 moveDirection = Vector2.right;
        [SerializeField] private GravityController gravityController;

        private readonly HashSet<Rigidbody2D> riderBodies = new HashSet<Rigidbody2D>();

        private Rigidbody2D body;
        private bool isMoving;

        public bool IsMoving => isMoving;

        private Vector2 GravityDirection => gravityController != null
            ? gravityController.GravityDirection
            : Vector2.down;

        private void Awake()
        {
            if (gravityController == null)
            {
                gravityController = FindObjectOfType<GravityController>();
            }

            body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.useFullKinematicContacts = true;
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            Collider2D platformCollider = GetComponent<Collider2D>();
            platformCollider.isTrigger = false;
        }

        private void FixedUpdate()
        {
            if (!isMoving)
            {
                return;
            }

            Vector2 previousPosition = body.position;
            Vector2 step = moveDirection.normalized * (moveSpeed * Time.fixedDeltaTime);
            Vector2 newPosition = previousPosition + step;
            Vector2 delta = newPosition - previousPosition;

            body.MovePosition(newPosition);
            CarryRiders(delta);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            UpdateRiderRegistration(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            UpdateRiderRegistration(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            TryUnregisterRider(collision.collider);
        }

        public void BeginMovement()
        {
            isMoving = true;
        }

        public void StopMovement()
        {
            isMoving = false;
        }

        public void ReleaseAllRiders()
        {
            riderBodies.Clear();
        }

        public void UnregisterRider(Rigidbody2D riderBody)
        {
            if (riderBody != null)
            {
                riderBodies.Remove(riderBody);
            }
        }

        public bool IsCarrying(Rigidbody2D riderBody)
        {
            return riderBody != null && riderBodies.Contains(riderBody);
        }

        private void UpdateRiderRegistration(Collision2D collision)
        {
            Collider2D playerCollider = collision.collider;
            Collider2D platformCollider = GetComponent<Collider2D>();

            if (MovingPlatformContact.HasWalkableSupport(
                    collision,
                    GravityDirection,
                    playerCollider,
                    platformCollider))
            {
                TryRegisterRider(playerCollider);
                return;
            }

            TryUnregisterRider(playerCollider);
        }

        private void CarryRiders(Vector2 delta)
        {
            if (delta == Vector2.zero)
            {
                return;
            }

            foreach (Rigidbody2D riderBody in riderBodies)
            {
                if (riderBody == null)
                {
                    continue;
                }

                PlatformRider2D rider = riderBody.GetComponent<PlatformRider2D>();
                if (rider != null && rider.BlocksPlatformInteraction)
                {
                    continue;
                }

                riderBody.MovePosition(riderBody.position + delta);

                PlayerController2D player = riderBody.GetComponent<PlayerController2D>();
                player?.ApplyPlatformStrafeAfterCarry();
            }
        }

        private void TryRegisterRider(Collider2D other)
        {
            if (!isMoving)
            {
                return;
            }

            PlatformRider2D rider = other.GetComponent<PlatformRider2D>();
            if (rider != null && rider.BlocksPlatformInteraction)
            {
                return;
            }

            PlayerController2D player = other.GetComponent<PlayerController2D>();
            if (player == null)
            {
                return;
            }

            Rigidbody2D riderBody = player.GetComponent<Rigidbody2D>();
            if (riderBody != null)
            {
                riderBodies.Add(riderBody);
            }
        }

        private void TryUnregisterRider(Collider2D other)
        {
            PlayerController2D player = other.GetComponent<PlayerController2D>();
            if (player == null)
            {
                return;
            }

            Rigidbody2D riderBody = player.GetComponent<Rigidbody2D>();
            if (riderBody != null)
            {
                riderBodies.Remove(riderBody);
            }
        }
    }
}
