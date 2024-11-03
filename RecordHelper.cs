using System;
using CloudX.Shared;

namespace ResonitePackageExporter
{
    public static class RecordHelper
    {
        public static R CreateForObject<R>(string name, string ownerId, string assetUrl, string thumbnailUrl = null, string recordId = null) where R : class, IRecord, new()
        {
            R r = new()
            {
                Name = name,
                AssetURI = assetUrl,
                ThumbnailURI = thumbnailUrl,
                OwnerId = ownerId,
                RecordId = recordId ?? GenerateRecordID(),
                RecordType = "object",
                CreationTime = new DateTime?(DateTime.UtcNow),
                LastModificationTime = DateTime.UtcNow
            };
            return r;
        }
        public static bool IsValidRecordID(string recordId) => !string.IsNullOrWhiteSpace(recordId) && recordId.StartsWith("R-") && recordId.Length > "R-".Length;
        public static string GenerateRecordID()
        {
            return "R-" + Guid.NewGuid().ToString();
        }
    }
}
