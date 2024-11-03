using BaseX;

namespace ResonitePackageExporter
{
    internal class Logger
    {
        internal static void Log(string message) => UniLog.Log($"[ResonitePackageExporter]  {message}");
        internal static void Log(object obj) => Log(obj.ToString());
     
        internal static void Error(string message) => UniLog.Error($"[ResonitePackageExporter]  {message}");
        internal static void Error(object obj) => Error(obj.ToString());

        internal static void Warning(string message) => UniLog.Warning($"[ResonitePackageExporter]  {message}");
        internal static void Warning(object obj) => Warning(obj.ToString());
    }
}