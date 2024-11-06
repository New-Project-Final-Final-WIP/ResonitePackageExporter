using CodeX;
using System;
using System.IO;
using System.Text.Json;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using Stream = System.IO.Stream;
using Record = CloudX.Shared.Record;

namespace ResonitePackageExporter
{
    public class RecordPackage : IDisposable
    {
        public const string ASSET_SCHEME = "packdb";
        public const string MAIN_RECORD_ID = "R-Main";
        public const string ASSETS_FOLDER = "Assets";
        public const string VARIANTS_FOLDER = "Variants";
        public const string METADATA_FOLDER = "Metadata";
        public const string RECORD_EXTENSION = ".record";

        private ZipArchive _archive;
        private readonly Dictionary<string, Record> _records = [];
        private readonly Dictionary<string, ZipArchiveEntry> _assets = [];
        private readonly Dictionary<string, Dictionary<string, ZipArchiveEntry>> _variants = [];
        private readonly Dictionary<string, IAssetMetadata> _metadata = [];

        public static Uri GetAssetURL(string signature) => new("packdb:///" + signature);

        public static string GetAssetSignature(Uri uri)
        {
            if (uri.Scheme != "packdb")
                throw new ArgumentException("Uri is not a package asset URL");
            return uri.Segments.Length < 2 ? null : Path.GetFileNameWithoutExtension(uri.Segments[1]);
        }

        public int RecordCount => _records.Count;

        public int AssetCount => _assets.Count;

        public Record MainRecord
        {
            get
            {
                _records.TryGetValue("R-Main", out Record mainRecord);
                return mainRecord;
            }
        }

        public IEnumerable<Record> Records => _records.Values;

        public IEnumerable<string> Assets => _assets.Keys;
        public IEnumerable<IAssetMetadata> Metadata => _metadata.Values;


        public static RecordPackage Create(Stream writeStream) => new()
        {
            _archive = new ZipArchive(writeStream, ZipArchiveMode.Create)
        };

        public bool HasAsset(string signature) => _assets.ContainsKey(signature?.ToLower());

        public bool HasVariant(string signature, string variantIdentifier)
        {
            signature = signature.ToLower();
            return _variants.TryGetValue(signature, out Dictionary<string, ZipArchiveEntry> dictionary) && dictionary.ContainsKey(variantIdentifier);
        }

        public IEnumerable<string> EnumerateVariantsForAsset(string signature)
        {
            signature = signature.ToLower();
            if (_variants.TryGetValue(signature, out Dictionary<string, ZipArchiveEntry> dictionary))
            {
                foreach (KeyValuePair<string, ZipArchiveEntry> keyValuePair in dictionary)
                    yield return keyValuePair.Key;
            }
        }

        public IAssetMetadata TryGetMetadata(string signature)
        {
            return _metadata.TryGetValue(signature, out IAssetMetadata assetMetadata) ? assetMetadata : null;
        }

        public async Task WriteRecord(Record record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));
            if (string.IsNullOrEmpty(record.RecordId))
                throw new ArgumentException("RecordId is empty");
            _records.Add(record.RecordId, record);

            using Stream utf8Json = _archive.CreateEntry(record.RecordId + ".record", CompressionLevel.Optimal).Open();
            await JsonSerializer.SerializeAsync(utf8Json, Resonite.RecordConverter.NeosRecordToResonite(record));
        }

        public void WriteMetadata(IAssetMetadata metadata)
        {
            if (string.IsNullOrEmpty(metadata.AssetIdentifier))
                throw new ArgumentException("Metadata is missing asset identifier");
            if (_metadata.ContainsKey(metadata.AssetIdentifier))
                throw new InvalidOperationException("Metadata for asset " + metadata.AssetIdentifier + " has already been added!");
            _metadata.Add(metadata.AssetIdentifier, metadata);
            switch (metadata)
            {
                case BitmapMetadata bitmap:
                    WriteMetadata(bitmap, "bitmap");
                    break;
                case CubemapMetadata cubemap:
                    WriteMetadata(cubemap, "cubemap");
                    break;
                case MeshMetadata mesh:
                    WriteMetadata(mesh, "mesh");
                    break;
                case ShaderMetadata shader:
                    WriteMetadata(shader, "shader");
                    break;
                default:
                    throw new ArgumentException("Unsupported metadata type: " + metadata?.ToString());
            }
        }

        private void WriteMetadata<M>(M metadata, string extension) where M : IAssetMetadata
        {
            using Stream utf8Json = _archive.CreateEntry("Metadata/" + metadata.AssetIdentifier + "." + extension, CompressionLevel.Optimal).Open();
            using Utf8JsonWriter utf8JsonWriter = new(utf8Json, default);

            JsonSerializer.Serialize(utf8JsonWriter, metadata, new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                // For some machines it seems JsonIgnore is not respected in some cases?
                // Adding the following options to resolve these issues
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
            });
        }

        public void WriteAsset(string signature, string file)
        {
            using FileStream assetData = File.OpenRead(file);
            WriteAsset(signature, assetData);
        }

        public void WriteAsset(string signature, Stream assetData)
        {
            signature = signature.ToLower();
            Write(_assets, "Assets", signature, assetData);
        }

        public void WriteVariant(string signature, string variantIdentifier, string file)
        {
            using FileStream assetData = File.OpenRead(file);
            WriteVariant(signature, variantIdentifier, assetData);
        }

        public void WriteVariant(string signature, string variantIdentifier, Stream assetData)
        {
            if (!_variants.TryGetValue(signature, out Dictionary<string, ZipArchiveEntry> entries))
            {
                entries = [];
                _variants.Add(signature, entries);
            }
            Write(entries, "Variants/" + signature, variantIdentifier, assetData);
        }

        private void Write(
          Dictionary<string, ZipArchiveEntry> entries,
          string folder,
          string identifier,
          Stream sourceData)
        {
            if (entries.ContainsKey(identifier))
                throw new InvalidOperationException("Asset/Variant " + identifier + " has already been written");
            ZipArchiveEntry entry = _archive.CreateEntry(folder + "/" + identifier, CompressionLevel.NoCompression);
            using (Stream destination = entry.Open())
                sourceData.CopyTo(destination);
            entries.Add(identifier, entry);
        }

        public void Dispose()
        {
            _archive?.Dispose();
            _archive = null;
        }
    }
}
