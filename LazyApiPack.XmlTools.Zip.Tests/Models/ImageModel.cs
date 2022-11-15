using LazyApiPack.XmlTools.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Zip.Tests.Models {
    [XmlClass]
    public class ImageModel {
        List<ZipResource> _images = new List<ZipResource>();
        [XmlArray("Images")]
        [XmlArrayItem("Image")]
        public List<ZipResource> Images { get => _images; set => _images = value; }

        private ZipResource _mainImage;
        [XmlAttribute]
        public ZipResource MainImage { get => _mainImage; set => _mainImage=value; }
    }
}
