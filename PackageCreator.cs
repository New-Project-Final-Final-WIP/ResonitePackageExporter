using BaseX;
using CloudX.Shared;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FrooxEngine;
using CodeX;

namespace ResonitePackageExporter
{
    public static class PackageCreator
    {
        public static async Task BuildPackage(Engine engine, CloudX.Shared.Record record, SavedGraph savedGraph, System.IO.Stream writeStream, bool includeVariants)
        {
            Logger.Log("Building Package");
            RecordPackage package = RecordPackage.Create(writeStream);
            record.NeosDBManifest = [];

            if (savedGraph.Root.TryGetNode("Slots") != null)
                Logger.Warning("This is exporting as a world instead of an object!\nThis is currently unsupported on Resonite and may not import");
            
            Logger.Warning($"Using {(ResonitePackageExporter.UseNewtonsoftJson ? "NewtonsoftJson" : "System.Text.Json")} to export metadata");

            await CollectAssets(engine, record, savedGraph, package, includeVariants).ConfigureAwait(false);


            Logger.Log("Saving DataTree");
            /// THIS MIGHT NEED TO BE ADDRESSED
            string tempFilePath = engine.LocalDB.GetTempFilePath(".lz4bson"); //".brson"
            DataTreeExportCompatibility.Save(savedGraph.Root, tempFilePath);
            //DataTreeConverter.Save(savedGraph.Root, tempFilePath);  //, DataTreeConverter.Compression.Brotli
            ///

            string hashSignature = AssetUtil.GenerateHashSignature(tempFilePath);
            Uri assetUrl = RecordPackage.GetAssetURL(hashSignature);

            Logger.Log("Writing R-Main record");
            package.WriteAsset(hashSignature, tempFilePath);
            
            record.RecordId = "R-Main";
            record.AssetURI = assetUrl.OriginalString;
            
            await package.WriteRecord(record);
            package.Dispose();
            Logger.Log("Finished package!");
        }

        private static async Task CollectAssets(Engine engine, CloudX.Shared.Record record, SavedGraph savedGraph, RecordPackage package, bool includeVariants)
        {
            Logger.Log("Collecting assets");
            foreach (DataTreeValue urlNode1 in savedGraph.URLNodes)
            {
                DataTreeValue urlNode = urlNode1;
                if (!urlNode.IsNull)
                {
                    Uri url = urlNode.TryExtractURL();
                    if (!(url == null))
                    {
                        
                        bool isValidDBuri = CloudXInterface.IsValidNeosDBUri(url);
                        if (isValidDBuri || url.Scheme == "local")
                        {
                            AssetRecord assetRecord1 = await engine.LocalDB.TryFetchAssetRecordWithMetadataAsync(url).ConfigureAwait(false);
                            string assetSignature;
                            string str;
                            long num;
                            if (assetRecord1 == null)
                            {
                                if (isValidDBuri)
                                {
                                    assetSignature = CloudXInterface.NeosDBSignature(url);
                                    
                                    str = await engine.AssetManager.RequestGather(url, Priority.Background).ConfigureAwait(false);
                                    if (str == null)
                                    {
                                        Logger.Warning("Failed to gather asset for packaging: " + url?.ToString());
                                        continue;
                                    }
                                    num = new FileInfo(str).Length;
                                }
                                else
                                {
                                    Logger.Warning("Failed to fetch asset record for asset packaging: " + url?.ToString());
                                    continue;
                                }
                            }
                            else
                            {
                                assetSignature = assetRecord1.cloudsig;
                                str = assetRecord1.path;
                                num = assetRecord1.bytes;
                            }
                            urlNode.UpdateValue(RecordPackage.GetAssetURL(assetSignature));
                            if (!package.HasAsset(assetSignature))
                            {
                                package.WriteAsset(assetSignature, str);
                                record.NeosDBManifest.Add(new NeosDBAsset()
                                {
                                    Hash = assetSignature,
                                    Bytes = num
                                });
                                IAssetMetadata metadata = await engine.LocalDB.TryFetchAssetMetadataAsync(assetSignature).ConfigureAwait(false);
                                if (metadata != null)
                                {
                                    metadata.AssetIdentifier = assetSignature;
                                    package.WriteMetadata(metadata);
                                }
                                if (includeVariants)
                                {
                                    List<AssetRecord> allVariantsAsync = await engine.LocalDB.GetAllVariantsAsync(url);
                                    foreach (AssetRecord assetRecord2 in await engine.LocalDB.GetAllVariantsAsync(url).ConfigureAwait(false))
                                    {
                                        Uri uri = new(assetRecord2.url);
                                        string path = assetRecord2.path;
                                        if (File.Exists(path))
                                        {
                                            string variantIdentifier = uri.Query.Substring(1);
                                            if (!package.HasVariant(assetSignature, variantIdentifier))
                                                package.WriteVariant(assetSignature, variantIdentifier, path);
                                        }
                                    }
                                }
                            }
                            assetSignature = null;
                        }
                        url = null;
                        urlNode = null;
                    }
                }
            }
        }
    }
}
