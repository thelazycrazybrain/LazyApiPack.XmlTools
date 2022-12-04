using LazyApiPack.XmlTools.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class StrokeCollectionSerializer : IExternalObjectSerializer {
        private readonly ColorSerializer _colorSerializer;
        private readonly MatrixSerializer _matrixSerializer;
        public StrokeCollectionSerializer([NotNull] ColorSerializer colorSerializer, [NotNull] MatrixSerializer matrixSerializer) {
            _colorSerializer = colorSerializer;
            _matrixSerializer = matrixSerializer;

        }
        /// <inheritdoc/>
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var coll = Activator.CreateInstance(type) as StrokeCollection;
            var elements = node.Element(XName.Get("StrokeCollection"));
            if (elements != null) {
                foreach (var strokeElement in elements.Elements()) {
                    var stylusPointCollection = new StylusPointCollection();
                    var drawingAttributes = new DrawingAttributes();

                    var fitToCurveValue = strokeElement.Attribute(XName.Get("FitToCurve"))?.Value;
                    if (fitToCurveValue != null) {
                        drawingAttributes.FitToCurve = bool.Parse(fitToCurveValue);
                    }
                    var heightValue = strokeElement.Attribute(XName.Get("Height"))?.Value;
                    if (heightValue != null) {
                        drawingAttributes.Height = double.Parse(heightValue, format);
                    }
                    var widthValue = strokeElement.Attribute(XName.Get("Width"))?.Value;
                    if (widthValue != null) {
                        drawingAttributes.Width = double.Parse(widthValue, format);
                    }
                    var ignorePressureValue = strokeElement.Attribute(XName.Get("IgnorePressure"))?.Value;
                    if (ignorePressureValue != null) {
                        drawingAttributes.IgnorePressure = bool.Parse(ignorePressureValue);
                    }
                    var isHighlighterValue = strokeElement.Attribute(XName.Get("IsHighlighter"))?.Value;
                    if (isHighlighterValue != null) {
                        drawingAttributes.IsHighlighter = bool.Parse(isHighlighterValue);
                    }
                    var stylusTipValue = strokeElement.Attribute(XName.Get("StylusTip"))?.Value;
                    if (stylusTipValue != null) {
                        drawingAttributes.StylusTip = (StylusTip)Enum.Parse(typeof(StylusTip), stylusTipValue);
                    }
                    var colorNode = strokeElement.Element(XName.Get("DrawingColor"));
                    if (colorNode != null) {
                        var color = (Color?)_colorSerializer.Deserialize(colorNode, typeof(Color), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                        if (color != null) {
                            drawingAttributes.Color = color.Value;
                        }
                    }
                    var stylusTipTransformNode = strokeElement.Element(XName.Get("DrawingStylusTipTransform"));
                    if (stylusTipTransformNode != null) {
                        var matrix = (Matrix?)_matrixSerializer.Deserialize(stylusTipTransformNode, typeof(Matrix), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                        if (matrix != null) {
                            drawingAttributes.StylusTipTransform = matrix.Value;
                        }
                    }

                    var stylusPointsElement = strokeElement.Elements().FirstOrDefault(s => s.Name == XName.Get("StylusPoints"));
                    if (stylusPointsElement != null) {
                        foreach (var stylusPointElement in stylusPointsElement.Elements()) {
                            var x = double.Parse(stylusPointElement.Attribute(XName.Get("X"))?.Value ?? "0", format);
                            var y = double.Parse(stylusPointElement.Attribute(XName.Get("Y"))?.Value ?? "0", format);
                            var pressureFactor = float.Parse(stylusPointElement.Attribute(XName.Get("PressureFactor"))?.Value ?? "0", format);
                            stylusPointCollection.Add(new StylusPoint(x, y, pressureFactor));
                        }
                    }
                    coll.Add(new Stroke(stylusPointCollection, drawingAttributes));
                }
            }
            return coll;
        }

        /// <inheritdoc/>
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is StrokeCollection collection) {
                writer.WriteStartElement("StrokeCollection");
                foreach (Stroke stroke in collection) {
                    writer.WriteStartElement("Stroke");
                    writer.WriteAttributeString("FitToCurve", stroke.DrawingAttributes.FitToCurve.ToString(format));
                    writer.WriteAttributeString("Height", stroke.DrawingAttributes.Height.ToString(format));
                    writer.WriteAttributeString("Width", stroke.DrawingAttributes.Width.ToString(format));
                    writer.WriteAttributeString("IgnorePressure", stroke.DrawingAttributes.IgnorePressure.ToString(format));
                    writer.WriteAttributeString("IsHighlighter", stroke.DrawingAttributes.IsHighlighter.ToString(format));
                    writer.WriteAttributeString("StylusTip", stroke.DrawingAttributes.StylusTip.ToString());

                    writer.WriteStartElement("DrawingColor");
                    _colorSerializer.Serialize(writer, stroke.DrawingAttributes.Color, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
                    writer.WriteEndElement();

                    writer.WriteStartElement("DrawingStylusTipTransform");
                    _matrixSerializer.Serialize(writer, stroke.DrawingAttributes.StylusTipTransform, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
                    writer.WriteEndElement();

                    writer.WriteStartElement("StylusPoints");
                    foreach (StylusPoint stylusPoint in stroke.StylusPoints) {
                        writer.WriteStartElement("StylusPoint");
                        writer.WriteAttributeString("PressureFactor", stylusPoint.PressureFactor.ToString(format));
                        writer.WriteAttributeString("X", stylusPoint.X.ToString(format));
                        writer.WriteAttributeString("Y", stylusPoint.Y.ToString(format));
                        writer.WriteEndElement(); // StylusPoint
                    }
                    writer.WriteEndElement(); // StylusPoints

                    writer.WriteEndElement(); // Stroke
                }
                writer.WriteEndElement(); // StrokeCollection
                return true;

            }
            return false;
        }

        /// <inheritdoc/>
        public bool SupportsType(Type type, string? dataFormat) {
            return type.IsAssignableTo(typeof(StrokeCollection));
        }

    }
}