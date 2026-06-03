using GravityFlip.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GravityFlip.UI
{
    public sealed class GameplayHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ProgressManager progressManager;
        [SerializeField] private GravityController gravityController;
        [SerializeField] private Text progressText;
        [SerializeField] private Text gravityText;
        [SerializeField] private Text controlsText;

        [Header("Labels")]
        [SerializeField] private string progressLabel = "Keys";
        [SerializeField] private string gravityDownLabel = "Gravity: Down";
        [SerializeField] private string gravityUpLabel = "Gravity: Up";
        [TextArea(1, 3)]
        [SerializeField] private string controlsLabel = "A/D <-/-> Move | Space Jump | Shift Flip Gravity | R Reset";

        private void Awake()
        {
            if (progressManager == null)
            {
                progressManager = FindObjectOfType<ProgressManager>();
            }

            if (gravityController == null)
            {
                gravityController = FindObjectOfType<GravityController>();
            }
        }

        private void Start()
        {
            ApplyControlsLabel();
            RefreshAll();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ApplyControlsLabel();
        }
#endif

        private void ApplyControlsLabel()
        {
            if (controlsText != null && !string.IsNullOrWhiteSpace(controlsLabel))
            {
                controlsText.text = controlsLabel;
            }
        }

        private void OnEnable()
        {
            if (progressManager != null)
            {
                progressManager.ProgressChanged += HandleProgressChanged;
            }

            if (gravityController != null)
            {
                gravityController.GravityDirectionChanged += HandleGravityDirectionChanged;
            }
        }

        private void OnDisable()
        {
            if (progressManager != null)
            {
                progressManager.ProgressChanged -= HandleProgressChanged;
            }

            if (gravityController != null)
            {
                gravityController.GravityDirectionChanged -= HandleGravityDirectionChanged;
            }
        }

        private void HandleProgressChanged(int collectedCount, int totalCount)
        {
            UpdateProgress(collectedCount, totalCount);
        }

        private void HandleGravityDirectionChanged(Vector2 gravityDirection)
        {
            UpdateGravity(gravityDirection);
        }

        private void RefreshAll()
        {
            if (progressManager != null)
            {
                UpdateProgress(progressManager.CollectedCount, progressManager.TotalCount);
            }

            if (gravityController != null)
            {
                UpdateGravity(gravityController.GravityDirection);
            }
        }

        private void UpdateProgress(int collectedCount, int totalCount)
        {
            if (progressText == null)
            {
                return;
            }

            progressText.text = $"{progressLabel} {collectedCount}/{totalCount}";
        }

        private void UpdateGravity(Vector2 gravityDirection)
        {
            if (gravityText == null)
            {
                return;
            }

            gravityText.text = gravityDirection == Vector2.up ? gravityUpLabel : gravityDownLabel;
        }
    }
}
