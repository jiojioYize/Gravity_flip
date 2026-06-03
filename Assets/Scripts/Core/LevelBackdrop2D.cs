using UnityEngine;

namespace GravityFlip.Core
{
    /// <summary>
    /// Scales a sprite backdrop to a world-axis rectangle behind gameplay art.
    /// Hides empty letterboxing above/below/beyond finite ground and ceiling strips.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class LevelBackdrop2D : MonoBehaviour
    {
        [SerializeField] private float minX = -22f;
        [SerializeField] private float maxX = 22f;
        [SerializeField] private float minY = -12f;
        [SerializeField] private float maxY = 7f;
        [SerializeField] private Color backdropColor = new Color(0.08f, 0.1f, 0.14f, 1f);
        [SerializeField] private int sortingOrder = -100;
        [SerializeField] private float depthZ = 5f;

        private SpriteRenderer backdropRenderer;

        private void Awake()
        {
            backdropRenderer = GetComponent<SpriteRenderer>();
            backdropRenderer.sortingOrder = sortingOrder;
            ApplyBounds();
        }

        private void OnValidate()
        {
            if (backdropRenderer == null)
            {
                backdropRenderer = GetComponent<SpriteRenderer>();
            }

            if (backdropRenderer != null)
            {
                ApplyBounds();
            }
        }

        public void ApplyBounds()
        {
            if (backdropRenderer == null)
            {
                return;
            }

            float width = Mathf.Max(0.1f, maxX - minX);
            float height = Mathf.Max(0.1f, maxY - minY);

            transform.position = new Vector3(
                (minX + maxX) * 0.5f,
                (minY + maxY) * 0.5f,
                depthZ);
            transform.localScale = new Vector3(width, height, 1f);
            backdropRenderer.color = backdropColor;
            backdropRenderer.sortingOrder = sortingOrder;
        }
    }
}
