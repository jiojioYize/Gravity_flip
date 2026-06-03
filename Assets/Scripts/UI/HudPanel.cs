using UnityEngine;
using UnityEngine.UI;

namespace GravityFlip.UI
{
    /// <summary>
    /// One HUD block: optional background Image + Text label. Swap the Image sprite for art later without changing GameplayHUD.
    /// </summary>
    public sealed class HudPanel : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Text valueText;

        public Text ValueText => valueText;

        public void SetText(string text)
        {
            if (valueText != null)
            {
                valueText.text = text;
            }
        }

        public void SetBackgroundSprite(Sprite sprite)
        {
            if (backgroundImage == null)
            {
                return;
            }

            backgroundImage.sprite = sprite;
            backgroundImage.enabled = sprite != null;
        }

        public void SetBackgroundVisible(bool isVisible)
        {
            if (backgroundImage != null)
            {
                backgroundImage.enabled = isVisible;
            }
        }
    }
}
