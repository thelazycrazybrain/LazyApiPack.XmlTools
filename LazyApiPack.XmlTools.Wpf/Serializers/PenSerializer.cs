using LazyApiPack.XmlTools.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class PenSerializer : IExternalObjectSerializer {
        private readonly DashStyleSerializer _dashStyleSerializer;
        private readonly BrushSerializer _brushSerializer;

        public PenSerializer([NotNull] DashStyleSerializer dashStyleSerializer, [NotNull] BrushSerializer brushSerializer) {
            _dashStyleSerializer = dashStyleSerializer;
            _brushSerializer = brushSerializer;
        }
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var pen = new Pen();

            var brushNode = node.Element(XName.Get("Brush"));
            if (brushNode != null) {
                var brush = (Brush?)_brushSerializer.Deserialize(brushNode, typeof(Brush), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                if (brush != null) {
                    pen.Brush = brush;
                }
            }

            var dashStyleNode = node.Element(XName.Get("DashStyle"));
            if (dashStyleNode != null) {
                var dashStyle = (DashStyle?)_dashStyleSerializer.Deserialize(dashStyleNode, typeof(DashStyle), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                if (dashStyle !=null) {
                    pen.DashStyle = dashStyle;
                }
            }

            var thicknessValue = node.Attribute(XName.Get("Thickness"))?.Value;
            if (thicknessValue != null) {
                pen.Thickness = double.Parse(thicknessValue, format);
            }

            var dashCapValue = node.Attribute(XName.Get("DashCap"))?.Value;
            if (dashCapValue != null) {
                pen.DashCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), dashCapValue);
            }

            var startLineCapValue = node.Attribute(XName.Get("StartLineCap"))?.Value;
            if (startLineCapValue != null) {
                pen.StartLineCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), startLineCapValue);
            }

            var endLineCapValue = node.Attribute(XName.Get("EndLineCap"))?.Value;
            if (endLineCapValue != null) {
                pen.EndLineCap = (PenLineCap)Enum.Parse(typeof(PenLineCap), endLineCapValue);
            }

            var lineJoinValue = node.Attribute(XName.Get("PenLineJoin"))?.Value;
            if (lineJoinValue != null) {
                pen.LineJoin = (PenLineJoin)Enum.Parse(typeof(PenLineJoin), lineJoinValue);
            }

            var miterLimitValue = node.Attribute(XName.Get("MiterLimit"))?.Value;
            if (miterLimitValue != null) {
                pen.MiterLimit = double.Parse(miterLimitValue, format);
            }
            return pen;
        }

        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is Pen pen) {
                writer.WriteAttributeString("DashCap", pen.DashCap.ToString());
                writer.WriteAttributeString("EndLineCap", pen.EndLineCap.ToString());
                writer.WriteAttributeString("PenLineJoin", pen.LineJoin.ToString());
                writer.WriteAttributeString("MiterLimit", pen.MiterLimit.ToString(format));
                writer.WriteAttributeString("StartLineCap", pen.StartLineCap.ToString());
                writer.WriteAttributeString("Thickness", pen.Thickness.ToString(format));
                writer.WriteStartElement("Brush");
                _brushSerializer.Serialize(writer, pen.Brush, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
                writer.WriteEndElement(); // Brush

                writer.WriteStartElement("DashStyle");
                _dashStyleSerializer.Serialize(writer, pen.DashStyle, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
                writer.WriteEndElement(); // DashStyle
                return true;
            }
            return false;
        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(Pen);
        }

    }
}