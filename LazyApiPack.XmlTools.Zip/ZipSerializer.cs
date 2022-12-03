using LazyApiPack.XmlTools.Exceptions;
using LazyApiPack.XmlTools.Zip.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO.Compression;
using System.Net.Security;
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
        /// <param name="byteArrayAsZipEntry">Serializes the type byte[] as zip archive file.</param>
        /// <exception cref="NullReferenceException">If sourceclass is null.</exception>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        public Stream Serialize([NotNull] TClass sourceClass, bool checkDuplicates = true,
                                CompressionLevel compressionLevel = CompressionLevel.Optimal, bool byteArrayAsZipEntry = true) {
            return Serialize(checkDuplicates, compressionLevel, (w) => w.Serialize(sourceClass), byteArrayAsZipEntry);


        }

        /// <summary>
        /// Serializes multiple classes to an xml stream
        /// </summary>
        /// <param name="sourceClasses">The classes to serialize.</param>
        /// <param name="checkDuplicates">If true, duplicates (determined by SHA256 hash) will not be safed.</param>
        /// <param name="compressionLevel">The compression level for the zip file.</param>
        /// <param name="byteArrayAsZipEntry">Serializes the type byte[] as zip archive file.</param>
        /// <returns>The stream containing the serialized class.</returns>
        /// <exception cref="NullReferenceException">If sourceclass is null.</exception>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        public Stream SerializeAll([NotNull] IEnumerable<TClass> sourceClasses, bool checkDuplicates = true,
                                CompressionLevel compressionLevel = CompressionLevel.Optimal, bool byteArrayAsZipEntry = true) {
            return Serialize(checkDuplicates, compressionLevel, (w) => w.SerializeAll(sourceClasses), byteArrayAsZipEntry);


        }

        /// <summary>
        /// Serializes a class to an xml file.
        /// </summary>
        /// <param name="sourceClass">The class to serialize.</param>
        /// <param name="fileName">The fully qualified file name for the xml.</param>
        /// <param name="checkDuplicates">If true, duplicates (determined by SHA256 hash) will not be safed.</param>
        /// <param name="compressionLevel">The compression level for the zip file.</param>
        /// <param name="byteArrayAsZipEntry">Serializes the type byte[] as zip archive file.</param>
        public void Serialize(TClass sourceClass, string fileName, bool checkDuplicates = true, CompressionLevel compressionLevel = CompressionLevel.Optimal, bool byteArrayAsZipEntry = true) {
            using (var serialized = Serialize(sourceClass, checkDuplicates, compressionLevel, byteArrayAsZipEntry))
            using (var fs = File.OpenWrite(fileName)) {
                fs.SetLength(serialized.Length);
                serialized.CopyTo(fs);
            }
        }

        /// <summary>
        /// Serializes multiple classes to an xml file.
        /// </summary>
        /// <param name="sourceClasses">The classes to serialize.</param>
        /// <param name="fileName">The fully qualified file name for the xml.</param>
        /// <param name="checkDuplicates">If true, duplicates (determined by SHA256 hash) will not be safed.</param>
        /// <param name="compressionLevel">The compression level for the zip file.</param>
        /// <param name="byteArrayAsZipEntry">Serializes the type byte[] as zip archive file.</param>
        public void SerializeAll(IEnumerable<TClass> sourceClasses, string fileName, bool checkDuplicates = true, CompressionLevel compressionLevel = CompressionLevel.Optimal, bool byteArrayAsZipEntry = true) {
            using (var serialized = SerializeAll(sourceClasses, checkDuplicates, compressionLevel, byteArrayAsZipEntry))
            using (var fs = File.OpenWrite(fileName)) {
                fs.SetLength(serialized.Length);
                serialized.CopyTo(fs);
            }
        }


        /// <summary>
        /// Deserializes a class from a zip stream.
        /// </summary>
        /// <param name="sourceStream">The zip stream.</param>
        /// <param name="checkAppCompatibility">If true, the serializer checks the application compatibility for migration.</param>
        /// <param name="byteArrayAsZipEntry">Serializes the type byte[] as zip archive file.</param>
        /// <returns>The object that has been deserialized from the given zip stream.</returns>
        public TClass Deserialize(Stream sourceStream, bool checkAppCompatibility = false, bool byteArrayAsZipEntry = true) {
            return Deserialize(sourceStream, (w, s) => w.Deserialize(s, checkAppCompatibility), byteArrayAsZipEntry);

        }
        /// <summary>
        /// Deserializes a zip file to an object.
        /// </summary>
        /// <param name="file">Full file name of the xml.</param>
        /// <param name="checkAppCompatibility">If true, the serializer checks the application compatibility (migration).</param>
        /// <param name="byteArrayAsZipEntry">Serializes the type byte[] as zip archive file.</param>
        /// <returns>The object that have been deserialized from the given zip file.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public TClass Deserialize(string file, bool checkAppCompatibility = false, bool byteArrayAsZipEntry = true) {
            if (!File.Exists(file)) throw new FileNotFoundException($"The specified file ({file}) does not exist");
            using (var fs = File.OpenRead(file)) {
                return Deserialize(fs, checkAppCompatibility, byteArrayAsZipEntry);
            }
        }

        /// <summary>
        /// Deserializes multiple classes from a zip stream.
        /// </summary>
        /// <param name="checkAppCompatibility">If true, the serializer checks the application compatibility for migration.</param>
        /// <param name="byteArrayAsZipEntry">Serializes the type byte[] as zip archive file.</param>
        /// <returns>The object that has been deserialized from the given zip stream.</returns>
        public IEnumerable<TClass> DeserializeAll(Stream sourceStream, bool checkAppCompatibility = false, bool byteArrayAsZipEntry = true) {
            return Deserialize(sourceStream, (w, s) => w.DeserializeAll(s, checkAppCompatibility), byteArrayAsZipEntry);

        }
        /// <summary>
        /// Deserializes a zip file to a list.
        /// </summary>
        /// <param name="file">Full file name of the xml.</param>
        /// <param name="checkAppCompatibility">If true, the serializer checks the application compatibility (migration).</param>
        /// <param name="byteArrayAsZipEntry">Serializes the type byte[] as zip archive file.</param>
        /// <returns>The objects that have been deserialized from the given zip file.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public IEnumerable<TClass> DeserializeAll(string file, bool checkAppCompatibility = false, bool byteArrayAsZipEntry = true) {
            if (!File.Exists(file)) throw new FileNotFoundException($"The specified file ({file}) does not exist");
            using (var fs = File.OpenRead(file)) {
                return DeserializeAll(fs, checkAppCompatibility, byteArrayAsZipEntry);
            }
        }
        /// <summary>
        /// Serializes a class to an xml stream using custom serializer functions.
        /// </summary>
        /// <param name="checkDuplicates">If true, duplicates (determined by SHA256 hash) will not be safed.</param>
        /// <param name="compressionLevel">The compression level for the zip file.</param>
        /// <param name="serializeXml">Is called when the xml serializer is prepared and the xml stream is needed.</param>
        /// <returns>The stream containing the serialized object.</returns>
        private Stream Serialize(bool checkDuplicates, CompressionLevel compressionLevel, Func<ExtendedXmlSerializer<TClass>, Stream> serializeXml, bool byteArrayAsZipEntry = true) {
            ZipResourceSerializer? resourceSerializer = null;
            try {
                using (var ms = new MemoryStream()) {
                    using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true)) {
                        var resourceManager = new ResourceManager(checkDuplicates, archive, compressionLevel);
                        _xmlSerializer.ExternalSerializers.Add(resourceSerializer = new ZipResourceSerializer(resourceManager, byteArrayAsZipEntry));
                        var xml = serializeXml(_xmlSerializer);
                        using (var xmlArchiveStream = archive.CreateEntry("data.xml", compressionLevel).Open()) {
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
        /// Deserializes a zip file to objects using custom deserialization.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sourceStream">The zip stream.</param>
        /// <param name="deserialize">The function that is used to deserialize the objects.</param>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private TResult Deserialize<TResult>(Stream sourceStream, Func<ExtendedXmlSerializer<TClass>, Stream, TResult> deserialize, bool byteArrayAsZipEntry = true) {
            var archive = new ZipArchive(sourceStream, ZipArchiveMode.Read, false);
            var xmlEntry = archive.GetEntry("data.xml") ?? throw new ExtendedXmlSerializationException("data.xml file is missing in archive.");

            ZipResourceSerializer? resourceSerializer = null;
            try {
                var resourceManager = new ResourceManager(true, archive, CompressionLevel.Optimal);
                _xmlSerializer.ExternalSerializers.Add(resourceSerializer = new ZipResourceSerializer(resourceManager, byteArrayAsZipEntry));

                using (var xmlStream = xmlEntry.Open()) {
                    return deserialize(_xmlSerializer, StreamHelper.AsSeekable(xmlStream));
                }
            }
            finally {
                if (resourceSerializer != null) {
                    _xmlSerializer.ExternalSerializers.Remove(resourceSerializer);
                }
            }
        }

    }
}