using LazyApiPack.XmlTools.Attributes;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Zip.Tests.Models {
    [XmlClass]
    public class ImageModelByte {
        List<byte[]> _images = new List<byte[]>();
        [XmlArray("Images")]
        [XmlArrayItem("Image")]
        public List<byte[]> Images { get => _images; set => _images = value; }

        private byte[] _mainImage;
        [XmlProperty]
        public byte[] MainImage { get => _mainImage; set => _mainImage=value; }
    }
}
