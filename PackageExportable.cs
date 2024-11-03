using System;
using FrooxEngine;


using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Record = CloudX.Shared.Record;

namespace ResonitePackageExporter;

// Implement PackageExportable component from Resonite
[Category(new string[] { "Assets/Export" })]
public class PackageExportable : Component, IExportable, IComponent, IComponentBase, IDestroyable, IWorker, IWorldElement, IUpdatable, IChangeable, IAudioUpdatable, IInitializable, ILinkable
{

    public readonly SyncRef<Slot> Root;
    public string ExportName => Root.Target?.Name;
    public int ExportTypeCount => Root.Target == World.RootSlot ? 4 : 2;

    public IEnumerable<Slot> ExportRoots
    {
        get
        {
            if (Root.Target != null)
                yield return Root.Target;
        }
    }

    public async Task<bool> Export(string folder, string name, int exportType)
    {
        PackageExportable packageExportable = this;
        if (packageExportable.Root.Target == null)
            return false;

        // 1 and 3 are include variants options
        bool includeVariants = exportType % 2 == 1;
        SavedGraph savedGraph;
        
        // 2 and 3 are World export, this should only show if Root slot is held
        if (exportType >= 2 )
        {
            // Optimize world for export
            int mats = MaterialOptimizer.DeduplicateMaterials(World);
            int staticProviders = WorldOptimizer.DeduplicateStaticProviders(World);
            int assets = WorldOptimizer.CleanupAssets(World, true, WorldOptimizer.CleanupMode.MarkNonpersistent);
            
            Logger.Log(string.Format("World Optimized! Deduplicated Materials: {0}, Deduplicated Static Providers: {1}, Cleaned Up Assets: {2}", mats, staticProviders, assets));
            
            // Save the world
            savedGraph = World.SaveWorld();
        } else
        {
            // Save the object
            savedGraph = packageExportable.Root.Target.SaveObject(DependencyHandling.CollectAssets);
        }

        // Create record and build the package
        Record record = RecordHelper.CreateForObject<Record>(packageExportable.ExportName, packageExportable.LocalUser.UserID ?? packageExportable.LocalUser.MachineID, null);
        await new ToBackground();

        using FileStream fstream = File.OpenWrite(Path.Combine(folder, Path.ChangeExtension(name, ".resonitepackage")));

        await PackageCreator.BuildPackage(packageExportable.Engine, record, savedGraph, fstream, includeVariants);
        return true;
    }

    public string GetExportDescription(int index)
    {
        if (index == 0)
            return "Resonite Package";
        if (index == 1)
            return "Resonite Package + Variants";
        if (index == 2)
            return "World Resonite Package";
        if (index == 3)
            return "World Resonite Package + Variants";
        throw new ArgumentOutOfRangeException(nameof(index));
    }

    // File extension is always ResonitePackage
    public string GetExportExtension(int index) => ".ResonitePackage";

}