using LazyApiPack.XmlTools.Helpers;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class PointCollectionSerializer : IExternalObjectSerializer {
        private readonly PointSerializer _pointSerializer;

        public PointCollectionSerializer(PointSerializer pointSerializer) {
            _pointSerializer = pointSerializer;
        }
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var collection = new PointCollection();

            foreach (var pointNode in node.Elements()) {
                var point = (Point?)_pointSerializer.Deserialize(pointNode, typeof(Point), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                if (point != null) {
                    collection.Add(point.Value);
                }

            }
            return collection;
        }
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is PointCollection collection) {
                foreach (Point point in collection) {
                    writer.WriteStartElement("Point");
                    _pointSerializer.Serialize(writer, point, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
                    writer.WriteEndElement();
                }
                return true;
            }
            return false;
        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(PointCollection);
        }


    }
}