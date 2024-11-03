using CodeX;
using System;
using System.IO;
using System.Text.Json;
using System.IO.Compression;
using System.Threading.Tasks;
using Stream = System.IO.Stream;
using Record = CloudX.Shared.Record;
using System.Collections.Generic;

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
        private Dictionary<string, Record> _records = new();
        private Dictionary<string, ZipArchiveEntry> _assets = new();
        private Dictionary<string, Dictionary<string, ZipArchiveEntry>> _variants = new();
        private Dictionary<string, IAssetMetadata> _metadata = new();

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

        /*public static async Task<RecordPackage> Decode(string file) => await RecordPackage.Decode(File.OpenRead(file));

        public static async Task<RecordPackage> Decode(Stream stream)
        {
            RecordPackage recordPackage = new();
            await recordPackage.Load(stream);
            return recordPackage;
        }*/

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
            await JsonSerializer.SerializeAsync(utf8Json, ResoniteConvert.RecordConverter.NeosRecordToResonite(record));//JsonSerializer.Serialize<Record>(utf8Json, record, null);


        }

        public async Task WriteMetadata(IAssetMetadata metadata)
        {
            if (string.IsNullOrEmpty(metadata.AssetIdentifier))
                throw new ArgumentException("Metadata is missing asset identifier");
            if (_metadata.ContainsKey(metadata.AssetIdentifier))
                throw new InvalidOperationException("Metadata for asset " + metadata.AssetIdentifier + " has already been added!");
            _metadata.Add(metadata.AssetIdentifier, metadata);
            switch (metadata)
            {
                case BitmapMetadata metadata1:
                    await WriteMetadata(metadata1, "bitmap");
                    break;
                case CubemapMetadata metadata2:
                    await WriteMetadata(metadata2, "cubemap");
                    break;
                /*case VolumeMetadata metadata3:
                    this.WriteMetadata<VolumeMetadata>(metadata3, "volume");
                    break;*/
                case MeshMetadata metadata4:
                    await WriteMetadata(metadata4, "mesh");
                    break;
                case ShaderMetadata metadata5:
                    await WriteMetadata(metadata5, "shader");
                    break;
                default:
                    throw new ArgumentException("Unsupported metadata type: " + metadata?.ToString());
            }
        }

        private async Task WriteMetadata<M>(M metadata, string extension) where M : IAssetMetadata
        {
            /*Logger.Log(JsonSerializer.Serialize(metadata, new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));*/
            var serialized = System.Text.Json.JsonSerializer.Serialize(metadata, metadata.GetType(), null);

            using Stream utf8Json = _archive.CreateEntry("Metadata/" + metadata.AssetIdentifier + "." + extension, CompressionLevel.Optimal).Open();
            using Utf8JsonWriter utf8JsonWriter = new(utf8Json, default);

            JsonSerializer.Serialize(utf8JsonWriter, metadata, new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
        }

        /*private async Task DecodeMetadata<M>(string signature, ZipArchiveEntry entry) where M : IAssetMetadata
        {
            using (Stream utf8Json = entry.Open())
            {
                M m = await JsonSerializer.DeserializeAsync<M>(utf8Json);
                _metadata.Add(signature, m);
            }
        }

        public void ExtractAsset(string signature, Stream targetStream)
        {
            signature = signature.ToLower();
            Extract(_assets, signature, targetStream);
        }

        public void ExtractVariant(string signature, string variantIdentifier, Stream targetStream) => Extract(_variants[signature], variantIdentifier, targetStream);
*/
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

        /*public Stream ReadAsset(string signature)
        {
            signature = signature.ToLower();
            return Read(_assets, signature);
        }

        public Stream ReadVariant(string signature, string variantIdentifier) => Read(_variants[signature], variantIdentifier);
*/
        public void WriteVariant(string signature, string variantIdentifier, string file)
        {
            using FileStream assetData = File.OpenRead(file);
            WriteVariant(signature, variantIdentifier, (Stream)assetData);
        }

        public void WriteVariant(string signature, string variantIdentifier, Stream assetData)
        {
            if (!_variants.TryGetValue(signature, out Dictionary<string, ZipArchiveEntry> entries))
            {
                entries = new Dictionary<string, ZipArchiveEntry>();
                _variants.Add(signature, entries);
            }
            Write(entries, "Variants/" + signature, variantIdentifier, assetData);
        }

        /*private void Extract(
          Dictionary<string, ZipArchiveEntry> entries,
          string identifier,
          Stream targetStream)
        {
            using (Stream stream = Read(entries, identifier))
                stream.CopyTo(targetStream);
        }

        private Stream Read(Dictionary<string, ZipArchiveEntry> entries, string identifier)
        {
            ZipArchiveEntry zipArchiveEntry;
            if (!entries.TryGetValue(identifier, out zipArchiveEntry))
                throw new KeyNotFoundException("Package doesn't contain asset/variant " + identifier);
            return zipArchiveEntry.Open();
        }
*/
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

        /*private async Task Load(Stream stream)
        {
            _archive = new ZipArchive(stream);
            foreach (ZipArchiveEntry entry in _archive.Entries)
            {
                if (entry.FullName != null)
                {
                    if (".record".Equals(Path.GetExtension(entry.FullName), StringComparison.OrdinalIgnoreCase))
                    {
                        using (Stream utf8Json = entry.Open())
                        {
                            Record record = await JsonSerializer.DeserializeAsync<Record>(utf8Json);
                            _records.Add(record.RecordId, record);
                        }
                    }
                    else if (entry.FullName.StartsWith("Assets", true, CultureInfo.InvariantCulture))
                        _assets.Add(Path.GetFileNameWithoutExtension(entry.FullName).ToLower(), entry);
                    else if (entry.FullName.StartsWith("Variants", true, CultureInfo.InvariantCulture))
                    {
                        string withoutExtension = Path.GetFileNameWithoutExtension(entry.FullName);
                        string fileName = Path.GetFileName(Path.GetDirectoryName(entry.FullName));
                        Dictionary<string, ZipArchiveEntry> dictionary;
                        if (!_variants.TryGetValue(fileName, out dictionary))
                        {
                            dictionary = new Dictionary<string, ZipArchiveEntry>();
                            _variants.Add(fileName, dictionary);
                        }
                        dictionary.Add(withoutExtension, entry);
                    }
                    else if (entry.FullName.StartsWith("Metadata", true, CultureInfo.InvariantCulture))
                    {
                        string str = Path.GetExtension(entry.FullName)?.Replace(".", "");
                        string withoutExtension = Path.GetFileNameWithoutExtension(entry.FullName);
                        if (!(str == "bitmap"))
                        {
                            if (!(str == "cubemap"))
                            {
                                //if (!(str == "volume"))
                                //{
                                    if (!(str == "mesh"))
                                    {
                                        if (str == "shader")
                                            await DecodeMetadata<ShaderMetadata>(withoutExtension, entry);
                                        else
                                            Logger.Warning("Unsupported metadata type in package: " + str);
                                    }
                                    else
                                        await DecodeMetadata<MeshMetadata>(withoutExtension, entry);
                                //}
                                else
                                //    await DecodeMetadata<VolumeMetadata>(withoutExtension, entry);
                            }
                            else
                                await DecodeMetadata<CubemapMetadata>(withoutExtension, entry);
                        }
                        else
                            await DecodeMetadata<BitmapMetadata>(withoutExtension, entry);
                    }
                }
            }
        }
        */
        public void Dispose()
        {
            _archive?.Dispose();
            _archive = null;
        }
    }
}
