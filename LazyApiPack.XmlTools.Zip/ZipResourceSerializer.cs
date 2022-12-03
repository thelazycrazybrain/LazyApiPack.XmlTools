using LazyApiPack.XmlTools.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics.X86;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Zip {
    public class ZipResourceSerializer : IExternalObjectSerializer {
        private ResourceManager _resourceManager;
        private bool _byteArrayAsZipEntry;
        private const string ZIP_RESOURCE_DATA_FORMAT_NAME = "zipResource";

        public ZipResourceSerializer([NotNull] ResourceManager resourceManager, bool byteArrayAsZipEntry) {
            _resourceManager = resourceManager;
            _byteArrayAsZipEntry = byteArrayAsZipEntry;
        }
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            if (node.Value == null) return null;
            if (type == typeof(ZipResource)) {
                return new ZipResource(_resourceManager.GetResource(node.Value));
            } else if (_byteArrayAsZipEntry && type == typeof(byte[]) && dataFormat == ZIP_RESOURCE_DATA_FORMAT_NAME ) {
                return _resourceManager.GetResource(node.Value);
            } else {
                throw new NotSupportedException($"Type {type.FullName} with data format {dataFormat} is not supported by {nameof(ZipResourceSerializer)}.{nameof(Deserialize)}.");
            }
        }
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, 
                              bool serializeAsAttribute, IFormatProvider format, 
                              string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            string id;
            if (value is ZipResource zr) {
                id = _resourceManager.AddResource(zr.Data);
            } else if (_byteArrayAsZipEntry && value is byte[] ba) {
                setDataFormat(ZIP_RESOURCE_DATA_FORMAT_NAME);
                id = _resourceManager.AddResource(ba);
            } else {
                return false;
            }

            writer.WriteValue(id);
            return true;
        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return
                (type.IsAssignableTo(typeof(ZipResource))) ||
                (_byteArrayAsZipEntry ? type.IsAssignableTo(typeof(byte[])) && (dataFormat == null || dataFormat == ZIP_RESOURCE_DATA_FORMAT_NAME) : false);
        }
    }
}