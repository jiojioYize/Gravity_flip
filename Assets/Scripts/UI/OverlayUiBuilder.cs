using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GravityFlip.UI
{
    /// <summary>
    /// Builds simple overlay UI (panels, labels, buttons) at runtime for menus and flow screens.
    /// </summary>
    internal static class OverlayUiBuilder
    {
        private static Font cachedFont;

        public static Font DefaultFont
        {
            get
            {
                if (cachedFont == null)
                {
                    cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }

                return cachedFont;
            }
        }

        /// <summary>
        /// Ensures a single EventSystem exists for UI clicks. Only runs in Play Mode.
        /// </summary>
        public static void EnsureEventSystem()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            EventSystem[] systems = UnityEngine.Object.FindObjectsOfType<EventSystem>(true);
            if (systems.Length > 0)
            {
                EventSystem system = systems[0];
                if (!system.gameObject.activeInHierarchy)
                {
                    system.gameObject.SetActive(true);
                }

                system.enabled = true;
                return;
            }

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        public static void DestroyCanvasIfExists(string canvasName)
        {
            GameObject existing = GameObject.Find(canvasName);
            if (existing == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(existing);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(existing);
            }
        }

        public static RectTransform CreateFullStretchRoot(Transform parent, string name)
        {
            GameObject rootObject = new GameObject(name, typeof(RectTransform));
            RectTransform rect = rootObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            StretchFull(rect);
            return rect;
        }

        public static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
        }

        public static Image CreateDimPanel(RectTransform parent, string name, Color color, bool blockRaycasts = false)
        {
            GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            RectTransform rect = panelObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            StretchFull(rect);

            Image image = panelObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = blockRaycasts;
            return image;
        }

        public static Text CreateText(RectTransform parent, string name, string content, int fontSize, TextAnchor anchor)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            StretchFull(rect);

            Text text = textObject.GetComponent<Text>();
            ApplyTextDefaults(text, content, fontSize, anchor);
            return text;
        }

        public static Text CreateHeaderText(RectTransform parent, string name, string content, int fontSize, float heightPx)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, heightPx);
            rect.anchoredPosition = Vector2.zero;

            Text text = textObject.GetComponent<Text>();
            ApplyTextDefaults(text, content, fontSize, TextAnchor.MiddleCenter);
            return text;
        }

        /// <summary>
        /// Menu button with fixed pixel height so labels stay visible at any Game view scale.
        /// </summary>
        public static Button CreateMenuButton(
            RectTransform parent,
            string name,
            string label,
            float centerY01,
            float heightPx,
            UnityAction onClick)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.1f, centerY01);
            rect.anchorMax = new Vector2(0.9f, centerY01);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, heightPx);
            rect.anchoredPosition = Vector2.zero;

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.22f, 0.28f, 0.38f, 0.98f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.SetParent(rect, false);
            StretchFull(labelRect);

            Text labelText = labelObject.GetComponent<Text>();
            int fontSize = Mathf.Clamp(Mathf.RoundToInt(heightPx * 0.42f), 18, 28);
            ApplyTextDefaults(labelText, label, fontSize, TextAnchor.MiddleCenter);

            return button;
        }

        private static void ApplyTextDefaults(Text text, string content, int fontSize, TextAnchor anchor)
        {
            text.font = DefaultFont;
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.resizeTextForBestFit = false;
            text.raycastTarget = false;
        }

        public static Canvas CreateOverlayCanvas(string canvasName, int sortingOrder)
        {
            GameObject canvasObject = new GameObject(canvasName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(600, 700);
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform rect = canvasObject.GetComponent<RectTransform>();
            StretchFull(rect);
            return canvas;
        }
    }
}
