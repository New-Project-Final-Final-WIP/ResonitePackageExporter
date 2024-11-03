//using Newtonsoft.Json;

using ResonitePackageExporter.Resonite;

using System.Text.Json;

namespace ResonitePackageExporter.ResoniteConvert
{
    public static class RecordConverter
    {
        public static Resonite.Record NeosRecordToResonite(CloudX.Shared.Record neosRecord)
        {
            var serializedRecord = JsonSerializer.Serialize(neosRecord);
            //Logger.Log(serializedRecord);
            var record = JsonSerializer.Deserialize<Record>(serializedRecord, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});
/*
            Logger.Log(record.ToString());

            var reserializedRecord = JsonSerializer.Serialize(record);
            Logger.Log(reserializedRecord);*/
            return record;
        }
    }
}
