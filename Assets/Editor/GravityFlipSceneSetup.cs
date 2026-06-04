#if UNITY_EDITOR
using System.IO;
using GravityFlip.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GravityFlip.Editor
{
    public static class GravityFlipSceneSetup
    {
        private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
        private const string LevelScenePath = "Assets/Scenes/Level01.unity";

        [MenuItem("Gravity Flip/Create or Fix Main Menu Scene", true)]
        public static bool ValidateCreateOrFixMainMenuScene()
        {
            return !EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode;
        }

        [MenuItem("Gravity Flip/Create or Fix Main Menu Scene")]
        public static void CreateOrFixMainMenuScene()
        {
            if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog(
                    "Stop Play Mode first",
                    "Create or Fix Main Menu Scene only works while the Editor is not in Play Mode.\n\n" +
                    "Click the Play button to stop, then run this menu item again.",
                    "OK");
                return;
            }

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            RemoveBrokenMainMenuAssetIfNeeded();

            Scene scene;
            if (TryOpenMainMenuScene(out scene))
            {
                FixMainMenuInScene(scene);
                EditorSceneManager.SaveScene(scene);
            }
            else
            {
                scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                CreateMainMenuRoot();
                EditorSceneManager.SaveScene(scene, MainMenuScenePath);
            }

            AssetDatabase.ImportAsset(MainMenuScenePath, ImportAssetOptions.ForceUpdate);
            EnsureBuildSettings();
            AssetDatabase.Refresh();
            Debug.Log("MainMenu scene ready at " + MainMenuScenePath + ". Open it from Assets/Scenes/MainMenu.unity.");
        }

        [MenuItem("Gravity Flip/Add Scenes To Build Settings")]
        public static void EnsureBuildSettingsMenu()
        {
            EnsureBuildSettings();
            Debug.Log("Build settings: MainMenu (0), Level01 (1).");
        }

        private static bool TryOpenMainMenuScene(out Scene scene)
        {
            scene = default;
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(MainMenuScenePath);
            if (sceneAsset == null)
            {
                return false;
            }

            try
            {
                scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
                return scene.IsValid();
            }
            catch (System.ArgumentException ex)
            {
                Debug.LogWarning("MainMenu scene could not be opened; recreating. " + ex.Message);
                return false;
            }
        }

        private static void RemoveBrokenMainMenuAssetIfNeeded()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(MainMenuScenePath) != null)
            {
                return;
            }

            if (AssetDatabase.LoadMainAssetAtPath(MainMenuScenePath) != null)
            {
                AssetDatabase.DeleteAsset(MainMenuScenePath);
                AssetDatabase.Refresh();
                return;
            }

            if (!File.Exists(MainMenuScenePath))
            {
                return;
            }

            string metaPath = MainMenuScenePath + ".meta";
            File.Delete(MainMenuScenePath);
            if (File.Exists(metaPath))
            {
                File.Delete(metaPath);
            }

            AssetDatabase.Refresh();
            Debug.Log("Removed broken MainMenu scene files that Unity could not import.");
        }

        private static void CreateMainMenuRoot()
        {
            GameObject root = new GameObject("MainMenu");
            root.AddComponent<MainMenuController>();
        }

        private static void FixMainMenuInScene(Scene scene)
        {
            GameObject root = FindMainMenuRoot(scene);
            if (root == null)
            {
                root = new GameObject("MainMenu");
            }

            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(root);

            if (root.GetComponent<MainMenuController>() == null)
            {
                root.AddComponent<MainMenuController>();
            }

            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static GameObject FindMainMenuRoot(Scene scene)
        {
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                if (roots[i].name == "MainMenu")
                {
                    return roots[i];
                }
            }

            MainMenuController existing = Object.FindObjectOfType<MainMenuController>();
            return existing != null ? existing.gameObject : null;
        }

        private static void EnsureBuildSettings()
        {
            string mainMenuGuid = AssetDatabase.AssetPathToGUID(MainMenuScenePath);
            string levelGuid = AssetDatabase.AssetPathToGUID(LevelScenePath);

            if (string.IsNullOrEmpty(mainMenuGuid) || string.IsNullOrEmpty(levelGuid))
            {
                EditorBuildSettings.scenes = new[]
                {
                    new EditorBuildSettingsScene(MainMenuScenePath, true),
                    new EditorBuildSettingsScene(LevelScenePath, true)
                };
                return;
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(LevelScenePath, true)
            };
        }
    }
}
#endif
