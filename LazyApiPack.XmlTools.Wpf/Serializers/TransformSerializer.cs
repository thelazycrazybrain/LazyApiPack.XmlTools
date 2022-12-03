using LazyApiPack.XmlTools.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class TransformSerializer : IExternalObjectSerializer {
        private readonly MatrixSerializer _matrixSerializer;
        public TransformSerializer([NotNull] MatrixSerializer matrixSerializer) {
            _matrixSerializer = matrixSerializer;
        }
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var transformType = node.Attribute(XName.Get("TransformType"))?.Value;
            switch (transformType) {
                case "MatrixTransform":
                    return DeserializeMatrixTransform(node, format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                case "RotateTransform":
                    return DeserializeRotateTransform(node, format);
                case "ScaleTransform":
                    return DeserializeScaleTransform(node, format);
                case "SkewTransform":
                    return DeserializeSkewTransform(node, format);
                case "TranslateTransform":
                    return DeserializeTranslateTransform(node, format);
                default:
                    throw new NotSupportedException($"The Transform {format} cannot be deserialized");
            }
        }

        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is Transform transform) {
                if (transform is MatrixTransform mxt) {
                    SerializeMatrixTransform(writer, mxt, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
                } else if (transform is RotateTransform rt) {
                    SerializeRotateTransform(writer, rt, format);
                } else if (transform is ScaleTransform stt) {
                    SerializeScaleTransform(writer, stt, format);
                } else if (transform is SkewTransform skt) {
                    SerializeSkewTransform(writer, skt, format);
                } else if (transform is TranslateTransform tt) {
                    SerializeTranslateTransform(writer, tt, format);
                } else {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool SupportsType(Type type, string? dataFormat) {
            return type.IsAssignableTo(typeof(RotateTransform));
        }


        public void SerializeMatrixTransform(XmlWriter writer, MatrixTransform matrixTransform, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            writer.WriteAttributeString("TransformType", "MatrixTransform");
            writer.WriteStartElement("Matrix");
            _matrixSerializer.Serialize(writer, matrixTransform.Matrix, serializeAsAttribute, format, dateTimeFormat, enableRecursiveSerialization, setDataFormat);
            writer.WriteEndElement(); // Matrix

        }

        public MatrixTransform DeserializeMatrixTransform(XElement node, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var matrixNode = node.Element(XName.Get("Matrix"));
            if (matrixNode != null) {
                var matrix = (Matrix?)_matrixSerializer.Deserialize(matrixNode, typeof(Matrix), format, dateTimeFormat, enableRecursiveSerialization, dataFormat);
                if (matrix != null) {
                    return new MatrixTransform(matrix.Value);
                }
            }
            return new MatrixTransform();

        }

        public void SerializeRotateTransform(XmlWriter writer, RotateTransform rotateTransform, IFormatProvider format) {
            writer.WriteAttributeString("TransformType", "RotateTransform");
            writer.WriteAttributeString("CenterX", rotateTransform.CenterX.ToString(format));
            writer.WriteAttributeString("CenterY", rotateTransform.CenterY.ToString(format));
            writer.WriteAttributeString("Angle", rotateTransform.Angle.ToString(format));
        }

        public RotateTransform DeserializeRotateTransform(XElement objectNode, IFormatProvider format) {
            return new RotateTransform(
                double.Parse(objectNode.Attribute(XName.Get("Angle"))?.Value ?? "0", format),
                double.Parse(objectNode.Attribute(XName.Get("CenterX"))?.Value ?? "0", format),
                double.Parse(objectNode.Attribute(XName.Get("CenterY"))?.Value ?? "0", format));
        }

        public void SerializeScaleTransform(XmlWriter writer, ScaleTransform scaleTransform, IFormatProvider format) {
            writer.WriteAttributeString("TransformType", "ScaleTransform");
            writer.WriteAttributeString("CenterX", scaleTransform.CenterX.ToString(format));
            writer.WriteAttributeString("CenterY", scaleTransform.CenterY.ToString(format));
            writer.WriteAttributeString("ScaleX", scaleTransform.ScaleX.ToString(format));
            writer.WriteAttributeString("ScaleY", scaleTransform.ScaleY.ToString(format));
        }

        public ScaleTransform DeserializeScaleTransform(XElement objectNode, IFormatProvider format) {
            return new ScaleTransform(double.Parse(objectNode.Attribute(XName.Get("ScaleX"))?.Value ?? "0", format),
                double.Parse(objectNode.Attribute(XName.Get("ScaleY"))?.Value ?? "0", format),
                double.Parse(objectNode.Attribute(XName.Get("CenterX"))?.Value ?? "0", format),
                double.Parse(objectNode.Attribute(XName.Get("CenterY"))?.Value ?? "0", format));
        }

        public void SerializeSkewTransform(XmlWriter writer, SkewTransform skewTransform, IFormatProvider format) {
            writer.WriteAttributeString("TransformType", "SkewTransform");
            writer.WriteAttributeString("AngleX", skewTransform.AngleX.ToString(format));
            writer.WriteAttributeString("AngleY", skewTransform.AngleY.ToString(format));
            writer.WriteAttributeString("CenterX", skewTransform.CenterX.ToString(format));
            writer.WriteAttributeString("CenterY", skewTransform.CenterY.ToString(format));
        }

        public SkewTransform DeserializeSkewTransform(XElement objectNode, IFormatProvider format) {
            return new SkewTransform(
                double.Parse(objectNode.Attribute(XName.Get("AngleX"))?.Value ?? "0", format),
                double.Parse(objectNode.Attribute(XName.Get("AngleY"))?.Value ?? "0", format),
                double.Parse(objectNode.Attribute(XName.Get("CenterX"))?.Value ?? "0", format),
                double.Parse(objectNode.Attribute(XName.Get("CenterY"))?.Value ?? "0", format));
        }

        public void SerializeTranslateTransform(XmlWriter writer, TranslateTransform translateTransform, IFormatProvider format) {
            writer.WriteAttributeString("TransformType", "TranslateTransform");
            writer.WriteAttributeString("X", translateTransform.X.ToString(format));
            writer.WriteAttributeString("Y", translateTransform.Y.ToString(format));
        }

        public TranslateTransform DeserializeTranslateTransform(XElement objectNode, IFormatProvider format) {
            return new TranslateTransform(
                double.Parse(objectNode.Attribute(XName.Get("X"))?.Value ?? "0", format),
                double.Parse(objectNode.Attribute(XName.Get("Y"))?.Value ?? "0", format));
        }
    }
}