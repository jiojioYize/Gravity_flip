using UnityEngine;

namespace GravityFlip.UI
{
    /// <summary>
    /// Pins a UI RectTransform to a screen corner or bottom-center with pixel margins.
    /// Use on Screen Space - Overlay canvas children so HUD does not move with the world camera.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class HudScreenAnchor : MonoBehaviour
    {
        public enum AnchorPreset
        {
            TopLeft,
            TopRight,
            BottomCenter
        }

        [SerializeField] private AnchorPreset preset = AnchorPreset.TopLeft;
        [SerializeField] private Vector2 margin = new Vector2(24f, 24f);
        [SerializeField] private bool applyOnAwake = true;

        private void Awake()
        {
            if (applyOnAwake)
            {
                ApplyAnchor();
            }
        }

        public void ApplyAnchor()
        {
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform == null)
            {
                return;
            }

            switch (preset)
            {
                case AnchorPreset.TopLeft:
                    rectTransform.anchorMin = new Vector2(0f, 1f);
                    rectTransform.anchorMax = new Vector2(0f, 1f);
                    rectTransform.pivot = new Vector2(0f, 1f);
                    rectTransform.anchoredPosition = new Vector2(margin.x, -margin.y);
                    break;

                case AnchorPreset.TopRight:
                    rectTransform.anchorMin = new Vector2(1f, 1f);
                    rectTransform.anchorMax = new Vector2(1f, 1f);
                    rectTransform.pivot = new Vector2(1f, 1f);
                    rectTransform.anchoredPosition = new Vector2(-margin.x, -margin.y);
                    break;

                case AnchorPreset.BottomCenter:
                    rectTransform.anchorMin = new Vector2(0.5f, 0f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0f);
                    rectTransform.pivot = new Vector2(0.5f, 0f);
                    rectTransform.anchoredPosition = new Vector2(0f, margin.y);
                    break;
            }
        }
    }
}
