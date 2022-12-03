using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers
{
    /// <summary>
    /// Adds serialization support for System.Windows.Point.
    /// </summary>
    public class PointSerializer : IExternalObjectSerializer
    {
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat)
        {
            if (value == null)
            {
                writer.WriteValue(null);
            }
            else
            {
                writer.WriteValue(((Point)value).ToString());
            }
            return true;
        }

        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat)
        {
            if (string.IsNullOrWhiteSpace(node.Value)) return default(Point);
            return Point.Parse(node.Value);
        }

        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat)
        {
            return type == typeof(Point);
        }


    }
}