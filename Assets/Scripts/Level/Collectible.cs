using GravityFlip.Audio;
using GravityFlip.Core;
using GravityFlip.Player;
using UnityEngine;

namespace GravityFlip.Level
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Collectible : MonoBehaviour
    {
        [SerializeField] private ProgressManager progressManager;
        [SerializeField] private AudioManager audioManager;

        [Header("Collection rules (optional)")]
        [SerializeField] private bool requireActiveShuttleRun;
        [SerializeField] private ShuttlePlatformController shuttleController;

        public bool IsCollected { get; private set; }

        private void Awake()
        {
            if (progressManager == null)
            {
                progressManager = FindObjectOfType<ProgressManager>();
            }

            if (audioManager == null)
            {
                audioManager = FindObjectOfType<AudioManager>();
            }

            if (shuttleController == null)
            {
                shuttleController = FindObjectOfType<ShuttlePlatformController>();
            }

            progressManager?.RegisterCollectible(this);
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController2D>() == null)
            {
                return;
            }

            if (!CanCollectFrom(other))
            {
                return;
            }

            if (progressManager != null && !IsCollected)
            {
                progressManager.Collect(this);
                audioManager?.PlayCollect();
            }
        }

        private bool CanCollectFrom(Collider2D other)
        {
            if (!requireActiveShuttleRun)
            {
                return true;
            }

            return shuttleController != null && shuttleController.IsRunActive;
        }

        public void MarkCollected()
        {
            IsCollected = true;
            gameObject.SetActive(false);
        }

        public void ResetCollectible()
        {
            IsCollected = false;
            gameObject.SetActive(true);
        }
    }
}
