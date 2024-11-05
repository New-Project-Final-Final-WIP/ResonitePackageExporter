using System.Text.Json;

namespace ResonitePackageExporter.Resonite
{
    public static class RecordConverter
    {
        public static Record NeosRecordToResonite(CloudX.Shared.Record neosRecord)
        {
            var serializedRecord = JsonSerializer.Serialize(neosRecord);
            var record = JsonSerializer.Deserialize<Record>(serializedRecord, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});
            return record;
        }
    }
}
