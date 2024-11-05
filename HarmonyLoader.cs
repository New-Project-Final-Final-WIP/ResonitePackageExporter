using System;
using System.IO;
using System.Reflection;

namespace ResonitePackageExporter
{
    internal static class HarmonyLoader
    {
        internal static Assembly? LoadAssembly(string filepath)
        {
            //string filename = Path.GetFileName(filepath);

            Assembly assembly;
            try
            {
                //Logger.Log($"load assembly {filename}");
                assembly = Assembly.LoadFrom(filepath);
            }
            catch (Exception e)
            {
                Logger.Error($"error loading assembly from {filepath}: {e}");
                return null;
            }
            if (assembly == null)
            {
                Logger.Error($"unexpected null loading assembly from {filepath}");
                return null;
            }
            return assembly;
        }

        internal static bool LoadHarmony()
        {
            string[] paths_to_search = ["", "Libraries", "nml_libs"];
            string found_harmony = null;

            Logger.Log("Loading HarmonyLib");
            foreach (string dirName in paths_to_search)
            {
                string directory = Path.Combine(PlatformHelper.MainDirectory, dirName);
                string file = Path.Combine(directory, "0Harmony.dll");
                //Logger.Log(file);
                if (File.Exists(file))
                {
                    Logger.Log($"Found HarmonyLib at {file}");
                    found_harmony = file;
                    break;
                }
            }

            if (found_harmony == null) return false;

            if (LoadAssembly(found_harmony) != null)
            {
                Logger.Log("Loaded HarmonyLib");
                return true;
            };

            return false;
        }
    }


    // Provides helper functions for platform-specific operations.
    // Used for cases such as file handling which can vary between platforms.
    internal class PlatformHelper
    {
        public static readonly string AndroidNeosPath = "/sdcard/ModData/com.Solirax.Neos";

        // Android does not support Directory.GetCurrentDirectory(), so will fall back to the root '/' directory.
        public static bool UseFallbackPath() => Directory.GetCurrentDirectory().Replace('\\', '/') == "/" && !Directory.Exists("/Neos_Data");
        public static bool IsPathEmbedded(string path) => path.StartsWith("/data/app/com.Solirax.Neos");

        public static string MainDirectory
        {
            get { return UseFallbackPath() ? AndroidNeosPath : Directory.GetCurrentDirectory(); }
        }
    }
}