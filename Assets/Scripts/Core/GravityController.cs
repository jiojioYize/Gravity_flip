using System;
using GravityFlip.Audio;
using UnityEngine;

namespace GravityFlip.Core
{
    public sealed class GravityController : MonoBehaviour
    {
        public event Action<Vector2> GravityDirectionChanged;

        [SerializeField] private Vector2 initialGravityDirection = Vector2.down;
        [SerializeField] private AudioManager audioManager;

        public Vector2 GravityDirection { get; private set; } = Vector2.down;
        public bool IsInverted => GravityDirection == Vector2.up;

        private void Awake()
        {
            if (audioManager == null)
            {
                audioManager = FindObjectOfType<AudioManager>();
            }

            GravityDirection = NormalizeVerticalDirection(initialGravityDirection);
        }

        public void FlipGravity()
        {
            Vector2 previousDirection = GravityDirection;
            SetGravityDirection(-GravityDirection);

            if (GravityDirection != previousDirection)
            {
                audioManager?.PlayFlip();
            }
        }

        public void ResetGravity()
        {
            SetGravityDirection(Vector2.down);
        }

        private void SetGravityDirection(Vector2 direction)
        {
            Vector2 normalizedDirection = NormalizeVerticalDirection(direction);
            if (GravityDirection == normalizedDirection)
            {
                return;
            }

            GravityDirection = normalizedDirection;
            GravityDirectionChanged?.Invoke(GravityDirection);
        }

        private static Vector2 NormalizeVerticalDirection(Vector2 direction)
        {
            return direction.y >= 0f ? Vector2.up : Vector2.down;
        }
    }
}
