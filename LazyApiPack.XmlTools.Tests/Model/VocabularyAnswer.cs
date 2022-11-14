using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model {
    [XmlClass]
    public class VocabularyAnswer : XmlSerializableTestBase, IAnswer {
        public VocabularyAnswer() : base() { }

        public VocabularyAnswer(string answer, bool isCaseSensitive = false) : this() {
            IsCorrect = true;
            Answer = answer;
            IsCaseSensitive = isCaseSensitive;
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

        private bool _isCaseSensitive;
        [XmlAttribute]
        public bool IsCaseSensitive
        {
            get => _isCaseSensitive;
            set => SetPropertyValue(ref _isCaseSensitive, value);
        }

    }
}
