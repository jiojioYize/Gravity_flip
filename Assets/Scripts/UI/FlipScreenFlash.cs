using System.Collections;
using GravityFlip.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GravityFlip.UI
{
    public sealed class FlipScreenFlash : MonoBehaviour
    {
        [SerializeField] private GravityController gravityController;
        [SerializeField] private Image flashImage;
        [SerializeField] private float flashDuration = 0.12f;
        [SerializeField] private float maxAlpha = 0.35f;

        private Coroutine flashRoutine;

        private void Awake()
        {
            if (gravityController == null)
            {
                gravityController = FindObjectOfType<GravityController>();
            }

            if (flashImage != null)
            {
                SetAlpha(0f);
            }
        }

        private void OnEnable()
        {
            if (gravityController != null)
            {
                gravityController.GravityDirectionChanged += HandleGravityDirectionChanged;
            }
        }

        private void OnDisable()
        {
            if (gravityController != null)
            {
                gravityController.GravityDirectionChanged -= HandleGravityDirectionChanged;
            }
        }

        private void HandleGravityDirectionChanged(Vector2 gravityDirection)
        {
            if (flashImage == null)
            {
                return;
            }

            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }

            flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            float halfDuration = flashDuration * 0.5f;
            float elapsed = 0f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                SetAlpha(Mathf.Lerp(0f, maxAlpha, t));
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                SetAlpha(Mathf.Lerp(maxAlpha, 0f, t));
                yield return null;
            }

            SetAlpha(0f);
            flashRoutine = null;
        }

        private void SetAlpha(float alpha)
        {
            Color color = flashImage.color;
            color.a = alpha;
            flashImage.color = color;
        }
    }
}
