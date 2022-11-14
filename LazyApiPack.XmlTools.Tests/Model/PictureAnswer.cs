using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model {
    [XmlClass]
    public class PictureAnswer : XmlSerializableTestBase, IAnswer {
        public PictureAnswer() : base() { }
        public PictureAnswer(string answer, bool isCaseSensitive, Point labelLocation, Size labelSize,
                             params PictureAlternativeAnswer[] alternativeAnswers) : this() {
            Answer = answer;
            IsCaseSensitive = isCaseSensitive;
            LabelLocation = labelLocation;
            LabelSize = labelSize;
            AlternativeAnswers = new ObservableCollection<PictureAlternativeAnswer>(alternativeAnswers);
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

        private Point _labelLocation;
        [XmlAttribute]
        public Point LabelLocation
        {
            get => _labelLocation;
            set => SetPropertyValue(ref _labelLocation, value);
        }

        private Size _labelSize;
        [XmlAttribute]
        public Size LabelSize
        {
            get => _labelSize;
            set => SetPropertyValue(ref _labelSize, value);
        }

        private ObservableCollection<PictureAlternativeAnswer> _alternativeAnswers
                            = new ObservableCollection<PictureAlternativeAnswer>();
        [XmlArray("AlternativeAnswers")]
        [XmlArrayItem("AlternativeAnswers")]
        public ObservableCollection<PictureAlternativeAnswer> AlternativeAnswers
        {
            get => _alternativeAnswers;
            set => SetPropertyValue(ref _alternativeAnswers, value);
        }

    }
}
