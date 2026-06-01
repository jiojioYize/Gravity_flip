using System;
using System.Collections;
using GravityFlip.Core;
using UnityEngine;

namespace GravityFlip.Level
{
    public sealed class ShuttlePlatformController : MonoBehaviour
    {
        [Header("Activation")]
        [SerializeField] private Collectible activationCollectible;
        [SerializeField] private ProgressManager progressManager;

        [Header("Platform")]
        [SerializeField] private MovingPlatform2D movingPlatform;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float respawnDelay = 0.5f;

        private bool isSystemActive;
        private bool isRunInProgress;
        private bool hideUsesSetActive;
        private Coroutine respawnRoutine;

        public event Action PlatformRunStarted;
        public event Action PlatformRunEnded;

        public bool IsRunActive => isRunInProgress;

        private SpriteRenderer[] platformRenderers;
        private Collider2D[] platformColliders;

        private void Awake()
        {
            if (progressManager == null)
            {
                progressManager = FindObjectOfType<ProgressManager>();
            }

            if (movingPlatform == null)
            {
                movingPlatform = GetComponent<MovingPlatform2D>();
            }

            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }

            if (movingPlatform != null)
            {
                GameObject platformObject = movingPlatform.gameObject;
                hideUsesSetActive = platformObject != gameObject;
                // Root only — child collectables (C2) manage their own renderers/colliders.
                platformRenderers = platformObject.GetComponents<SpriteRenderer>();
                platformColliders = platformObject.GetComponents<Collider2D>();
            }
        }

        private void OnEnable()
        {
            if (progressManager != null)
            {
                progressManager.ProgressChanged += HandleProgressChanged;
            }

            RefreshActivationState();
        }

        private void OnDisable()
        {
            if (progressManager != null)
            {
                progressManager.ProgressChanged -= HandleProgressChanged;
            }
        }

        private void Start()
        {
            if (!isSystemActive)
            {
                HidePlatform();
            }
        }

        public void NotifyCorridorExited()
        {
            if (!isSystemActive || !isRunInProgress)
            {
                return;
            }

            EndCurrentRun();
        }

        private void HandleProgressChanged(int collected, int total)
        {
            RefreshActivationState();
        }

        private void RefreshActivationState()
        {
            bool shouldBeActive = activationCollectible != null && activationCollectible.IsCollected;

            if (shouldBeActive == isSystemActive)
            {
                return;
            }

            isSystemActive = shouldBeActive;

            if (isSystemActive)
            {
                StartRunIfIdle();
                return;
            }

            StopAllRuns();
            HidePlatform();
            PlatformRunEnded?.Invoke();
        }

        private void StartRunIfIdle()
        {
            if (!isSystemActive || isRunInProgress)
            {
                return;
            }

            BeginRun();
        }

        private void BeginRun()
        {
            if (movingPlatform == null)
            {
                return;
            }

            if (respawnRoutine != null)
            {
                StopCoroutine(respawnRoutine);
                respawnRoutine = null;
            }

            isRunInProgress = true;
            SetPlatformVisible(true);
            movingPlatform.transform.position = spawnPoint.position;
            movingPlatform.ReleaseAllRiders();
            movingPlatform.BeginMovement();
            PlatformRunStarted?.Invoke();
        }

        private void EndCurrentRun()
        {
            if (!isRunInProgress)
            {
                return;
            }

            EndRunAndHidePlatform();

            if (isSystemActive)
            {
                respawnRoutine = StartCoroutine(RespawnAfterDelay());
            }
        }

        private void StopAllRuns()
        {
            bool wasRunning = isRunInProgress;
            isRunInProgress = false;

            if (respawnRoutine != null)
            {
                StopCoroutine(respawnRoutine);
                respawnRoutine = null;
            }

            if (movingPlatform != null)
            {
                movingPlatform.StopMovement();
                movingPlatform.ReleaseAllRiders();
            }

            if (wasRunning)
            {
                PlatformRunEnded?.Invoke();
            }
        }

        private void EndRunAndHidePlatform()
        {
            bool wasRunning = isRunInProgress;
            isRunInProgress = false;

            if (movingPlatform != null)
            {
                movingPlatform.StopMovement();
                movingPlatform.ReleaseAllRiders();
            }

            HidePlatform();

            if (wasRunning)
            {
                PlatformRunEnded?.Invoke();
            }
        }

        private void HidePlatform()
        {
            if (movingPlatform == null)
            {
                return;
            }

            movingPlatform.StopMovement();
            SetPlatformVisible(false);
        }

        private void SetPlatformVisible(bool isVisible)
        {
            if (movingPlatform == null)
            {
                return;
            }

            GameObject platformObject = movingPlatform.gameObject;

            if (hideUsesSetActive)
            {
                platformObject.SetActive(isVisible);
                return;
            }

            foreach (SpriteRenderer renderer in platformRenderers)
            {
                if (renderer != null)
                {
                    renderer.enabled = isVisible;
                }
            }

            foreach (Collider2D collider in platformColliders)
            {
                if (collider != null)
                {
                    collider.enabled = isVisible;
                }
            }
        }

        private IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(respawnDelay);

            respawnRoutine = null;

            if (isSystemActive)
            {
                BeginRun();
            }
        }
    }
}
