using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System;

namespace ResonitePackageExporter.Resonite
{
    [Serializable]
    public class Submission
    {
        [JsonProperty(PropertyName = "id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "ownerId")]
        [JsonPropertyName("ownerId")]
        public string GroupId { get; set; }

        [JsonProperty(PropertyName = "targetRecordId")]
        [JsonPropertyName("targetRecordId")]
        public RecordId TargetRecordId { get; set; } = new RecordId();

        [JsonProperty(PropertyName = "submissionTime")]
        [JsonPropertyName("submissionTime")]
        public DateTime SubmissionTime { get; set; }

        [JsonProperty(PropertyName = "submittedById")]
        [JsonPropertyName("submittedById")]
        public string SubmittedById { get; set; }

        [JsonProperty(PropertyName = "submittedByName")]
        [JsonPropertyName("submittedByName")]
        public string SubmittedByName { get; set; }

        [JsonProperty(PropertyName = "featured")]
        [JsonPropertyName("featured")]
        public bool Featured { get; set; }

        [JsonProperty(PropertyName = "featuredByUserId")]
        [JsonPropertyName("featuredByUserId")]
        public string FeaturedByUserId { get; set; }

        [JsonProperty(PropertyName = "featuredTimestamp")]
        [JsonPropertyName("featuredTimestamp")]
        public DateTime? FeaturedTimestamp { get; set; }
    }
}