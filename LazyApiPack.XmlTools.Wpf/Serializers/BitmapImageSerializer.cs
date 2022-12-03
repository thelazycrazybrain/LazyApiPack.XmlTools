using LazyApiPack.XmlTools.Helpers;
using System.IO;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf.Serializers {
    public class BitmapImageSerializer : IExternalObjectSerializer {
        /// <inheritdoc />
        public object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
            var image = new BitmapImage();
            image.DecodePixelWidth = int.Parse(node.Attribute(XName.Get("DecodePixelWidth"))?.Value ?? "0", format);
            image.DecodePixelHeight = int.Parse(node.Attribute(XName.Get("DecodePixelHeight"))?.Value ?? "0", format);
            var contentNode = node.Element(XName.Get("Content"));
            if (string.IsNullOrWhiteSpace(contentNode?.Value)) {
                return image;
            }
            var data = Convert.FromBase64String(contentNode.Value);

            using (var ms = new MemoryStream(data)) {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
            }
            return image;
        }
        /// <inheritdoc />
        public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
            if (value == null) return true;
            if (value is BitmapImage bitmapImage) {
                var image = new PngBitmapEncoder();
                writer.WriteAttributeString("ImageSourceType", "BitmapImage");
                writer.WriteAttributeString("DecodePixelWidth", bitmapImage.DecodePixelWidth.ToString(format));
                writer.WriteAttributeString("DecodePixelHeight", bitmapImage.DecodePixelHeight.ToString(format));
                writer.WriteAttributeString("Rotation", bitmapImage.Rotation.ToString());

                writer.WriteStartElement("Content");
                using (var ms = new MemoryStream()) {
                    image.Frames.Add(BitmapFrame.Create(bitmapImage));
                    image.Save(ms);
                    writer.WriteValue(Convert.ToBase64String(ms.GetBuffer()));
                }
                writer.WriteEndElement();
            }
            return true;
        }
        /// <inheritdoc />
        public bool SupportsType(Type type, string? dataFormat) {
            return type == typeof(BitmapImage);
        }


    }
}