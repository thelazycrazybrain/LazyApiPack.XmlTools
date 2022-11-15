using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model {
    [XmlClass]
    public class PictureQuestion : XmlSerializableTestBase, IQuestion {
        public PictureQuestion() : base() { }
        public PictureQuestion(string question, string? description) : this() {
            Id = Guid.NewGuid();
            Question = question;
            Description= description;
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


        private Stream? _imageStream;
        public Stream? ImageStream
        {
            get => _imageStream ??= new MemoryStream(ImageData ?? Array.Empty<byte>());
            set
            {
                if (value == null) {
                    _imageData = null;
                } else {
                    var pos = value.Position;
                    var buff = new byte[value.Length - value.Position];
                    value.Read(buff, 0, buff.Length);
                    _imageData = buff;
                    if (value.CanSeek) {
                        value.Position = pos;
                    }
                }
                SetPropertyValue(ref _imageStream, value);
            }
        }

        private byte[]? _imageData;
        [XmlProperty]
        public byte[]? ImageData
        {
            get => _imageData;
            set
            {
                _imageStream = null;
                SetPropertyValue(ref _imageData, value);
            }
        }

        private PictureQuestionMode _mode;
        [XmlAttribute]
        public PictureQuestionMode Mode
        {
            get => _mode;
            set => SetPropertyValue(ref _mode, value);
        }

        private ObservableCollection<IAnswer> _answers = new ObservableCollection<IAnswer>(); // PictureAnswer
        [XmlArray("Answer")]
        [XmlArrayItem("Answers")]
        public ObservableCollection<IAnswer> Answers
        {
            get => _answers;
            set => SetPropertyValue(ref _answers, value);
        }
    }
}
