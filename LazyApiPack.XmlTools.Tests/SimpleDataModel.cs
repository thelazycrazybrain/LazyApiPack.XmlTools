using LazyApiPack.XmlTools.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests {
    [XmlClass("simpleDataModel")]
    public class SimpleDataModel {

        [XmlAttribute("location")]
        public Point Location
        {
            get; set;
        }
        [XmlAttribute("padding")]
        public Thickness Padding { get; set; }

        [XmlAttribute("version")]
        public Version Version { get; set; }
    }
}
