using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResonitePackageExporter.Resonite
{
    public interface IRecord
    {
        string RecordId { get; set; }

        string OwnerId { get; set; }

        string AssetURI { get; set; }

        [JsonIgnore]
        RecordId CombinedRecordId { get; }

        RecordVersion Version { get; set; }

        string Name { get; set; }

        string OwnerName { get; set; }

        string Description { get; set; }

        string RecordType { get; set; }

        HashSet<string> Tags { get; set; }

        string Path { get; set; }

        string ThumbnailURI { get; set; }

        bool IsPublic { get; set; }

        bool IsForPatrons { get; set; }

        bool IsListed { get; set; }

        bool IsDeleted { get; set; }

        int Visits { get; set; }

        double Rating { get; set; }

        int RandomOrder { get; set; }

        MigrationMetadata MigrationMetadata { get; set; }

        DateTime? FirstPublishTime { get; set; }

        DateTime? CreationTime { get; set; }

        DateTime LastModificationTime { get; set; }

        List<DBAsset> AssetManifest { get; set; }

        R Clone<R>() where R : class, IRecord, new();

        void IncrementGlobalVersion();

        void OverrideGlobalVersion(int globalVersion);

        void IncrementLocalVersion(string machineId, string userId);
    }
}
