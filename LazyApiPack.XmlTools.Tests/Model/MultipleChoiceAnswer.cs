using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model {
    [XmlClass]
    public class MultipleChoiceAnswer : XmlSerializableTestBase, IAnswer {
        public MultipleChoiceAnswer() : base() { }
        public MultipleChoiceAnswer(string answer, bool isCorrect) : this() {
            Answer = answer;
            IsCorrect = isCorrect;
        }

        private bool _isCorrect;
        [XmlAttribute]
        public bool IsCorrect
        {
            get => _isCorrect;
            set => SetPropertyValue(ref _isCorrect, value);
        }

        private string? _answer;
        [XmlAttribute]
        public string? Answer
        {
            get => _answer;
            set => SetPropertyValue(ref _answer, value);
        }
    }
}
