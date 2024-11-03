
using System;
using FrooxEngine;

namespace ResonitePackageExporter
{
    [ImplementableClass(true)]
    internal class ExecutionHook
    {
#pragma warning disable CS0169
        // fields must exist due to reflective access
        private static Type? __connectorType; // needed in all Neos versions
        private static Type? __connectorTypes; // needed in Neos 2021.10.17.1326 and later
#pragma warning restore CS0169

        static ExecutionHook()
        {
            try
            {
                if (!HarmonyLoader.LoadHarmony())
                {
                    Logger.Error("Harmony not found, Please put 0Harmony.dll in the Libraries folder");
                    return;
                }

                ResonitePackageExporter.Initialize();

                /*Engine.Current.OnReady += () =>
                {
                    ResonitePackageExporter.OnEngineReady();
                };*/
            }
            catch (Exception e)
            {
                Logger.Log($"Thrown Exception \n{e}");
            }
        }

        // implementation not strictly required, but method must exist due to reflective access
        private static DummyConnector InstantiateConnector() => new();

        private class DummyConnector : IConnector
        {
            public IImplementable? Owner { get; private set; }
            public void ApplyChanges() { }
            public void AssignOwner(IImplementable owner) => Owner = owner;
            public void Destroy(bool destroyingWorld) { }
            public void Initialize() { }
            public void RemoveOwner() => Owner = null;
        }
    }
}