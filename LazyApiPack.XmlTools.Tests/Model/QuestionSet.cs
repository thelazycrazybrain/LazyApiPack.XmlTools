using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model {
    [XmlClass]
    public class QuestionSet : XmlSerializableTestBase {
        public QuestionSet() : base() { }

        public QuestionSet(string name) : this() {
            Id = Guid.NewGuid();
            Name = name;
        }

        private Guid _id;
        [XmlClassKey]
        public Guid Id
        {
            get => _id;
            set => SetPropertyValue(ref _id, value);
        }

        private string _name = "Question Set";
        [XmlAttribute]
        public string Name
        {
            get => _name;
            set => SetPropertyValue(ref _name, value);
        }

        private ObservableCollection<IQuestion> _questions = new ObservableCollection<IQuestion>();
        [XmlArray("Questions")]
        [XmlArrayItem("Question")]
        public ObservableCollection<IQuestion> Questions
        {
            get => _questions;
            set => SetPropertyValue(ref _questions, value);
        }

        private ObservableCollection<QuestionSetStatistic> _statistics = new ObservableCollection<QuestionSetStatistic>();
        [XmlArray("Statistics")]
        [XmlArrayItem("Statistic")]
        public ObservableCollection<QuestionSetStatistic> Statistics
        {
            get => _statistics;
            set => SetPropertyValue(ref _statistics, value);
        }

    }
}
