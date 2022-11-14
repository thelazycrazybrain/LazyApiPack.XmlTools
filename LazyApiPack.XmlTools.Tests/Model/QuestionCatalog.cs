using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model {
    [XmlClass]
    public class QuestionCatalog : ExtendedXmlSerializableTestBase {
        public QuestionCatalog() : base() { }

        private string? _name;
        [XmlAttribute]
        public string? Name
        {
            get => _name;
            set => SetPropertyValue(ref _name, value);
        }

        private ObservableCollection<QuestionSet> _questions = new ObservableCollection<QuestionSet>();
        [XmlArray("Sets")]
        [XmlArrayItem("Set")]
        public ObservableCollection<QuestionSet> Sets
        {
            get => _questions;
            set => SetPropertyValue(ref _questions, value);
        }

    }
}
