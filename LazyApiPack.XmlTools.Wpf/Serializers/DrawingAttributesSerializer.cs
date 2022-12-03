using LazyApiPack.XmlTools.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Ink;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class DrawingAttributesSerializer : IExternalObjectSerializer {
        private readonly ColorSerializer _colorSerializer;
        private readonly MatrixSerializer _matrixSerializer;
        public DrawingAttributesSerializer([NotNull] ColorSerializer colorSerializer, [NotNull] MatrixSerializer matrixSerializer) {
            _colorSerializer = colorSerializer;
            _matrixSerializer = matrixSerializer;
        }
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {

            var datt = new DrawingAttributes();

            var colorNode = node.Element(XName.Get("Color"));
            var color = _colorSerializer.Deserialize(colorNode, typeof(Color), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);

            var stylusTipTransformNode = node.Element(XName.Get("StylusTipTransform"));
            if (stylusTipTransformNode != null) {
                var stylusTipTransform = (Matrix?)_matrixSerializer.Deserialize(stylusTipTransformNode, typeof(Transform), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                if (stylusTipTransform != null) {
                    datt.StylusTipTransform = stylusTipTransform.Value;
                }
            }

            var fitToCurveValue = node.Attribute(XName.Get("FitToCurve"))?.Value;
            if (fitToCurveValue != null) {
                datt.FitToCurve = bool.Parse(fitToCurveValue);
            }

            var widthValue = node.Attribute(XName.Get("Width"))?.Value;
            if (widthValue != null) {
                datt.Width= double.Parse(widthValue, format);
            }

            var heightValue = node.Attribute(XName.Get("Height"))?.Value;
            if (heightValue != null) {
                datt.Height =double.Parse(heightValue, format);
            }

            var ignorePressureValue = node.Attribute(XName.Get("IgnorePressure"))?.Value;
            if (ignorePressureValue != null) {
                datt.IgnorePressure = bool.Parse(ignorePressureValue);
            }

            var isHighlighterValue = node.Attribute(XName.Get("IsHighlighter"))?.Value;
            if (isHighlighterValue != null) {
                datt.IsHighlighter = bool.Parse(isHighlighterValue);
            }

            var stylusTipValue = node.Attribute(XName.Get("StylusTip"))?.Value;
            if (stylusTipValue != null) {
                datt.StylusTip = (StylusTip)Enum.Parse(typeof(StylusTip), stylusTipValue);
            }

            return datt;
        }

        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is DrawingAttributes drawingAttributes) {
                writer.WriteAttributeString("FitToCurve", drawingAttributes.FitToCurve.ToString(format));
                writer.WriteAttributeString("Width", drawingAttributes.Width.ToString(format));
                writer.WriteAttributeString("Height", drawingAttributes.Height.ToString(format));
                writer.WriteAttributeString("IgnorePressure", drawingAttributes.IgnorePressure.ToString(format));
                writer.WriteAttributeString("IsHighlighter", drawingAttributes.IsHighlighter.ToString(format));
                writer.WriteAttributeString("StylusTip", drawingAttributes.StylusTip.ToString());

                writer.WriteStartElement("Color");
                _colorSerializer.Serialize(writer, drawingAttributes.Color, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
                writer.WriteEndElement(); // Color

                writer.WriteStartElement("StylusTipTransform");
                _matrixSerializer.Serialize(writer, drawingAttributes.StylusTipTransform, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
                writer.WriteEndElement(); // StylusTipTransform
                return true;
            }
            return false;
        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(DrawingAttributes);
        }

    }
}