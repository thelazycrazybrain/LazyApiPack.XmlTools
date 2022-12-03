using LazyApiPack.XmlTools.Zip.Helpers;
using System.IO.Compression;
using System.Security.Cryptography;

namespace LazyApiPack.XmlTools.Zip {
    public class ResourceManager {
        private ZipArchive _archive;
        private SHA256 _sha256 = SHA256.Create();
        /// <summary>
        /// If a zip resource was given but the data was empty.
        /// </summary>
        public const string EMPTY_VALUE = "";
        public bool CheckDuplicates { get; }
        public CompressionLevel CompressionLevel { get; }
        /// <summary>
        /// Contains the file id and the accompanying data hash.
        /// </summary>
        private Dictionary<byte[], string> _dataDictionary;

        public ResourceManager(bool checkDuplicates, ZipArchive archive, CompressionLevel compressionLevel) {
            CheckDuplicates = checkDuplicates;
            CompressionLevel = compressionLevel;
            _archive = archive;
            _dataDictionary = new Dictionary<byte[], string>(new ByteArrayComparer());

        }

        public string AddResource(byte[]? data) {
            string id;
            if (data == null || data.Length == 0) return EMPTY_VALUE;
            if (CheckDuplicates) {
                var hash = _sha256.ComputeHash(data);
                if (!_dataDictionary.ContainsKey(hash)) {
                    _dataDictionary.Add(hash, id = GetNewId());
                    CreateEntry(id, data);
                } else {
                    return _dataDictionary[hash];
                }
            } else {
                CreateEntry(id = GetNewId(), data);
            }
            return id;

        }

        public byte[]? GetResource(string id) {
            if (id == EMPTY_VALUE) return null;

            var entry = _archive.GetEntry(id);
            if (entry == null) throw new FileNotFoundException($"Resource {id} was not found.");

            using (var stream = entry.Open()) {
                using (var seekable = StreamHelper.AsSeekable(stream)) {
                    var buff = new byte[seekable.Length];
                    seekable.Read(buff, 0, buff.Length);
                    return buff;
                }
            }
        }
        void CreateEntry(string id, byte[] data) {
            var entry = _archive.CreateEntry(id, CompressionLevel);
            using (var entryStream = entry.Open()) {
                entryStream.Write(data, 0, data.Length);
            }
        }

        string GetNewId() {
            return $"{Guid.NewGuid().ToString("d")}.dat";
        }

        public void Clear() {
            _dataDictionary.Clear();
        }



    }
}