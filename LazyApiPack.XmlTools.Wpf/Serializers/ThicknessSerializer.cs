using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    /// <summary>
    /// Adds serialization support for System.Windows.Thickness.
    /// </summary>
    public class ThicknessSerializer : IExternalObjectSerializer {
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) {
                writer.WriteValue(null);
            } else {
                var thk = (Thickness)value;
                writer.WriteValue($"{thk.Left},{thk.Top},{thk.Right},{thk.Bottom}");
            }
            return true;
        }
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            if (string.IsNullOrWhiteSpace(node.Value)) return default(Thickness);

            var s = node.Value.Split(",");
            if (s.Length == 4 && double.TryParse(s[0], out var l) && double.TryParse(s[0], out var t) && double.TryParse(s[0], out var r) && double.TryParse(s[0], out var b)) {
                return new Thickness(l, t, r, b);
            } else {
                throw new InvalidCastException($"Cannot convert {node.Value} to {typeof(Thickness).FullName}.");
            }
        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(Thickness);

        }
    }
}