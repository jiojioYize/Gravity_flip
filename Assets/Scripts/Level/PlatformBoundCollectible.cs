using GravityFlip.Core;
using UnityEngine;

namespace GravityFlip.Level
{
    /// <summary>
    /// Collectable that only appears during an active shuttle platform run (C2).
    /// Keeps the GameObject active so <see cref="Collectible"/> registers with progress at load.
    /// </summary>
    [RequireComponent(typeof(Collectible))]
    public sealed class PlatformBoundCollectible : MonoBehaviour
    {
        [SerializeField] private ShuttlePlatformController shuttleController;
        [SerializeField] private ProgressManager progressManager;
        [SerializeField] private SpriteRenderer pickupRenderer;
        [SerializeField] private Collider2D pickupCollider;

        private Collectible collectible;

        private void Awake()
        {
            collectible = GetComponent<Collectible>();

            if (shuttleController == null)
            {
                shuttleController = FindObjectOfType<ShuttlePlatformController>();
            }

            if (progressManager == null)
            {
                progressManager = FindObjectOfType<ProgressManager>();
            }

            if (pickupRenderer == null)
            {
                pickupRenderer = GetComponent<SpriteRenderer>();
            }

            if (pickupCollider == null)
            {
                pickupCollider = GetComponent<Collider2D>();
            }

            HidePickupVisuals();
        }

        private void OnEnable()
        {
            if (shuttleController != null)
            {
                shuttleController.PlatformRunStarted += HandlePlatformRunStarted;
                shuttleController.PlatformRunEnded += HandlePlatformRunEnded;
            }

            if (progressManager != null)
            {
                progressManager.ProgressChanged += HandleProgressChanged;
            }

            RefreshVisibility();
        }

        private void OnDisable()
        {
            if (shuttleController != null)
            {
                shuttleController.PlatformRunStarted -= HandlePlatformRunStarted;
                shuttleController.PlatformRunEnded -= HandlePlatformRunEnded;
            }

            if (progressManager != null)
            {
                progressManager.ProgressChanged -= HandleProgressChanged;
            }
        }

        private void HandlePlatformRunStarted()
        {
            RefreshVisibility();
        }

        private void HandlePlatformRunEnded()
        {
            HidePickupVisuals();
        }

        private void HandleProgressChanged(int collected, int total)
        {
            RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            if (collectible != null && collectible.IsCollected)
            {
                return;
            }

            if (shuttleController != null && shuttleController.IsRunActive)
            {
                ShowPickupVisuals();
                return;
            }

            HidePickupVisuals();
        }

        private void ShowPickupVisuals()
        {
            if (pickupRenderer != null)
            {
                pickupRenderer.enabled = true;
            }

            if (pickupCollider != null)
            {
                pickupCollider.enabled = true;
            }
        }

        private void HidePickupVisuals()
        {
            if (pickupRenderer != null)
            {
                pickupRenderer.enabled = false;
            }

            if (pickupCollider != null)
            {
                pickupCollider.enabled = false;
            }
        }
    }
}
