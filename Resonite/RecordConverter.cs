using System.Text.Json;

namespace ResonitePackageExporter.Resonite
{
    public static class RecordConverter
    {
        // NeosDB Record does currently work fine, however just to be safe if Resonite drops neos record support at some point I'll pre convert to a resonite record here
        public static Record NeosRecordToResonite(CloudX.Shared.Record neosRecord)
        {
            var serializedRecord = JsonSerializer.Serialize(neosRecord);
            var record = JsonSerializer.Deserialize<Record>(serializedRecord, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});
            return record;
        }
    }
}
