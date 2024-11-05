using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace ResonitePackageExporter.Resonite
{
    public class MigrationMetadata
    {
        [JsonProperty(PropertyName = "migrationId")]
        [JsonPropertyName("migrationId")]
        public string MigrationId { get; set; }

        [JsonProperty(PropertyName = "migrationSource")]
        [JsonPropertyName("migrationSource")]
        public string MigrationSource { get; set; }

        [JsonProperty(PropertyName = "migratedOn")]
        [JsonPropertyName("migratedOn")]
        public DateTime MigratedOn { get; set; }

        [JsonProperty(PropertyName = "sourceVersion")]
        [JsonPropertyName("sourceVersion")]
        public RecordVersion SourceVersion { get; set; }

        [JsonProperty(PropertyName = "targetVersion")]
        [JsonPropertyName("targetVersion")]
        public RecordVersion? TargetVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "assetManifest")]
        [JsonPropertyName("assetManifest")]
        public List<DBAsset> AssetManifest { get; set; }

        [JsonProperty(PropertyName = "previousMigration")]
        [JsonPropertyName("previousMigration")]
        public MigrationMetadata PreviousMigration { get; set; }
    }
}