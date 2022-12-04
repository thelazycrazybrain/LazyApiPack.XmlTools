using LazyApiPack.XmlTools.Helpers;
using System.IO;
using System.Runtime.Intrinsics.X86;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class BitmapImageSerializer : IExternalObjectSerializer {
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var image = new BitmapImage();
            var sourceType = node.Attribute(XName.Get("sourceType"))?.Value;
            if (sourceType== null) {
                return image;
            }

            var sourceValue = node.Attribute(XName.Get("source"))?.Value;
            if (sourceValue == null) {
                return image;
            }

            if (sourceType == "uri") {
                image.BeginInit();
                image.UriSource = new Uri(sourceValue);
                image.EndInit();
            } else if (sourceType == "stream") {
                var sourceFormat = node.Attribute(XName.Get("sourceFormat"))?.Value;
                if (sourceFormat == null) {
                    throw new InvalidOperationException("Attribute sourceFormat of BitmapImage is missing.");
                }

                if (sourceFormat == "base64") {
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(Convert.FromBase64String(sourceValue));
                    image.EndInit();
                } else {
                    throw new NotSupportedException($"sourceFormat {sourceFormat} is not supported of BitmapImage.");

                }
            }

            return image;
        }
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is BitmapImage bitmapImage) {
                if (bitmapImage.UriSource != null) {
                    writer.WriteAttributeString("sourceType", "uri");
                    writer.WriteAttributeString("source", bitmapImage.UriSource.ToString());
                } else if (bitmapImage.StreamSource != null) {
                    if (!bitmapImage.StreamSource.CanSeek) {
                        throw new InvalidOperationException($"Can not set position on {nameof(bitmapImage.StreamSource)} of {nameof(BitmapImage)} because it is not seekable.");
                    }

                    if (!bitmapImage.StreamSource.CanRead) {
                        throw new InvalidOperationException($"Can not read {nameof(bitmapImage.StreamSource)} of {nameof(BitmapImage)} because it is set as WriteOnly.");
                    }
                    writer.WriteAttributeString("sourceType", "stream");
                    writer.WriteAttributeString("sourceFormat", "base64");

                    var oldPos = bitmapImage.StreamSource.Position;
                    bitmapImage.StreamSource.Position = 0;
                    var buffer = new byte[bitmapImage.StreamSource.Length];
                    bitmapImage.StreamSource.Read(buffer, 0, buffer.Length);
                    bitmapImage.StreamSource.Position = oldPos;
                    writer.WriteAttributeString("source", Convert.ToBase64String(buffer));
                }
            }
            return true;
        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(BitmapImage);
        }


    }
}