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

        private QuestionCatalogScoreMode _scoreMode;
        [XmlAttribute]
        public QuestionCatalogScoreMode ScoreMode {
            get => _scoreMode;
            set => SetPropertyValue(ref _scoreMode, value);
        }
        private QuestionCatalogScoreMode _scoreModeProperty;
        [XmlProperty]
        public QuestionCatalogScoreMode ScoreModeProperty {
            get => _scoreModeProperty;
            set => SetPropertyValue(ref _scoreModeProperty, value);
        }
        private DateTime _lastLearned;
        [XmlAttribute]
        public DateTime LastLearned
        {
            get => _lastLearned;
            set => SetPropertyValue(ref _lastLearned, value);
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
    [Flags]
    public enum QuestionCatalogScoreMode : int {
        Default = 0,
        WrongAnswerGivesNegativePoints = 1,
        MultipleChoiceAnyWrongTickCountsAllWrong = 2,
        MultipleChoiceMissingTickCountsAllWrong = 4

    }
}
