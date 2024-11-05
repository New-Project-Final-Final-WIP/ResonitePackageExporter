using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace ResonitePackageExporter.Resonite
{
    [Serializable]
    public struct RecordVersion(
      int globalVersion,
      int localVersion,
      string lastModifyingUserId,
      string lastModifyingMachineId) : IEquatable<RecordVersion>
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

        public RecordVersion OverrideModifyingUserId(string userId) => new()
        {
            LastModifyingUserId = userId,
        };

        public override string ToString() => string.Format("Global: {0}, Local: {1}, UserId: {2}, MachineId: {3}", GlobalVersion, LocalVersion, LastModifyingUserId, LastModifyingMachineId);

        public bool Equals(RecordVersion other) => GlobalVersion == other.GlobalVersion
                                                   && LocalVersion == other.LocalVersion
                                                   && LastModifyingUserId == other.LastModifyingUserId
                                                   && LastModifyingMachineId == other.LastModifyingMachineId;

        public override int GetHashCode() => unchecked(((-690053944 * -1521134295 + GlobalVersion.GetHashCode()) * -1521134295 + LocalVersion.GetHashCode()) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LastModifyingUserId)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LastModifyingMachineId);
        public override bool Equals(object obj) => obj is RecordVersion other && Equals(other);

        public static bool operator ==(RecordVersion a, RecordVersion b) => a.Equals(b);

        public static bool operator !=(RecordVersion a, RecordVersion b) => !a.Equals(b);
    }
}