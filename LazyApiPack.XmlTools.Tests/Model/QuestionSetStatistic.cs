using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model {
    [XmlClass]
    public class QuestionSetStatistic : XmlSerializableTestBase {
        private DateTime _examTaken;
        [XmlAttribute]
        public DateTime ExamTaken
        {
            get => _examTaken;
            set => SetPropertyValue(ref _examTaken, value);
        }

        private TimeSpan _examDuration;
        [XmlAttribute]
        public TimeSpan ExamDuration
        {
            get => _examDuration;
            set => SetPropertyValue(ref _examDuration, value);
        }

        private int _correctAnswers;
        [XmlAttribute]
        public int CorrectAnswers
        {
            get => _correctAnswers;
            set => SetPropertyValue(ref _correctAnswers, value);
        }

        private int _incorrectAnswers;
        [XmlAttribute]
        public int IncorrectAnswers
        {
            get => _incorrectAnswers;
            set => SetPropertyValue(ref _incorrectAnswers, value);
        }
    }
}
