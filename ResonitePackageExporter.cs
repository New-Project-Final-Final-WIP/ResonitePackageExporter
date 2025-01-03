using BaseX;
using HarmonyLib;
using FrooxEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using CloudX.Shared;

namespace ResonitePackageExporter
{
    public static class ResonitePackageExporter
    {
        public static Harmony harmony;

        internal static string version = "0.1.5";
        public static bool ToggleNewtonsoft = false;
        public static void Initialize()
        {
            // Print initialization
            Logger.Log($"Initializing ResonitePackageExplorer v{version}");
            Logger.Log($"Using {typeof(System.Text.Json.JsonSerializer).Assembly.FullName}");
            Logger.Log($"Using {typeof(Newtonsoft.Json.JsonSerializer).Assembly.FullName}");

            harmony = new("Neos.ResonitePackageImporter");
            Logger.Log($"Using {typeof(Harmony).Assembly.FullName}");

            Logger.Log("Patching Methods");

            
            var export = typeof(FileBrowser).GetMethod("CreateNew", BindingFlags.NonPublic | BindingFlags.Instance);
            var exportSetup = typeof(ExportDialog).GetMethod(nameof(ExportDialog.Setup), BindingFlags.Public | BindingFlags.Instance);

            var exportPrefix = typeof(ResonitePackageExporter).GetMethod(nameof(InjectPackageExportable), BindingFlags.Public | BindingFlags.Static);
            var sortExportPatch = typeof(ResonitePackageExporter).GetMethod(nameof(SortExportables), BindingFlags.Public | BindingFlags.Static);

            // Patch methods
            harmony.Patch(export, prefix: new(exportPrefix));
            harmony.Patch(exportSetup, prefix: new(sortExportPatch));

            Engine.Current.RunPostInit(()=>
            {
                Logger.Log($"CloudXInterface UseNewtonsoftJson: {CloudXInterface.UseNewtonsoftJson}");
                AddCreateNewActions();
            });
        }

        static void AddCreateNewActions()
        {
            // Add option to switch to using newton soft or system.text.json
            DevCreateNewForm.AddAction("ResonitePackage Tools", "Toggle NewtonsoftJson", s =>
            {
                ToggleNewtonsoft = !ToggleNewtonsoft;
                Logger.Log($"Switched to using: {(CloudXInterface.UseNewtonsoftJson ^ ToggleNewtonsoft ? "NewtonsoftJson": "System.Text.Json")}");

                DevCreateNewForm.SpawnText(s);
                var text = s.GetComponent<TextRenderer>();
                text.Text.Value = $"Use NewtonsoftJson: {CloudXInterface.UseNewtonsoftJson ^ ToggleNewtonsoft}";

            });
            // Directly export from the create new menu
            DevCreateNewForm.AddAction("ResonitePackage Tools", "Export World", s =>
            {
                s.StartTask(async () =>
                {
                    s.PositionInFrontOfUser(float3.Backward);

                    var fileBrowser = Userspace.Current.World.RootSlot.GetComponentInChildren<FileBrowser>();
                    string path = fileBrowser?.CurrentPath?.Value;

                    if (string.IsNullOrWhiteSpace(path))
                    {
                        var variableResult = await s.Engine.LocalDB.TryReadVariableAsync<string>("FileBrowser.CurrentPath");
                        if (!variableResult.hasValue)
                        {
                            DevCreateNewForm.SpawnText(s);
                            var text = s.GetComponent<TextRenderer>();
                            text.Text.Value = "No file path found, Please select a directory with the File Browser";
                            return;
                        }

                        path = variableResult.value;
                    }

                    var exportDialog = s.AttachComponent<ExportDialog>();

                    var packageExportable = exportDialog.Slot.AttachComponent<PackageExportable>();
                    packageExportable.Root.Target = s.World.RootSlot;

                    exportDialog.Setup(path, [packageExportable]);

                });
            });
        }

        /*[HarmonyPrefix]
        [HarmonyPatch(typeof(FileBrowser), "CreateNew")]*/
        // This is to inject the PackageExportable component before CreateNew method copies Exportables to the Export Dialog
        public static void InjectPackageExportable(FileBrowser __instance, IButton button, ButtonEventData eventData)
        {

            if (!__instance.CanInteract(__instance.LocalUser))
                return;

            Grabber grabber = __instance.World.GetLocalUserGrabberWithItems(eventData.source.Slot);
            
            grabber?.World.RunSynchronously(() =>
                {
                    Slot target = grabber.HolderSlot[0]; // In resonite the export is the first object


                    // Exportable Target is held reference
                    ReferenceProxy componentInChildren = target.GetComponentInChildren<ReferenceProxy>(r => r.Reference.Target is Slot);
                    if (componentInChildren != null)
                        target = (Slot)componentInChildren.Reference.Target;

                    if ((!target.ForeachComponentInChildren<IItemPermissions>(p => p.CanSave) ? 0 : (grabber.World.CanSaveItems() ? 1 : 0)) == 0)
                        return;

                    // Has IExportable
                    List<IExportable> componentsInChildren = target.GetComponentsInChildren<IExportable>();

                    // Add PackageExportable and or ModelExportable components for export menu if they don't exist
                    // Always add a new package exportable if slot is root
                    if (!componentsInChildren.Any(e => e is PackageExportable) || target == target.World.RootSlot)
                    {
                        List<IExportable> cleanup = [];

                        // If no IExportable components exist add a ModelExportable if not root
                        if (componentsInChildren.Count == 0 && target != target.World.RootSlot)
                        {
                            ModelExportable modelExportable = target.AttachComponent<ModelExportable>();
                            modelExportable.Persistent = false;
                            modelExportable.Root.Target = target;
                            cleanup.Add(modelExportable);
                        }
                        
                        // Add PackageExportable component for slot
                        PackageExportable packageExportable = target.AttachComponent<PackageExportable>();
                        packageExportable.Persistent = false;
                        packageExportable.Root.Target = target;
                        cleanup.Add(packageExportable);

                        // Cleanup components after updates
                        // I don't remember if 2 updates is necessary
                        target.RunInUpdates(2, () =>
                        {
                            foreach (var component in cleanup)
                            {
                                component.Destroy();
                            }
                        });
                    }
                });
        }


        /*[HarmonyPrefix]
[HarmonyPatch(typeof(ExportDialog), nameof(ExportDialog.Setup))]*/
        public static void SortExportables(ref IExportable[] exportables)
        {
            // Put package exportables at the front of the list
            var newlist = new List<IExportable>();

            //var rootSlotList = new List<IExportable>();
            foreach (var exportable in exportables)
            {
                if (exportable is PackageExportable /*packageExportable*/)
                {
                   /* // Lazy setup to only include root slot exports
                    if (packageExportable.Root.Target == packageExportable.World.RootSlot)
                    {
                        rootSlotList.Add(exportable);
                    } else
                    {*/
                        newlist.Insert(0, exportable);
                    //}

                    continue;
                }

                newlist.Add(exportable);
            }
            /*
            if (rootSlotList.Count > 0)
            {
                exportables = [.. rootSlotList];
                return;
            }*/

            exportables = [.. newlist];
        }
    }
}
