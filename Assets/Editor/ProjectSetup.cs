using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace LuckyCharm.Editor
{
    public static class ProjectSetup
    {
        [MenuItem("LuckyCharm/Setup Project (First Time)", false, 0)]
        public static void SetupProject()
        {
            if (!EditorUtility.DisplayDialog(
                "Project Setup",
                "This will set up the LuckyCharm project:\n\n" +
                "1. Create localization assets and string tables\n" +
                "2. Wire up scene references\n" +
                "3. Save the scene\n\n" +
                "Continue?",
                "Setup",
                "Cancel"))
            {
                return;
            }

            UnityEngine.Debug.Log("=== Starting LuckyCharm Project Setup ===");

            // Step 1: Setup Localization
            UnityEngine.Debug.Log("[1/3] Setting up localization...");
            try
            {
                LocalizationSetup.Setup();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"Localization setup failed: {e.Message}");
            }

            // Step 2: Wire up scene references
            UnityEngine.Debug.Log("[2/3] Wiring up scene references...");
            try
            {
                SceneWireup.WireUpSceneReferences();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"Scene wireup failed: {e.Message}");
            }

            // Step 3: Save scene
            UnityEngine.Debug.Log("[3/3] Saving scene...");
            EditorSceneManager.SaveOpenScenes();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UnityEngine.Debug.Log("=== LuckyCharm Project Setup Complete ===");

            EditorUtility.DisplayDialog(
                "Setup Complete",
                "Project setup finished!\n\n" +
                "You can now enter Play mode to test the app.",
                "OK");
        }

        [MenuItem("LuckyCharm/Setup Project (First Time)", true)]
        private static bool ValidateSetupProject()
        {
            // Only enable if MainScene is loaded
            var scene = EditorSceneManager.GetActiveScene();
            return scene.name == "MainScene";
        }
    }
}
