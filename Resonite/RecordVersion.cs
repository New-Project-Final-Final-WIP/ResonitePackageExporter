using System;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ResonitePackageExporter.Resonite
{
    [Serializable]
    public struct RecordVersion(
      int globalVersion,
      int localVersion,
      string lastModifyingUserId,
      string lastModifyingMachineId)
    {
        [JsonProperty(PropertyName = "globalVersion")]
        [JsonPropertyName("globalVersion")]
        public int GlobalVersion { get; set; } = globalVersion;

        [JsonProperty(PropertyName = "localVersion")]
        [JsonPropertyName("localVersion")]
        public int LocalVersion { get; set; } = localVersion;

        [JsonProperty(PropertyName = "lastModifyingUserId")]
        [JsonPropertyName("lastModifyingUserId")]
        public string LastModifyingUserId { get; set; } = lastModifyingUserId;

        [JsonProperty(PropertyName = "lastModifyingMachineId")]
        [JsonPropertyName("lastModifyingMachineId")]
        public string LastModifyingMachineId { get; set; } = lastModifyingMachineId;
    }
}