using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model.SimpleModelV1
{
    [XmlClass]
    public class SimpleModel : XmlSerializableTestBase
    {
        public SimpleModel()
        {

        }

        public SimpleModel(string name) : this()
        {
            Name = name;
        }
        private string _name = "Simple Model V1";
        [XmlAttribute]
        public string Name
        {
            get => _name;
            set => SetPropertyValue(ref _name, value);
        }

     
    }

}
