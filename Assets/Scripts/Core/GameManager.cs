using GravityFlip.Audio;
using GravityFlip.Player;
using GravityFlip.UI;
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
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private GameFlowController gameFlow;

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

            if (audioManager == null)
            {
                audioManager = FindObjectOfType<AudioManager>();
            }

            if (gameFlow == null)
            {
                gameFlow = GetComponent<GameFlowController>();
            }

            if (gameFlow == null)
            {
                gameFlow = gameObject.AddComponent<GameFlowController>();
            }
        }

        private void Update()
        {
            if (GameFlowController.IsGameplayBlocked)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetLevel();
            }
        }

        public void RespawnPlayer()
        {
            RespawnPlayer(playDeathSound: true);
        }

        public void ResetLevel()
        {
            audioManager?.PlayLevelReset();
            RespawnPlayer(playDeathSound: false);
        }

        public void CompleteLevel()
        {
            if (IsLevelComplete)
            {
                return;
            }

            IsLevelComplete = true;
            audioManager?.PlayLevelComplete();
            gameFlow?.ShowWinPanel();
            Debug.Log("Level complete.");
        }

        private void RespawnPlayer(bool playDeathSound)
        {
            IsLevelComplete = false;

            if (playDeathSound)
            {
                audioManager?.PlayDeath();
            }

            gravityController?.ResetGravity();
            progressManager?.ResetProgress();

            if (player != null && spawnPoint != null)
            {
                player.ResetTo(spawnPoint.position);
            }
        }
    }
}
