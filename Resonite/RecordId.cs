using System;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ResonitePackageExporter.Resonite
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [Serializable]
    public class RecordId
    {
        [JsonProperty(PropertyName = "recordId")]
        [JsonPropertyName("recordId")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "ownerId")]
        [JsonPropertyName("ownerId")]
        public string OwnerId { get; set; }
    }
}