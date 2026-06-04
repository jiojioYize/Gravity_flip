using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GravityFlip.UI
{
    public sealed class GameFlowController : MonoBehaviour
    {
        public static GameFlowController Instance { get; private set; }

        public static bool IsGameplayBlocked =>
            Instance != null && (Instance.isPaused || Instance.isWinVisible);

        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string levelSceneName = "Level01";

        [Header("Instructions")]
        [TextArea(6, 12)]
        [SerializeField] private string instructionsText =
            "Move: A/D or Arrow keys\n" +
            "Jump: Space (only when grounded on floor or ceiling)\n" +
            "Flip gravity: Left Shift\n" +
            "Reset level: R\n" +
            "Pause: Esc\n\n" +
            "Combinations:\n" +
            "• Move + Jump: jump diagonally left or right.\n" +
            "• Flip + Move: steer left or right while rising or falling after a flip.\n" +
            "• Flip twice: press Shift again during a flip to return to your previous gravity.";

        private GameObject pauseRoot;
        private GameObject instructionsRoot;
        private GameObject winRoot;
        private bool isPaused;
        private bool isWinVisible;
        private bool showingInstructions;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            BuildOverlayUi();
            HideAllPanels();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
            }
        }

        private void Update()
        {
            if (isWinVisible)
            {
                return;
            }

            if (!Input.GetKeyDown(KeyCode.Escape))
            {
                return;
            }

            if (showingInstructions)
            {
                ShowPauseMenu();
                return;
            }

            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void ShowWinPanel()
        {
            HideAllPanels();
            isWinVisible = true;
            isPaused = false;
            showingInstructions = false;
            Time.timeScale = 1f;
            winRoot.SetActive(true);
        }

        public void Pause()
        {
            if (isWinVisible)
            {
                return;
            }

            isPaused = true;
            showingInstructions = false;
            Time.timeScale = 0f;
            pauseRoot.SetActive(true);
            instructionsRoot.SetActive(false);
            winRoot.SetActive(false);
        }

        public void Resume()
        {
            isPaused = false;
            showingInstructions = false;
            Time.timeScale = 1f;
            HideAllPanels();
        }

        public void ShowInstructions()
        {
            showingInstructions = true;
            pauseRoot.SetActive(false);
            instructionsRoot.SetActive(true);
        }

        public void ShowPauseMenu()
        {
            showingInstructions = false;
            pauseRoot.SetActive(true);
            instructionsRoot.SetActive(false);
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }

        public void ReloadLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(levelSceneName);
        }

        private void HideAllPanels()
        {
            if (pauseRoot != null)
            {
                pauseRoot.SetActive(false);
            }

            if (instructionsRoot != null)
            {
                instructionsRoot.SetActive(false);
            }

            if (winRoot != null)
            {
                winRoot.SetActive(false);
            }

            isPaused = false;
            isWinVisible = false;
            showingInstructions = false;
        }

        private void BuildOverlayUi()
        {
            OverlayUiBuilder.DestroyCanvasIfExists("GameFlowCanvas");
            OverlayUiBuilder.EnsureEventSystem();
            Canvas canvas = OverlayUiBuilder.CreateOverlayCanvas("GameFlowCanvas", 10);
            RectTransform canvasRoot = OverlayUiBuilder.CreateFullStretchRoot(canvas.transform, "FlowRoot");

            pauseRoot = BuildPausePanel(canvasRoot);
            instructionsRoot = BuildInstructionsPanel(canvasRoot);
            winRoot = BuildWinPanel(canvasRoot);
        }

        private GameObject BuildPausePanel(RectTransform parent)
        {
            RectTransform panelRoot = OverlayUiBuilder.CreateFullStretchRoot(parent, "PausePanel");
            OverlayUiBuilder.CreateDimPanel(panelRoot, "Dim", new Color(0f, 0f, 0f, 0.72f), blockRaycasts: true);
            OverlayUiBuilder.CreateHeaderText(panelRoot, "Title", "Paused", 32, 72f);

            RectTransform buttonArea = OverlayUiBuilder.CreateFullStretchRoot(panelRoot, "Buttons");
            buttonArea.offsetMin = new Vector2(40f, 100f);
            buttonArea.offsetMax = new Vector2(-40f, -100f);

            const float buttonHeight = 56f;
            OverlayUiBuilder.CreateMenuButton(buttonArea, "ResumeButton", "Resume", 0.72f, buttonHeight, Resume);
            OverlayUiBuilder.CreateMenuButton(buttonArea, "InstructionsButton", "Instructions", 0.5f, buttonHeight, ShowInstructions);
            OverlayUiBuilder.CreateMenuButton(buttonArea, "MainMenuButton", "Main menu", 0.28f, buttonHeight, LoadMainMenu);

            GameObject rootObject = panelRoot.gameObject;
            rootObject.SetActive(false);
            return rootObject;
        }

        private GameObject BuildInstructionsPanel(RectTransform parent)
        {
            RectTransform panelRoot = OverlayUiBuilder.CreateFullStretchRoot(parent, "InstructionsPanel");
            OverlayUiBuilder.CreateDimPanel(panelRoot, "Dim", new Color(0f, 0f, 0f, 0.72f), blockRaycasts: true);
            OverlayUiBuilder.CreateHeaderText(panelRoot, "Title", "Instructions", 28, 64f);

            OverlayUiBuilder.CreateText(panelRoot, "Body", instructionsText, 16, TextAnchor.UpperLeft);
            RectTransform bodyRect = panelRoot.Find("Body").GetComponent<RectTransform>();
            bodyRect.offsetMin = new Vector2(40f, 120f);
            bodyRect.offsetMax = new Vector2(-40f, -140f);

            OverlayUiBuilder.CreateMenuButton(panelRoot, "BackButton", "Back", 0.1f, 52f, ShowPauseMenu);

            GameObject rootObject = panelRoot.gameObject;
            rootObject.SetActive(false);
            return rootObject;
        }

        private GameObject BuildWinPanel(RectTransform parent)
        {
            RectTransform panelRoot = OverlayUiBuilder.CreateFullStretchRoot(parent, "WinPanel");
            OverlayUiBuilder.CreateDimPanel(panelRoot, "Dim", new Color(0f, 0f, 0f, 0.72f), blockRaycasts: true);
            OverlayUiBuilder.CreateHeaderText(panelRoot, "Title", "Level complete!", 32, 72f);

            RectTransform buttonArea = OverlayUiBuilder.CreateFullStretchRoot(panelRoot, "Buttons");
            buttonArea.offsetMin = new Vector2(40f, 100f);
            buttonArea.offsetMax = new Vector2(-40f, -100f);

            const float buttonHeight = 56f;
            OverlayUiBuilder.CreateMenuButton(buttonArea, "ReplayButton", "Play again", 0.58f, buttonHeight, ReloadLevel);
            OverlayUiBuilder.CreateMenuButton(buttonArea, "MenuButton", "Main menu", 0.38f, buttonHeight, LoadMainMenu);

            GameObject rootObject = panelRoot.gameObject;
            rootObject.SetActive(false);
            return rootObject;
        }
    }
}
