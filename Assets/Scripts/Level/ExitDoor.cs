using GravityFlip.Audio;
using GravityFlip.Core;
using GravityFlip.Player;
using UnityEngine;

namespace GravityFlip.Level
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class ExitDoor : MonoBehaviour
    {
        [SerializeField] private ProgressManager progressManager;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color lockedColor = Color.red;
        [SerializeField] private Color openColor = Color.green;
        [SerializeField] private AudioManager audioManager;

        private bool wasOpen;

        private void Awake()
        {
            if (progressManager == null)
            {
                progressManager = FindObjectOfType<ProgressManager>();
            }

            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (audioManager == null)
            {
                audioManager = FindObjectOfType<AudioManager>();
            }

            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnEnable()
        {
            if (progressManager != null)
            {
                progressManager.ProgressChanged += HandleProgressChanged;
                progressManager.ProgressCompleted += HandleProgressCompleted;
                HandleProgressChanged(progressManager.CollectedCount, progressManager.TotalCount);
            }
        }

        private void OnDisable()
        {
            if (progressManager != null)
            {
                progressManager.ProgressChanged -= HandleProgressChanged;
                progressManager.ProgressCompleted -= HandleProgressCompleted;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController2D>() == null || progressManager == null)
            {
                return;
            }

            if (progressManager.IsComplete)
            {
                gameManager?.CompleteLevel();
            }
            else
            {
                Debug.Log("Exit is locked. Collect all required items first.");
            }
        }

        private void HandleProgressChanged(int collectedCount, int totalCount)
        {
            SetOpenState(totalCount > 0 && collectedCount >= totalCount);
        }

        private void HandleProgressCompleted()
        {
            SetOpenState(true);
        }

        private void SetOpenState(bool isOpen)
        {
            if (isOpen && !wasOpen)
            {
                audioManager?.PlayDoorUnlock();
            }

            wasOpen = isOpen;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = isOpen ? openColor : lockedColor;
            }
        }
    }
}
