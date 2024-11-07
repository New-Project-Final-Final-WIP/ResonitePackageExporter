using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResonitePackageExporter.Resonite
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [Serializable]
    public class Record
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


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "assetManifest")]
        [JsonPropertyName("assetManifest")]
        public List<DBAsset> AssetManifest { get; set; }

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
