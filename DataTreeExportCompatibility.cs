using BaseX;
using SevenZip;
using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft.Json.Bson;

namespace ResonitePackageExporter
{
    public static class DataTreeExportCompatibility
    {
        static readonly MethodInfo DataTreeWrite = typeof(DataTreeConverter).GetMethod("Write", BindingFlags.NonPublic | BindingFlags.Static);

        public static void Save(DataTreeDictionary root, string file)
        {
            using FileStream stream2 = File.OpenWrite(file);
            To7zBSON(root, stream2);
        }
        public static bool To7zBSON(DataTreeDictionary root, Stream stream)
        {
            WriteHeader(stream, Compression.LZMA);
            
            using MemoryStream memoryStream = new();
            using BsonDataWriter bsonDataWriter = new(memoryStream);

            bsonDataWriter.CloseOutput = false;
            DataTreeWrite.Invoke(null, parameters: [root, bsonDataWriter]);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            Helper.Compress(memoryStream, stream);
            return false;
        }

        private static void WriteHeader(Stream stream, Compression compression)
        {
            using BinaryWriter binaryWriter = new(stream, Encoding.UTF8, true);

            for (int i = 0; i < "FrDT".Length; i++)
            {
                binaryWriter.Write((byte)"FrDT"[i]);
            }
            binaryWriter.Write(0);
            binaryWriter.WriteEnumBinary(compression);
        }

        public enum Compression
        {
            None,
            LZ4,
            LZMA,
            Brotli
        }
    }
}
