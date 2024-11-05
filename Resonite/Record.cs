using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json.Serialization;

namespace ResonitePackageExporter.Resonite
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [Serializable]
    public class Record : IRecord
    {
        [JsonProperty(PropertyName = "id")]
        [JsonPropertyName("id")]
        public string RecordId { get; set; }

        [JsonProperty(PropertyName = "ownerId")]
        [JsonPropertyName("ownerId")]
        public string OwnerId { get; set; }

        [JsonProperty(PropertyName = "assetUri")]
        [JsonPropertyName("assetUri")]
        public string AssetURI { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public RecordId CombinedRecordId => new(OwnerId, RecordId);

        [JsonProperty(PropertyName = "version")]
        [JsonPropertyName("version")]
        public RecordVersion Version { get; set; }

        [JsonProperty(PropertyName = "name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "recordType")]
        [JsonPropertyName("recordType")]
        public string RecordType { get; set; }

        [JsonProperty(PropertyName = "ownerName")]
        [JsonPropertyName("ownerName")]
        public string OwnerName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "tags")]
        [JsonPropertyName("tags")]
        public HashSet<string> Tags { get; set; }

        [JsonProperty(PropertyName = "path")]
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "thumbnailUri")]
        [JsonPropertyName("thumbnailUri")]
        public string ThumbnailURI { get; set; }

        [JsonProperty(PropertyName = "lastModificationTime")]
        [JsonPropertyName("lastModificationTime")]
        public DateTime LastModificationTime { get; set; }

        [JsonProperty(PropertyName = "creationTime")]
        [JsonPropertyName("creationTime")]
        public DateTime? CreationTime { get; set; }

        [JsonProperty(PropertyName = "firstPublishTime")]
        [JsonPropertyName("firstPublishTime")]
        public DateTime? FirstPublishTime { get; set; }

        [JsonProperty(PropertyName = "isDeleted")]
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty(PropertyName = "isPublic")]
        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; }

        [JsonProperty(PropertyName = "isForPatrons")]
        [JsonPropertyName("isForPatrons")]
        public bool IsForPatrons { get; set; }

        [JsonProperty(PropertyName = "isListed")]
        [JsonPropertyName("isListed")]
        public bool IsListed { get; set; }

        [JsonProperty(PropertyName = "visits")]
        [JsonPropertyName("visits")]
        public int Visits { get; set; }

        [JsonProperty(PropertyName = "rating")]
        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonProperty(PropertyName = "randomOrder")]
        [JsonPropertyName("randomOrder")]
        public int RandomOrder { get; set; }

        [JsonProperty(PropertyName = "submissions")]
        [JsonPropertyName("submissions")]
        public List<Submission> Submissions { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public List<string> Manifest { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "assetManifest")]
        [JsonPropertyName("assetManifest")]
        public List<DBAsset> AssetManifest { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "migrationMetadata")]
        [JsonPropertyName("migrationMetadata")]
        public MigrationMetadata MigrationMetadata { get; set; }

        public static bool IsValidId(string recordId) => recordId.StartsWith("R-");

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public bool IsValidOwnerId => CloudX.Shared.IdUtil.GetOwnerType(OwnerId) != CloudX.Shared.OwnerType.INVALID;

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public bool IsValidRecordId => RecordHelper.IsValidRecordID(RecordId);

        public void ResetVersioning() => Version = new RecordVersion();

        public void OverrideGlobalVersion(int globalVersion)
        {
            if (globalVersion < Version.GlobalVersion)
                throw new InvalidOperationException(string.Format("GlobalVersion cannot be set to a lower value than it already is. Current: {0}, target: {1}", (object)Version.GlobalVersion, (object)globalVersion));
            Version = Version with
            {
                GlobalVersion = globalVersion
            };
        }

        public void IncrementGlobalVersion() => OverrideGlobalVersion(Version.GlobalVersion + 1);

        public void IncrementLocalVersion(string machineId, string userId)
        {
            RecordVersion version = Version;
            ++version.LocalVersion;
            version.LastModifyingMachineId = machineId;
            version.LastModifyingUserId = userId;
            Version = version;
        }

        public R Clone<R>() where R : class, IRecord, new() => System.Text.Json.JsonSerializer.Deserialize<R>(System.Text.Json.JsonSerializer.Serialize<Record>(this));

        public Record Clone()
        {
            using MemoryStream serializationStream = new();
            BinaryFormatter binaryFormatter = new();

            binaryFormatter.Serialize(serializationStream, this);
            serializationStream.Position = 0L;

            return (Record)binaryFormatter.Deserialize(serializationStream);
        }

        [Obsolete]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "NeosDBManifest")]
        [JsonPropertyName("NeosDBManifest")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [Newtonsoft.Json.JsonIgnore]
        public List<DBAsset> LegacyManifest
        {
            get => null;
            set
            {
                if (value == null)
                    return;
                if (AssetManifest == null)
                    AssetManifest = value;
                else
                    AssetManifest.AddRange(value);
            }
        }

        [Obsolete]
        [JsonProperty(PropertyName = "globalVersion")]
        [JsonPropertyName("globalVersion")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? LegacyGlobalVersion
        {
            set
            {
                if (!value.HasValue)
                    return;
                Version = Version with
                {
                    GlobalVersion = value.Value
                };
            }
        }

        [Obsolete]
        [JsonProperty(PropertyName = "localVersion")]
        [JsonPropertyName("localVersion")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? LegacyLocalVersion
        {
            set
            {
                if (!value.HasValue)
                    return;
                Version = Version with
                {
                    LocalVersion = value.Value
                };
            }
        }

        [Obsolete]
        [JsonProperty(PropertyName = "lastModifyingUserId")]
        [JsonPropertyName("lastModifyingUserId")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string LegacyLastModifyingUserId
        {
            set => Version = Version with
            {
                LastModifyingUserId = value
            };
        }

        [Obsolete]
        [JsonProperty(PropertyName = "lastModifyingMachineId")]
        [JsonPropertyName("lastModifyingMachineId")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string LegacyLastModifyingMachineId
        {
            set => Version = Version with
            {
                LastModifyingMachineId = value
            };
        }


        [Obsolete]
        [JsonPropertyName("RecordId")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string RecordId_Neos
        {
            set => RecordId = value;
        }
        public override string ToString() => string.Format("Record {0}:{1}, Name: {2}, Type: {3}, Path: {4}, Version: {5}, AssetURI: {6}, Deleted: {7}", OwnerId, RecordId, Name, RecordType, Path, Version, AssetURI, IsDeleted);
    }
}
