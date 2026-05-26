using GravityFlip.Player;
using UnityEngine;

namespace GravityFlip.Core
{
    public sealed class GameManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private PlayerController2D player;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GravityController gravityController;
        [SerializeField] private ProgressManager progressManager;

        public bool IsLevelComplete { get; private set; }

        private void Awake()
        {
            if (player == null)
            {
                player = FindObjectOfType<PlayerController2D>();
            }

            if (gravityController == null)
            {
                gravityController = FindObjectOfType<GravityController>();
            }

            if (progressManager == null)
            {
                progressManager = FindObjectOfType<ProgressManager>();
            }
        }

        public void RespawnPlayer()
        {
            IsLevelComplete = false;
            gravityController?.ResetGravity();
            progressManager?.ResetProgress();

            if (player != null && spawnPoint != null)
            {
                player.ResetTo(spawnPoint.position);
            }
        }

        public void CompleteLevel()
        {
            if (IsLevelComplete)
            {
                return;
            }

            IsLevelComplete = true;
            Debug.Log("Level complete.");
        }
    }
}
