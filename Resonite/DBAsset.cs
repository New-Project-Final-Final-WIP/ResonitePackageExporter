using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System;

namespace ResonitePackageExporter.Resonite
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [Serializable]
    public class DBAsset
    {
        [JsonProperty(PropertyName = "hash")]
        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "bytes")]
        [JsonPropertyName("bytes")]
        public long Bytes { get; set; }
    }
}