using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model.SimpleModelV2 {
    [XmlClass]
    public class SimpleModel : XmlSerializableTestBase {
        public SimpleModel() {

        }

        public SimpleModel(string caption) : this() {
            Caption = caption;
        }
        private string _caption = "Simple Model V2";
        [XmlAttribute]
        public string Caption
        {
            get => _caption;
            set => SetPropertyValue(ref _caption, value);
        }

    }
}
