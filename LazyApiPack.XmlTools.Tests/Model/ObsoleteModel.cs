using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model.SimpleModelV1 {
    [XmlClass]
    public class ObsoleteModel : XmlSerializableTestBase {
        public ObsoleteModel() {

        }

        [XmlProperty("value")]
        [XmlObsolete("Use Value instead.")]
        public string OldValue
        {
            set => Value =  int.Parse(value);
        }

        private int _value = 0;
        [XmlProperty("intValue")]
        public int Value
        {
            get => _value;
            set => SetPropertyValue(ref _value, value);
        }
    }

}
