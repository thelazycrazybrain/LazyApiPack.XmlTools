using LazyApiPack.XmlTools.Helpers;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class StylusPointCollectionSerializer : IExternalObjectSerializer {
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var spc = new StylusPointCollection();
            foreach (var spcx in node.Elements(XName.Get("StylusPoint"))) {
                spc.Add(new StylusPoint(
                        double.Parse(spcx.Attribute(XName.Get("X"))?.Value ?? "0", format),
                        double.Parse(spcx.Attribute(XName.Get("Y"))?.Value ?? "0", format),
                        float.Parse(spcx.Attribute(XName.Get("PressureFactor"))?.Value ?? "0", format)
                    ));
            }
            return spc;
        }
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is StylusPointCollection spc) {
                foreach (StylusPoint stylusPoint in spc) {
                    writer.WriteStartElement("StylusPoint");
                    writer.WriteAttributeString("PressureFactor", stylusPoint.PressureFactor.ToString(format));
                    writer.WriteAttributeString("X", stylusPoint.X.ToString(format));
                    writer.WriteAttributeString("Y", stylusPoint.Y.ToString(format));
                    writer.WriteEndElement();
                }
                return true;
            }
            return false;

        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(StylusPointCollection);
        }

    }
}