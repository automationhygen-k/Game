#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace OpenWorldRealisticMobileRacer.EditorTools
{
    public static class CliAndroidBuilder
    {
        public static void BuildAndroidApk()
        {
            string outputPath = Environment.GetCommandLineArgs() is var args
                ? GetArg(args, "-customBuildPath", "build/local-apk/OpenWorldRealisticMobileRacer.apk")
                : "build/local-apk/OpenWorldRealisticMobileRacer.apk";

            string[] scenes = GetEnabledScenes();
            if (scenes.Length == 0)
            {
                throw new Exception("No enabled scenes in Build Settings.");
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

            Console.WriteLine($"APK build succeeded: {outputPath}");
        }

        private static string[] GetEnabledScenes()
        {
            return Array.FindAll(EditorBuildSettings.scenes, s => s.enabled) is var scenes
                ? Array.ConvertAll(scenes, s => s.path)
                : Array.Empty<string>();
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
