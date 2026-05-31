using GravityFlip.Core;
using GravityFlip.Level;
using UnityEngine;

namespace GravityFlip.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class PlatformRider2D : MonoBehaviour
    {
        [SerializeField] private GravityController gravityController;

        private MovingPlatform2D activePlatform;
        private Rigidbody2D body;
        private float blockPlatformInteractionUntil;

        public bool IsInContactWithPlatform => activePlatform != null;

        public MovingPlatform2D ActivePlatform => activePlatform;

        public bool BlocksPlatformInteraction => Time.time < blockPlatformInteractionUntil;

        private Rigidbody2D Body
        {
            get
            {
                if (body == null)
                {
                    body = GetComponent<Rigidbody2D>();
                }

                return body;
            }
        }

        private Vector2 GravityDirection => gravityController != null
            ? gravityController.GravityDirection
            : Vector2.down;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();

            if (gravityController == null)
            {
                gravityController = FindObjectOfType<GravityController>();
            }
        }

        public void BeginPlatformInteractionBlock(float durationSeconds)
        {
            blockPlatformInteractionUntil = Time.time + durationSeconds;
        }

        public void ReleaseFromPlatform()
        {
            if (activePlatform == null)
            {
                return;
            }

            activePlatform.UnregisterRider(Body);
            activePlatform = null;
        }

        public void ClearPlatformContact()
        {
            ReleaseFromPlatform();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            UpdatePlatformContact(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            UpdatePlatformContact(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (activePlatform == null || collision == null || collision.collider == null)
            {
                return;
            }

            MovingPlatform2D platform = collision.collider.GetComponent<MovingPlatform2D>();
            if (platform != null && platform == activePlatform)
            {
                platform.UnregisterRider(Body);
                activePlatform = null;
            }
        }

        private void UpdatePlatformContact(Collision2D collision)
        {
            if (collision == null || collision.collider == null)
            {
                return;
            }

            MovingPlatform2D platform = collision.collider.GetComponent<MovingPlatform2D>();
            if (platform == null)
            {
                return;
            }

            if (MovingPlatformContact.HasWalkableSupport(
                    collision,
                    GravityDirection,
                    GetComponent<Collider2D>(),
                    collision.collider))
            {
                activePlatform = platform;
                return;
            }

            if (activePlatform == platform)
            {
                platform.UnregisterRider(Body);
                activePlatform = null;
            }
        }
    }
}
