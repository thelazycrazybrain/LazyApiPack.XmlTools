using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model {
    [XmlClass]
    public class PictureAlternativeAnswer : XmlSerializableTestBase, IAnswer {
        public PictureAlternativeAnswer() : base() { }
        public PictureAlternativeAnswer(string answer, bool isCaseSensitive = false) : this() {
            Answer = answer;
            IsCaseSensitive = isCaseSensitive;
            IsCorrect = true;
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
