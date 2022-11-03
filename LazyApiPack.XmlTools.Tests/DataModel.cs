using LazyApiPack.XmlTools.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests {
    [XmlClass]
    public class QuestionCatalog : IExtendedXmlSerializable {
        [XmlArray("Sets")]
        [XmlArrayItem("Set")]
        public ObservableCollection<QuestionSet> Sets { get; set; } = new ObservableCollection<QuestionSet>();

        [XmlProperty("SingleSet")]
        public QuestionSet SingleSet { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        public void Save(string fileName) {
            new ExtendedXmlSerializer<QuestionCatalog>() { UseFullNamespace = false }
                .Serialize(this, fileName);
        }

        public static QuestionCatalog? Load(string fileName) {
            return new ExtendedXmlSerializer<QuestionCatalog>() { UseFullNamespace = false }
                .Deserialize(fileName);
        }

        public void OnDeserializing() {

        }

        public void OnDeserialized(bool success) {

        }

        public void OnSerializing() {

        }

        public void OnSerialized(bool success) {

        }

        internal void SetSingle(QuestionSet setn) {
            SingleSet = setn;
        }

        internal QuestionSet InternalSet { get => SingleSet; }
    }

    [XmlClass]
    public class QuestionSet {
        public QuestionSet() {

        }

        public QuestionSet(string name) : this() {
            Id = Guid.NewGuid();
            Name = name;
        }
        [XmlClassKey]
        public Guid Id { get; set; }
        [XmlAttribute]
        public string Name { get; set; } = "Question Set";
        [XmlArray("Questions")]
        [XmlArrayItem("Question")]
        public ObservableCollection<IQuestion> Questions { get; set; } = new ObservableCollection<IQuestion>();
        [XmlArray("Statistics")]
        [XmlArrayItem("Statistic")]
        public ObservableCollection<QuestionSetStatistic> Statistics { get; set; } = new ObservableCollection<QuestionSetStatistic>();

    }
    [XmlClass]
    public class QuestionSetStatistic {
        [XmlAttribute]
        public DateTime ExamTaken { get; set; }
        [XmlAttribute]
        public TimeSpan ExamDuration { get; set; }
        [XmlAttribute]
        public int CorrectAnswers { get; set; }
        [XmlAttribute]
        public int IncorrectAnswers { get; set; }
    }


    [XmlClass]
    public class MultipleChoiceQuestion : IQuestion {
        public MultipleChoiceQuestion() {

        }
        public MultipleChoiceQuestion(string question, string description) {
            Id = Guid.NewGuid();
            Question = question;
            Description = description;
        }
        [XmlClassKey]
        public Guid Id { get; set; }
        [XmlAttribute]
        public string Question { get; set; }
        [XmlAttribute]
        public string? Description { get; set; }
        [XmlArray("Answers")]
        [XmlArrayItem("Answer")]
        public ObservableCollection<IAnswer> Answers { get; set; } = new ObservableCollection<IAnswer>(); // MultipleChoiceAnswer

    }
    [XmlClass]
    public class MultipleChoiceAnswer : IAnswer {
        public MultipleChoiceAnswer() {

        }
        public MultipleChoiceAnswer(string answer, bool isCorrect) : this() {
            Answer = answer;
            IsCorrect = isCorrect;
        }
        [XmlAttribute]
        public bool IsCorrect { get; set; }
        [XmlAttribute]
        public string Answer { get; set; }
    }
    [XmlClass]
    public class VocabularyQuestion : IQuestion {
        public VocabularyQuestion() {

        }
        public VocabularyQuestion(string question, string description) {
            Id = Guid.NewGuid();
            Question = question;
            Description = description;
        }
        [XmlClassKey]
        public Guid Id { get; set; }
        [XmlAttribute]
        public string Question { get; set; }
        [XmlAttribute]
        public string? Description { get; set; }
        [XmlArray("Answer")]
        [XmlArrayItem("Answers")]
        public ObservableCollection<IAnswer> Answers { get; set; } = new ObservableCollection<IAnswer>(); // VocabularyAnswer
    }
    [XmlClass]
    public class VocabularyAnswer : IAnswer {
        public VocabularyAnswer() {

        }

        public VocabularyAnswer(string answer, bool isCaseSensitive = false) : this() {
            IsCorrect = true;
            Answer = answer;
            IsCaseSensitive = isCaseSensitive;
        }
        [XmlAttribute]
        public bool IsCorrect { get; set; }
        [XmlAttribute]
        public string Answer { get; set; }
        [XmlAttribute]
        public bool IsCaseSensitive { get; set; }

    }
    [XmlClass]
    public class PictureQuestion : IQuestion {
        public PictureQuestion() {

        }
        public PictureQuestion(string question, string description) {
            Id = Guid.NewGuid();
            Question = question;
            Description= description;
        }
        [XmlClassKey]
        public Guid Id { get; set; }
        [XmlAttribute]
        public string Question { get; set; }
        [XmlAttribute]
        public string? Description { get; set; }
        BitmapImage? _image;
        public BitmapImage? Image
        {
            get
            {
                if (_image != null) return _image;
                if (ImageData != null && ImageData.Length > 0) {
                    try {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(ImageData);
                        bitmap.EndInit();
                        return _image = bitmap;
                    } catch {
                        return null;
                    }
                }
                return null;

            }
        }
        byte[]? _imageData;
        [XmlProperty]
        public byte[]? ImageData
        {
            get { return _imageData; }
            set
            {
                _image = null;
                _imageData = value;
            }
        }
        [XmlAttribute]
        public PictureQuestionMode Mode { get; set; }
        [XmlArray("Answer")]
        [XmlArrayItem("Answers")]
        public ObservableCollection<IAnswer> Answers { get; set; } = new ObservableCollection<IAnswer>(); // PictureAnswer
    }


    [XmlClass]
    public class PictureAnswer : IAnswer {
        public PictureAnswer() {

        }
        public PictureAnswer(string answer, bool isCaseSensitive, Point labelLocation, Size labelSize, params PictureAlternativeAnswer[] alternativeAnswers) : this() {
            Answer = answer;
            IsCaseSensitive = isCaseSensitive;
            LabelLocation = labelLocation;
            LabelSize = labelSize;
            AlternativeAnswers = new ObservableCollection<PictureAlternativeAnswer>(alternativeAnswers);
            IsCorrect = true;
        }
        [XmlAttribute]
        public bool IsCorrect { get; set; }
        [XmlAttribute]
        public string Answer { get; set; }
        [XmlAttribute]
        public bool IsCaseSensitive { get; set; }
        [XmlAttribute]
        public Point LabelLocation { get; set; }
        [XmlAttribute]
        public Size LabelSize { get; set; }
        [XmlArray("AlternativeAnswers")]
        [XmlArrayItem("AlternativeAnswers")]
        public ObservableCollection<PictureAlternativeAnswer> AlternativeAnswers { get; set; } = new ObservableCollection<PictureAlternativeAnswer>();
    }
    [XmlClass]
    public class PictureAlternativeAnswer : IAnswer {
        public PictureAlternativeAnswer() {

        }
        public PictureAlternativeAnswer(string answer, bool isCaseSensitive = false) : this() {
            Answer = answer;
            IsCaseSensitive = isCaseSensitive;
            IsCorrect = true;
        }
        [XmlAttribute]
        public bool IsCorrect { get; set; }
        [XmlAttribute]
        public string Answer { get; set; }
        [XmlAttribute]
        public bool IsCaseSensitive { get; set; }
    }
    public interface IQuestion {
        Guid Id { get; set; }
        string Question { get; set; }
        string? Description { get; set; }
        ObservableCollection<IAnswer> Answers { get; set; }
    }

    public interface IAnswer {
        bool IsCorrect { get; set; }
        string Answer { get; set; }
    }
    public enum PictureQuestionMode {
        EnterText,
        MoveLabels
    }
}
