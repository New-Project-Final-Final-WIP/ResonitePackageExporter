using Newtonsoft.Json;
using System;

using System.Text.Json.Serialization;


namespace ResonitePackageExporter.Resonite
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [Serializable]
    public class RecordId : IEquatable<RecordId>
    {
        [JsonProperty(PropertyName = "recordId")]
        [JsonPropertyName("recordId")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "ownerId")]
        [JsonPropertyName("ownerId")]
        public string OwnerId { get; set; }

        public override bool Equals(object obj)
        {
            RecordId other = obj as RecordId;
            return other != null && Equals(other);
        }

        public override int GetHashCode() => Id.GetHashCode() ^ OwnerId.GetHashCode();

        public bool Equals(RecordId other) => Id == other?.Id && OwnerId == other?.OwnerId;

        public bool Equals(string other)
        {
            if (string.IsNullOrEmpty(other))
                return false;
            int num = OwnerId.Length + 1 + Id.Length;
            return other.Length == num && other.StartsWith(OwnerId, StringComparison.InvariantCultureIgnoreCase) && other[OwnerId.Length] == ':' && other.EndsWith(Id, StringComparison.InvariantCultureIgnoreCase);
        }

        public RecordId(string ownerId, string recordId)
        {
            OwnerId = ownerId;
            Id = recordId;
        }

        public RecordId()
        {
        }

        public override string ToString() => OwnerId + ":" + Id;

        public static bool operator ==(RecordId a, RecordId b) => a is null ? b is null : a.Equals(b);

        public static bool operator !=(RecordId a, RecordId b) => !(a == b);
    }
}