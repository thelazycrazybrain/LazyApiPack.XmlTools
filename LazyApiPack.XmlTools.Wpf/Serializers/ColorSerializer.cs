using LazyApiPack.XmlTools.Helpers;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class ColorSerializer : IExternalObjectSerializer {
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var a = byte.Parse(node.Attribute(XName.Get("A"))?.Value ?? "0", format);
            var r = byte.Parse(node.Attribute(XName.Get("R"))?.Value?? "0", format);
            var g = byte.Parse(node.Attribute(XName.Get("G"))?.Value?? "0", format);
            var b = byte.Parse(node.Attribute(XName.Get("B"))?.Value?? "0", format);
            return Color.FromArgb(a, r, g, b);
        }
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is Color color) {
                writer.WriteAttributeString("A", color.A.ToString(format));
                writer.WriteAttributeString("R", color.R.ToString(format));
                writer.WriteAttributeString("G", color.G.ToString(format));
                writer.WriteAttributeString("B", color.B.ToString(format));
                return true;
            }
            return false;

        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type.IsAssignableTo(typeof(Color));
        }

    }
}