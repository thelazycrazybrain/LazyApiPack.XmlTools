using LazyApiPack.XmlTools.Helpers;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class TextDecorationCollectionSerializer : IExternalObjectSerializer {
        private readonly PenSerializer _penSerializer;
        public TextDecorationCollectionSerializer(PenSerializer penSerializer) {
            _penSerializer = penSerializer;
        }
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var collection = new TextDecorationCollection();
            foreach (var decoration in node.Elements(XName.Get("TextDecoration"))) {
                var td = new TextDecoration();

                var locationValue = decoration.Attribute(XName.Get("Location"))?.Value;
                if (locationValue != null) {
                    td.Location = (TextDecorationLocation)Enum.Parse(typeof(TextDecorationLocation), locationValue);
                }

                var pen = (Pen?)_penSerializer.Deserialize(decoration, typeof(Pen), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                if (pen != null) {
                    td.Pen =pen;
                }

                var penOffsetValue = decoration.Attribute(XName.Get("PenOffset"))?.Value;
                if (penOffsetValue != null) {
                    td.PenOffset =double.Parse(penOffsetValue, format);
                }

                var penOffsetUnitValue = decoration.Attribute(XName.Get("Location"))?.Value;
                if (penOffsetUnitValue != null) {
                    td.PenOffsetUnit =(TextDecorationUnit)Enum.Parse(typeof(TextDecorationUnit), penOffsetUnitValue);
                }

                var penThicknessUnitValue = decoration.Attribute(XName.Get("PenThicknessUnit"))?.Value;
                if (penThicknessUnitValue != null) {
                    td.PenThicknessUnit =  (TextDecorationUnit)Enum.Parse(typeof(TextDecorationUnit), penThicknessUnitValue);
                }

                collection.Add(td);

            }
            return collection;
        }
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is TextDecorationCollection textDecorationCollection) {
                foreach (var textDecoration in textDecorationCollection) {
                    writer.WriteStartElement("TextDecoration");
                    writer.WriteAttributeString("Location", textDecoration.Location.ToString());
                    writer.WriteAttributeString("PenOffset", textDecoration.PenOffset.ToString(format));
                    writer.WriteAttributeString("PenOffsetUnit", textDecoration.PenOffsetUnit.ToString());
                    writer.WriteAttributeString("PenThicknessUnit", textDecoration.PenThicknessUnit.ToString());
                    writer.WriteStartElement("Pen");
                    _penSerializer.Serialize(writer, textDecoration.Pen, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
                    writer.WriteEndElement(); // Pen
                    writer.WriteEndElement(); // TextDecoration
                }
                return true;
            }
            return false;
        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(TextDecorationCollection);
        }

    }
}