using LazyApiPack.XmlTools.Helpers;
using System.CodeDom;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf {
    /// <summary>
    /// Adds class extensions for the ExtendedXmlSerializer to add extended type support for wpf specific types.
    /// </summary>
    public static class WpfExtendedXmlSerializer {
        /// <summary>
        /// Adds support for Wpf specific types.
        /// </summary>
        /// <typeparam name="TClass">Class Type of the serializer target object.</typeparam>
        /// <param name="serializer">The serializer that uses the extensions.</param>
        public static ExtendedXmlSerializer<TClass> UseWpfTypeSupport
            <TClass>(this ExtendedXmlSerializer<TClass> serializer) where TClass : class {
            serializer.ExternalSerializers.Add(new WindowsPointSerializer());
            serializer.ExternalSerializers.Add(new WindowsThicknessSerializer());
            return serializer;
        }

    }

    /// <summary>
    /// Adds serialization support for System.Windows.Thickness.
    /// </summary>
    public class WindowsThicknessSerializer : IExternalObjectSerializer {
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string dateTimeFormat, bool suppressId) {
            if (value == null) {
                writer.WriteValue(null);
            } else {
                var thk = (Thickness)value;
                writer.WriteValue($"{thk.Left},{thk.Top},{thk.Right},{thk.Bottom}");
            }
            return true;
        }

        public object? Deserialize(string? value, Type type, IFormatProvider format, string dateTimeFormat, bool suppressId) {
            if (string.IsNullOrWhiteSpace(value)) return default(Thickness);

            var s = value.Split(",");
            if (s.Length == 4 && double.TryParse(s[0], out var l) && double.TryParse(s[0], out var t)&& double.TryParse(s[0], out var r)&& double.TryParse(s[0], out var b)) {
                return new Thickness(l, t, r, b);
            } else {
                throw new InvalidCastException($"Cannot convert {value} to {typeof(Thickness).FullName}.");
            }
        }

        public bool SupportsType(Type type) {
            return type == typeof(Thickness);
        }

    }

    /// <summary>
    /// Adds serialization support for System.Windows.Point.
    /// </summary>
    public class WindowsPointSerializer : IExternalObjectSerializer {
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string dateTimeFormat, bool suppressId) {
            if (value == null) {
                writer.WriteValue(null);
            } else {
                writer.WriteValue(((Point)value).ToString());
            }
            return true;
        }

        public object? Deserialize(string? value, Type type, IFormatProvider format, string dateTimeFormat, bool suppressId) {
            if (string.IsNullOrWhiteSpace(value)) return default(Point);
            return Point.Parse(value);
        }

        public bool SupportsType(Type type) {
            return type == typeof(Point);
        }


    }
}