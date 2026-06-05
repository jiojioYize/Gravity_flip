using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GravityFlip.UI
{
    /// <summary>
    /// Builds the main menu UI when Play Mode starts. Edit mode shows only script fields in the Inspector.
    /// </summary>
    public sealed class MainMenuController : MonoBehaviour
    {
        private const string MenuCanvasName = "MainMenuCanvas";
        private const int CurrentMenuCopyVersion = 2;

        private const string DefaultTitleText = "Gravity Flip";
        private const string DefaultStoryText =
            "Flip gravity at will to walk on floors and ceilings—dodge hazards along the way.\n\n" +
            "Collect every key, then reach the exit.";

        [SerializeField] private string levelSceneName = "Level01";

        [Header("Copy")]
        [SerializeField] private int menuCopyVersion;
        [SerializeField] private string titleText = DefaultTitleText;
        [TextArea(3, 6)]
        [SerializeField] private string storyText = DefaultStoryText;

        private void Awake()
        {
            ApplyMenuCopyIfOutdated();
            OverlayUiBuilder.DestroyCanvasIfExists(MenuCanvasName);
            EnsureCamera();
            OverlayUiBuilder.EnsureEventSystem();
            BuildMenu();
        }

        private void ApplyMenuCopyIfOutdated()
        {
            if (menuCopyVersion >= CurrentMenuCopyVersion)
            {
                return;
            }

            titleText = DefaultTitleText;
            storyText = DefaultStoryText;
            menuCopyVersion = CurrentMenuCopyVersion;
        }

        public void StartGame()
        {
            Time.timeScale = 1f;

            string scenePath = $"Assets/Scenes/{levelSceneName}.unity";
            int buildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
            if (buildIndex < 0)
            {
                Debug.LogError(
                    $"Scene '{levelSceneName}' is not in Build Settings. Add Assets/Scenes/MainMenu.unity and Level01.unity via File → Build Settings.");
                return;
            }

            SceneManager.LoadScene(buildIndex);
        }

        private static void EnsureCamera()
        {
            if (Camera.main != null)
            {
                return;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.1f, 0.14f, 1f);
            cameraObject.AddComponent<AudioListener>();
        }

        private void BuildMenu()
        {
            Canvas canvas = OverlayUiBuilder.CreateOverlayCanvas(MenuCanvasName, 0);
            RectTransform root = OverlayUiBuilder.CreateFullStretchRoot(canvas.transform, "MainMenuRoot");
            OverlayUiBuilder.CreateDimPanel(root, "Background", new Color(0.06f, 0.08f, 0.12f, 1f));

            OverlayUiBuilder.CreateText(root, "Title", titleText, 36, TextAnchor.UpperCenter);
            RectTransform titleRect = root.Find("Title").GetComponent<RectTransform>();
            titleRect.offsetMin = new Vector2(24f, 0f);
            titleRect.offsetMax = new Vector2(-24f, -48f);

            OverlayUiBuilder.CreateText(root, "Story", storyText, 16, TextAnchor.MiddleCenter);
            RectTransform storyRect = root.Find("Story").GetComponent<RectTransform>();
            storyRect.offsetMin = new Vector2(32f, 120f);
            storyRect.offsetMax = new Vector2(-32f, -120f);

            OverlayUiBuilder.CreateMenuButton(root, "StartButton", "Start", 0.12f, 56f, StartGame);
        }
    }
}
