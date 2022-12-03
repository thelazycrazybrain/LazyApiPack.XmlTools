using LazyApiPack.XmlTools.Helpers;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class MatrixSerializer : IExternalObjectSerializer {
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var m11 = double.Parse(node.Attribute(XName.Get("M11"))?.Value ?? "0", format);
            var m12 = double.Parse(node.Attribute(XName.Get("M12"))?.Value ?? "0", format);
            var m21 = double.Parse(node.Attribute(XName.Get("M21"))?.Value ?? "0", format);
            var m22 = double.Parse(node.Attribute(XName.Get("M22"))?.Value ?? "0", format);
            var offsetX = double.Parse(node.Attribute(XName.Get("OffsetX"))?.Value ?? "0", format);
            var offsetY = double.Parse(node.Attribute(XName.Get("OffsetY"))?.Value ?? "0", format);
            return new Matrix(m11, m12, m21, m22, offsetX, offsetY);
        }
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is Matrix matrix) {
                writer.WriteAttributeString("M11", matrix.M11.ToString(format));
                writer.WriteAttributeString("M12", matrix.M12.ToString(format));
                writer.WriteAttributeString("M21", matrix.M21.ToString(format));
                writer.WriteAttributeString("M22", matrix.M22.ToString(format));
                writer.WriteAttributeString("OffsetX", matrix.OffsetX.ToString(format));
                writer.WriteAttributeString("OffsetY", matrix.OffsetY.ToString(format));
                return true;
            }
            return false;

        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(Matrix);
        }

    }
}