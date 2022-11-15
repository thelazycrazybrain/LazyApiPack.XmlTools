using LazyApiPack.XmlTools.Exceptions;
using LazyApiPack.XmlTools.Zip.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO.Compression;
using System.Runtime.Intrinsics.Arm;

namespace LazyApiPack.XmlTools.Zip {
    /// <summary>
    /// Provides functionality to serialize classes to a zip archive that uses resources.
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    public class ZipSerializer<TClass> where TClass : class {
        ExtendedXmlSerializer<TClass> _xmlSerializer;

        public ZipSerializer([NotNull] ExtendedXmlSerializer<TClass> serializer, bool checkDuplicates = true) {
            _xmlSerializer = serializer;
        }

        public Stream Serialize(TClass sourceClass, bool checkDuplicates = true,
                                CompressionLevel compressionLevel = CompressionLevel.Optimal) {
            ZipResourceSerializer? resourceSerializer = null;
            try {
                using (var ms = new MemoryStream()) {
                    using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true)) {
                        var resourceManager = new ResourceManager(checkDuplicates, archive, compressionLevel);
                        _xmlSerializer.ExternalSerializers.Add(resourceSerializer = new ZipResourceSerializer(resourceManager));

                        var xml = _xmlSerializer.Serialize(sourceClass);
                        var data = archive.CreateEntry("data.xml", compressionLevel);
                        using (var xmlArchiveStream = data.Open()) {
                            xml.CopyTo(xmlArchiveStream);
                            xmlArchiveStream.Close();
                        }
                    }

                    ms.Position = 0;
                    var result = new MemoryStream();
                    ms.CopyTo(result);
                    result.Position = 0;
                    return result;
                }
            }
            finally {
                if (resourceSerializer != null) {
                    _xmlSerializer.ExternalSerializers.Remove(resourceSerializer);
                }
            }

        }
        public TClass Deserialize(Stream sourceStream, bool checkAppCompatibility = false) {
            var archive = new ZipArchive(sourceStream, ZipArchiveMode.Read, false);
            var xmlEntry = archive.GetEntry("data.xml") ?? throw new ExtendedXmlSerializationException("data.xml file is missing in archive.");

            using (var xmlStream = xmlEntry.Open()) {
                ZipResourceSerializer? resourceSerializer = null;
                try {
                    var resourceManager = new ResourceManager(true, archive, CompressionLevel.Optimal);
                    _xmlSerializer.ExternalSerializers.Add(resourceSerializer = new ZipResourceSerializer(resourceManager));
                    return _xmlSerializer.Deserialize(StreamHelper.AsSeekable(xmlStream), checkAppCompatibility);

                }
                finally {
                    if (resourceSerializer != null) {
                        _xmlSerializer.ExternalSerializers.Remove(resourceSerializer);
                    }
                }

            }

        }

    }
}