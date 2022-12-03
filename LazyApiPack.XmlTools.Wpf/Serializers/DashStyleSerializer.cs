using LazyApiPack.XmlTools.Helpers;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class DashStyleSerializer : IExternalObjectSerializer {
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var dashCollection = new DoubleCollection();
            var dashes = node.Element(XName.Get("Dashes"))?.Elements(XName.Get("Value"));
            if (dashes != null) {
                foreach (var dash in dashes) {
                    dashCollection.Add(double.Parse(dash.Value, format));
                }
            }
            var dashStyle = new DashStyle(dashCollection, 0);


            var offsetValue = node.Attribute(XName.Get("DashOffset"))?.Value;
            if (offsetValue != null) {
                dashStyle.Offset = double.Parse(offsetValue, format);
            }
            return dashStyle;
        }
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is DashStyle dashStyle) {

                writer.WriteAttributeString("DashOffset", dashStyle.Offset.ToString(format));
                writer.WriteStartElement("Dashes");
                foreach (var dash in dashStyle.Dashes) {
                    writer.WriteStartElement("Value");
                    writer.WriteValue(dash.ToString(format));
                    writer.WriteEndElement(); // Dash
                }
                writer.WriteEndElement(); // Dashes
                return true;
            }
            return false;
        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(DashStyle); ;
        }

    }
}