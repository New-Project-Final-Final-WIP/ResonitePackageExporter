using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace ResonitePackageExporter.Resonite
{
    [Serializable]
    public struct RecordVersion : IEquatable<RecordVersion>
    {
        [JsonProperty(PropertyName = "globalVersion")]
        [JsonPropertyName("globalVersion")]
        public int GlobalVersion { get; set; }

        [JsonProperty(PropertyName = "localVersion")]
        [JsonPropertyName("localVersion")]
        public int LocalVersion { get; set; }

        [JsonProperty(PropertyName = "lastModifyingUserId")]
        [JsonPropertyName("lastModifyingUserId")]
        public string LastModifyingUserId { get; set; }

        [JsonProperty(PropertyName = "lastModifyingMachineId")]
        [JsonPropertyName("lastModifyingMachineId")]
        public string LastModifyingMachineId { get; set; }

        public RecordVersion(
          int globalVersion,
          int localVersion,
          string lastModifyingUserId,
          string lastModifyingMachineId)
        {
            this.GlobalVersion = globalVersion;
            this.LocalVersion = localVersion;
            this.LastModifyingUserId = lastModifyingUserId;
            this.LastModifyingMachineId = lastModifyingMachineId;
        }

        public RecordVersion OverrideModifyingUserId(string userId) => new()
        {
            GlobalVersion = this.GlobalVersion,
            LocalVersion = this.LocalVersion,
            LastModifyingUserId = userId,
            LastModifyingMachineId = this.LastModifyingMachineId
        };

        public override string ToString() => string.Format("Global: {0}, Local: {1}, UserId: {2}, MachineId: {3}", (object)this.GlobalVersion, (object)this.LocalVersion, (object)this.LastModifyingUserId, (object)this.LastModifyingMachineId);

        public bool Equals(RecordVersion other) => this.GlobalVersion == other.GlobalVersion && this.LocalVersion == other.LocalVersion && this.LastModifyingUserId == other.LastModifyingUserId && this.LastModifyingMachineId == other.LastModifyingMachineId;

        public override int GetHashCode() => unchecked(((-690053944 * -1521134295 + this.GlobalVersion.GetHashCode()) * -1521134295 + this.LocalVersion.GetHashCode()) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.LastModifyingUserId)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.LastModifyingMachineId);
        public override bool Equals(object obj) => obj is RecordVersion other && this.Equals(other);

        public static bool operator ==(RecordVersion a, RecordVersion b) => a.Equals(b);

        public static bool operator !=(RecordVersion a, RecordVersion b) => !a.Equals(b);
    }
}