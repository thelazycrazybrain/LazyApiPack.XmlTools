using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace LazyApiPack.XmlTools.Zip {
    public class ZipResourceSerializer : IExternalObjectSerializer {
        private ResourceManager _resourceManager;
        public ZipResourceSerializer([NotNull] ResourceManager resourceManager) {
            _resourceManager = resourceManager;
        }
        public object? Deserialize(string? value, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization) {
            if (value == null) return null;
            return new ZipResource(_resourceManager.GetResource(value));
        }

        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization) {
            if (value is ZipResource zr) {
                var id = _resourceManager.AddResource(zr.Data);
                writer.WriteValue(id);
            }
            return true;
        }

        public bool SupportsType(Type type) {
            return type.IsAssignableTo(typeof(ZipResource));
        }
    }
}