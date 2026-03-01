#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenWorldRealisticMobileRacer.EditorTools
{
    public static class CliAndroidBuilder
    {
        public static void BuildAndroidApk()
        {
            string[] args = Environment.GetCommandLineArgs();
            string outputPath = GetArg(args, "-customBuildPath", "build/local-apk/OpenWorldRealisticMobileRacer.apk");

            string[] scenes = ResolveBuildScenes();
            if (scenes.Length == 0)
            {
                throw new Exception("Unable to resolve scenes for build.");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? "build/local-apk");
            EditorUserBuildSettings.buildAppBundle = false;

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new Exception($"Android APK build failed: {report.summary.result}");
            }

            Debug.Log($"APK build succeeded: {outputPath}");
        }

        private static string[] ResolveBuildScenes()
        {
            string[] enabledBuildScenes = EditorBuildSettings.scenes
                .Where(s => s != null && s.enabled && !string.IsNullOrWhiteSpace(s.path))
                .Select(s => s.path)
                .Distinct()
                .ToArray();

            if (enabledBuildScenes.Length > 0)
            {
                return enabledBuildScenes;
            }

            string[] projectScenes = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes", "Assets" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => p.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .ToArray();

            if (projectScenes.Length > 0)
            {
                EditorBuildSettings.scenes = projectScenes.Select(p => new EditorBuildSettingsScene(p, true)).ToArray();
                return projectScenes;
            }

            string autoScene = CreateAutoBootstrapScene();
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(autoScene, true) };
            return new[] { autoScene };
        }

        private static string CreateAutoBootstrapScene()
        {
            const string scenePath = "Assets/Scenes/AutoBootstrap.unity";
            Directory.CreateDirectory("Assets/Scenes");

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            GameObject bootstrap = new GameObject("AutoBootstrap");
            bootstrap.tag = "Untagged";

            var worldBootstrapType = Type.GetType("OpenWorldRealisticMobileRacer.World.WorldBootstrap, Assembly-CSharp");
            if (worldBootstrapType != null)
            {
                bootstrap.AddComponent(worldBootstrapType);
            }

            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.Refresh();
            return scenePath;
        }

        private static string GetArg(string[] args, string key, string fallback)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return fallback;
        }
    }
}
#endif
