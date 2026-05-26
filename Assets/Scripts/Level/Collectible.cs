using GravityFlip.Core;
using GravityFlip.Player;
using UnityEngine;

namespace GravityFlip.Level
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Collectible : MonoBehaviour
    {
        [SerializeField] private ProgressManager progressManager;

        public bool IsCollected { get; private set; }

        private void Awake()
        {
            if (progressManager == null)
            {
                progressManager = FindObjectOfType<ProgressManager>();
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

            progressManager?.Collect(this);
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
