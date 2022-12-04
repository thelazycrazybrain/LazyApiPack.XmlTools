using LazyApiPack.XmlTools.Helpers;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class BrushSerializer : IExternalObjectSerializer {
        private readonly ColorSerializer _colorSerializer;
        private readonly TransformSerializer _transformSerializer;
        private readonly RectSerializer _rectSerializer;
        private readonly BitmapImageSerializer _bitmapImageSerializer;

        public BrushSerializer([NotNull] ColorSerializer colorSerializer,
                               [NotNull] TransformSerializer transformSerializer,
                               [NotNull] RectSerializer rectSerializer,
                               [NotNull] BitmapImageSerializer bitmapImageSerializer) {
            _colorSerializer = colorSerializer;
            _transformSerializer = transformSerializer;
            _rectSerializer = rectSerializer;
            _bitmapImageSerializer = bitmapImageSerializer;

        }

        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var brushType = node.Attribute(XName.Get("BrushType"))?.Value;
            if (brushType == null) return null;
            switch (brushType) {
                case "SolidColorBrush":
                    return DeserializeSolidColorBrush(node, format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                case "RadialGradientBrush":
                    return DeserializeRadialGradientBrush(node, format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                case "LinearGradientBrush":
                    return DeserializeLinearGradientBrush(node, format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                case "ImageBrush":
                    return DeserializeImageBrush(node, format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                default:
                    throw new NotSupportedException("Brush {type} is not supported.");
            }

        }

        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is Brush brush) {
                if (brush is SolidColorBrush scb) {
                    SerializeSolidColorBrush(writer, scb, format);
                } else if (brush is RadialGradientBrush rgb) {
                    SerializeRadialGradientBrush(writer, rgb, format);
                } else if (brush is LinearGradientBrush lgb) {
                    SerializeLinearGradientBrush(writer, lgb, format);
                } else if (brush is ImageBrush ib) {
                    SerializeImageBrush(writer, ib, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
                } else {
                    return false;
                }
                return true;
            }
            return false;

        }

        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type.IsAssignableTo(typeof(Brush));
        }

        public void SerializeImageBrush(XmlWriter writer, ImageBrush imageBrush, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {

            writer.WriteAttributeString("BrushType", "ImageBrush");
            writer.WriteAttributeString("AlignmentX", imageBrush.AlignmentX.ToString());
            writer.WriteAttributeString("AlignmentY", imageBrush.AlignmentY.ToString());
            writer.WriteAttributeString("Opacity", imageBrush.Opacity.ToString(format));
            writer.WriteAttributeString("Stretch", imageBrush.Stretch.ToString());
            writer.WriteAttributeString("ViewPortUnits", imageBrush.ViewportUnits.ToString());
            writer.WriteAttributeString("TileMode", imageBrush.TileMode.ToString());

            writer.WriteStartElement("RelativeTransform");
            _transformSerializer.Serialize(writer, imageBrush.RelativeTransform, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
            writer.WriteEndElement(); // RelativeTransform

            writer.WriteStartElement("Transform");
            _transformSerializer.Serialize(writer, imageBrush.Transform, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
            writer.WriteEndElement(); // Transform

            writer.WriteStartElement("Viewport");
            _rectSerializer.Serialize(writer, imageBrush.Viewport, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
            writer.WriteEndElement(); // Viewport

            writer.WriteStartElement("Viewbox");
            _rectSerializer.Serialize(writer, imageBrush.Viewbox, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
            writer.WriteEndElement(); // Viewbox

            writer.WriteStartElement("BitmapImage");
            _bitmapImageSerializer.Serialize(writer, imageBrush.ImageSource, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
            writer.WriteEndElement(); // BitmapImage

        }

        public ImageBrush DeserializeImageBrush(XElement node, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {

            var imageNode = node.Element(XName.Get("BitmapImage"));


            ImageBrush brush;
            BitmapImage? imageSource = null;
            if (imageNode != null) {
                imageSource = (BitmapImage?)_bitmapImageSerializer.Deserialize(imageNode, typeof(BitmapImage), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
            }

            if (imageSource != null) {
                brush = new ImageBrush(imageSource);
            } else {
                brush = new ImageBrush();
            }



            var viewPortNode = node.Element(XName.Get("Viewport"));
            if (viewPortNode != null) {
                var viewPort = (Rect?)_rectSerializer.Deserialize(viewPortNode, typeof(Rect), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                if (viewPort != null) {
                    brush.Viewport = viewPort.Value;
                }
            }
            var viewBoxNode = node.Element(XName.Get("Viewbox"));
            if (viewBoxNode != null) {
                var viewBox = (Rect?)_rectSerializer.Deserialize(viewBoxNode, typeof(Rect), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                if (viewBox != null) {
                    brush.Viewbox = viewBox.Value;
                }
            }
            var alignmentXValue = node.Attribute(XName.Get("AlignmentX"))?.Value;
            if (alignmentXValue != null) {
                brush.AlignmentX = (AlignmentX)Enum.Parse(typeof(AlignmentX), alignmentXValue);

            }
            var alignmentYValue = node.Attribute(XName.Get("AlignmentY"))?.Value;
            if (alignmentYValue != null) {
                brush.AlignmentY = (AlignmentY)Enum.Parse(typeof(AlignmentX), alignmentYValue);
            }

            var opacityValue = node.Attribute(XName.Get("Opacity"))?.Value;
            if (opacityValue != null) {
                brush.Opacity = double.Parse(opacityValue, format);
            }

            var stretchValue = node.Attribute(XName.Get("Stretch"))?.Value;
            if (stretchValue != null) {
                brush.Stretch = (Stretch)Enum.Parse(typeof(Stretch), stretchValue);
            }

            var viewPortUnitsValue = node.Attribute(XName.Get("ViewPortUnits"))?.Value;
            if (viewPortUnitsValue != null) {
                brush.ViewportUnits = (BrushMappingMode)Enum.Parse(typeof(BrushMappingMode), viewPortUnitsValue);
            }

            var tileModeValue = node.Attribute(XName.Get("TileMode"))?.Value;
            if (tileModeValue != null) {
                brush.TileMode = (TileMode)Enum.Parse(typeof(TileMode), tileModeValue);
            }

            var relativeTransformNode = node.Element(XName.Get("RelativeTransform"));
            if (relativeTransformNode != null) {
                var relativeTransform = (Transform?)_transformSerializer.Deserialize(relativeTransformNode, typeof(Transform), format, dateTimeFormat, enableRecursiveSerialization, dataFormat); ;
                if (relativeTransform != null) {
                    brush.RelativeTransform = relativeTransform;
                }
            }

            var transformNode = node.Element(XName.Get("RelativeTransform"));
            if (transformNode != null) {
                var transform = (Transform?)_transformSerializer.Deserialize(transformNode, typeof(Transform), format, dateTimeFormat, enableRecursiveSerialization, dataFormat); ;
                if (transform != null) {
                    brush.Transform = transform;
                }
            }

            return brush;
        }

        public void SerializeLinearGradientBrush(XmlWriter writer, LinearGradientBrush lgb, IFormatProvider format) {
            writer.WriteAttributeString("BrushType", "LinearGradientBrush");
            writer.WriteStartElement("StartPoint");
            writer.WriteAttributeString("X", lgb.StartPoint.X.ToString(format));
            writer.WriteAttributeString("Y", lgb.StartPoint.Y.ToString(format));
            writer.WriteEndElement(); // StartPoint

            writer.WriteStartElement("EndPoint");
            writer.WriteAttributeString("X", lgb.EndPoint.X.ToString(format));
            writer.WriteAttributeString("Y", lgb.EndPoint.Y.ToString(format));
            writer.WriteEndElement(); // EndPoint

            writer.WriteStartElement("GradientStops");
            foreach (GradientStop gradientStop in lgb.GradientStops) {
                writer.WriteStartElement("GradientStop");
                writer.WriteAttributeString("Offset", gradientStop.Offset.ToString(format));
                writer.WriteAttributeString("A", gradientStop.Color.A.ToString(format));
                writer.WriteAttributeString("R", gradientStop.Color.R.ToString(format));
                writer.WriteAttributeString("G", gradientStop.Color.G.ToString(format));
                writer.WriteAttributeString("B", gradientStop.Color.B.ToString(format));
                writer.WriteEndElement(); // GradientStop
            }
            writer.WriteEndElement(); // GradientStops
        }

        public LinearGradientBrush DeserializeLinearGradientBrush(XElement node, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var startPointNode = node.Element(XName.Get("StartPoint"));
            var startPoint = new Point(
                    double.Parse(startPointNode?.Attribute(XName.Get("X"))?.Value ?? "0", format),
                    double.Parse(startPointNode?.Attribute(XName.Get("Y"))?.Value ?? "0", format));

            var endPointNode = node.Element(XName.Get("EndPoint"));
            var endPoint = new Point(
                    double.Parse(endPointNode?.Attribute(XName.Get("X"))?.Value ?? "0", format),
                    double.Parse(endPointNode?.Attribute(XName.Get("Y"))?.Value ?? "0", format));

            var linearGradientBrush = new LinearGradientBrush();
            var gradientStopsNode = node.Element(XName.Get("GradientStops"));

            var collection = new GradientStopCollection();
            if (gradientStopsNode != null) {
                foreach (var gsn in gradientStopsNode.Elements()) {
                    var color = (Color?)_colorSerializer.Deserialize(gsn, typeof(Color), format, dateTimeFormat, enableRecursiveSerialization, dataFormat) ?? throw new InvalidOperationException("Color of gradientstop is not set.");
                    var offset = double.Parse(gsn.Attribute(XName.Get("Offset"))?.Value ?? "0", format);
                    collection.Add(new GradientStop(color, offset));
                }
            }
            return new LinearGradientBrush(collection, startPoint, endPoint);
        }

        public void SerializeRadialGradientBrush(XmlWriter writer, RadialGradientBrush rgb, IFormatProvider format) {
            writer.WriteAttributeString("BrushType", "RadialGradientBrush");
            writer.WriteAttributeString("RadX", rgb.RadiusX.ToString(format));
            writer.WriteAttributeString("RadY", rgb.RadiusY.ToString(format));
            writer.WriteAttributeString("GOriginX", rgb.GradientOrigin.X.ToString(format));
            writer.WriteAttributeString("GOriginY", rgb.GradientOrigin.Y.ToString(format));

            writer.WriteStartElement("GradientStops");
            foreach (GradientStop gradientStop in rgb.GradientStops) {
                writer.WriteStartElement("GradientStop");
                writer.WriteAttributeString("Offset", gradientStop.Offset.ToString(format));
                writer.WriteAttributeString("A", gradientStop.Color.A.ToString(format));
                writer.WriteAttributeString("R", gradientStop.Color.R.ToString(format));
                writer.WriteAttributeString("G", gradientStop.Color.G.ToString(format));
                writer.WriteAttributeString("B", gradientStop.Color.B.ToString(format));
                writer.WriteEndElement(); // GradientStop
            }
            writer.WriteEndElement(); // GradientStops

        }

        public RadialGradientBrush DeserializeRadialGradientBrush(XElement node, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var radX = double.Parse(node.Attribute(XName.Get("RadX"))?.Value ?? "0", format);
            var radY = double.Parse(node.Attribute(XName.Get("RadY"))?.Value ?? "0", format);

            var originX = double.Parse(node.Attribute(XName.Get("GOriginX"))?.Value ?? "0", format);
            var originY = double.Parse(node.Attribute(XName.Get("GOriginY"))?.Value ?? "0", format);

            var gradientStopCollection = new GradientStopCollection();
            foreach (var gstopNode in node.Elements().Where(e => e.Name == XName.Get("GradientStop"))) {
                var color = (Color?)_colorSerializer.Deserialize(gstopNode, typeof(Color), format, dateTimeFormat, enableRecursiveSerialization, dataFormat) ?? throw new InvalidOperationException("Color of gradientstop is not set.");
                var offset = double.Parse(gstopNode.Attribute(XName.Get("Offset"))?.Value ?? "0", format);
                gradientStopCollection.Add(new GradientStop(color, offset));
            }
            return new RadialGradientBrush(gradientStopCollection) {
                GradientOrigin = new Point(originX, originY),
                RadiusX = radX,
                RadiusY = radY
            };
        }
        public void SerializeSolidColorBrush(XmlWriter writer, SolidColorBrush scb, IFormatProvider format) {
            writer.WriteAttributeString("BrushType", "SolidColorBrush");
            writer.WriteAttributeString("A", scb.Color.A.ToString(format));
            writer.WriteAttributeString("R", scb.Color.R.ToString(format));
            writer.WriteAttributeString("G", scb.Color.G.ToString(format));
            writer.WriteAttributeString("B", scb.Color.B.ToString(format));

        }

        public SolidColorBrush DeserializeSolidColorBrush(XElement node, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            return new SolidColorBrush((Color?)_colorSerializer.Deserialize(node, typeof(Color), format, dateTimeFormat, enableRecursiveSerialization, dataFormat)  ?? throw new InvalidOperationException("Color of SolidColorBrush is not set."));
        }
    }
}