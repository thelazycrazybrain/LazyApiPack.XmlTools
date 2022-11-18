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
    /// <typeparam name="TClass">The class type that is to be serialized.</typeparam>
    public class ZipSerializer<TClass> where TClass : class {
        /// <summary>
        /// Instance of the xml serializer to serialize the class.
        /// </summary>
        ExtendedXmlSerializer<TClass> _xmlSerializer;

        /// <summary>
        /// Creates an instance of the Zip Serializer.
        /// </summary>
        /// <param name="serializer">The xml serializer that is used to serialize the class.</param>
        public ZipSerializer([NotNull] ExtendedXmlSerializer<TClass> serializer) {
            _xmlSerializer = serializer;
        }

        /// <summary>
        /// Serializes a class to an xml stream
        /// </summary>
        /// <param name="sourceClass">The class to serialize.</param>
        /// <param name="checkDuplicates">If true, duplicates (determined by SHA256 hash) will not be safed.</param>
        /// <param name="compressionLevel">The compression level for the zip file.</param>
        /// <returns>The stream containing the serialized class.</returns>
        /// <exception cref="NullReferenceException">If sourceclass is null.</exception>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        public Stream Serialize([NotNull] TClass sourceClass, bool checkDuplicates = true,
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
        /// <summary>
        /// Serializes a class to an xml file.
        /// </summary>
        /// <param name="sourceClass">The class to serialize.</param>
        /// <param name="fileName">The fully qualified file name for the xml.</param>
        ///  /// <param name="checkDuplicates">If true, duplicates (determined by SHA256 hash) will not be safed.</param>
        /// <param name="compressionLevel">The compression level for the zip file.</param>
        public void Serialize(TClass sourceClass, string fileName, bool checkDuplicates = true, CompressionLevel compressionLevel = CompressionLevel.Optimal) {
            using (var serialized = Serialize(sourceClass, checkDuplicates, compressionLevel))
            using (var fs = File.OpenWrite(fileName)) {
                fs.SetLength(serialized.Length);
                serialized.CopyTo(fs);
            }
        }

        /// <summary>
        /// Deserializes a class from an zip stream.
        /// </summary>
        /// <param name="checkAppCompatibility">If true, the serializer checks the application compatibility for migration.</param>
        /// <returns>The object that has been deserialized from the given zip stream.</returns>
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
        /// <summary>
        /// Deserializes an zip file to an object.
        /// </summary>
        /// <param name="file">Full file name of the xml.</param>
        /// <param name="checkAppCompatibility">If true, the serializer checks the application compatibility (migration).</param>
        /// <returns>The object that has been deserialized from the given zip file.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public TClass Deserialize(string file, bool checkAppCompatibility = false) {
            if (!File.Exists(file)) throw new FileNotFoundException($"The specified file ({file}) does not exist");
            using (var fs = File.OpenRead(file)) {
                return Deserialize(fs, checkAppCompatibility);
            }
        }


    }
}