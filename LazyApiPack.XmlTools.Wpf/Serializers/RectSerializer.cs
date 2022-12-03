using LazyApiPack.XmlTools.Helpers;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class RectSerializer : IExternalObjectSerializer {
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            return new Rect(
                double.Parse(node.Attribute(XName.Get("Left"))?.Value ?? "0", format),
                double.Parse(node.Attribute(XName.Get("Top"))?.Value ?? "0", format),
                double.Parse(node.Attribute(XName.Get("Width"))?.Value ?? "0", format),
                double.Parse(node.Attribute(XName.Get("Height"))?.Value ?? "0", format)
            );
        }
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is Rect rect) {
                writer.WriteAttributeString("Left", rect.Left.ToString(format));
                writer.WriteAttributeString("Top", rect.Top.ToString(format));
                writer.WriteAttributeString("Width", rect.Width.ToString(format));
                writer.WriteAttributeString("Height", rect.Height.ToString(format));
                return true;
            }
            return false;
        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(Rect);
        }

    }
}