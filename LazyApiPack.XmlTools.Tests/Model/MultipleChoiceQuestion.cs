using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model {
    [XmlClass]
    public class MultipleChoiceQuestion : XmlSerializableTestBase, IQuestion {
        public MultipleChoiceQuestion() : base() { }
        public MultipleChoiceQuestion(string question, string description) : this() {
            Id = Guid.NewGuid();
            Question = question;
            Description = description;
        }

        private Guid _id;
        [XmlClassKey]
        public Guid Id
        {
            get => _id;
            set => SetPropertyValue(ref _id, value);
        }

        private string? _question;
        [XmlAttribute]
        public string? Question
        {
            get => _question;
            set => SetPropertyValue(ref _question, value);
        }

        private string? _description;
        [XmlAttribute]
        public string? Description
        {
            get => _description;
            set => SetPropertyValue(ref _description, value);
        }

        private ObservableCollection<IAnswer> _answers = new ObservableCollection<IAnswer>();
        [XmlArray("Answers")]
        [XmlArrayItem("Answer")]
        public ObservableCollection<IAnswer> Answers // MultipleChoiceAnswer
        {
            get => _answers;
            set => SetPropertyValue(ref _answers, value);
        }

    }
}
