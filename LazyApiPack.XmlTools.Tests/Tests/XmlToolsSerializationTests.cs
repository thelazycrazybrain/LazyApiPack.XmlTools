using LazyApiPack.XmlTools.Tests.Model;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Tests.Tests {
    public class XmlToolsSerializationTests {


        QuestionCatalog _model;
        DateTime _checkDateTime;
        [SetUp]
        public void Setup() {
            var asm = Assembly.GetExecutingAssembly();
            _model = new QuestionCatalog() {
                Name = "Question Catalog 1",
                LastLearned = _checkDateTime = DateTime.Now
            };

            var set1 = new QuestionSet("Random questions...");

            var set1q1 = new MultipleChoiceQuestion("How much wood would a woodchuck chuck, if a woodchuck could chuck wood?",
                                                    "Specify the correct answer");
            set1q1.Answers.Add(new MultipleChoiceAnswer("It would chuck no wood.", false));
            set1q1.Answers.Add(new MultipleChoiceAnswer("A woodchuck cant chuck wood.", true));
            set1q1.Answers.Add(new MultipleChoiceAnswer("No answer is correct.", true));
            set1.Questions.Add(set1q1);


            var set1q2 = new VocabularyQuestion("What does 'in front of' mean in latin?", null);
            set1q2.Answers.Add(new VocabularyAnswer("ventral", false));
            set1q2.Answers.Add(new VocabularyAnswer("ventr.", false));
            set1.Questions.Add(set1q2);


            var set1q3 = new PictureQuestion("Fill in the texts in the given image", "Do something...") {
                ImageStream = asm.GetManifestResourceStream("LazyApiPack.XmlTools.Tests.Resources.Sample1.jpg" ?? throw new NullReferenceException("Application Resource Sample2.jpg was not found.")),
                Mode = PictureQuestionMode.EnterText
            };

            set1q3.Answers.Add(new PictureAnswer("Sternum", false, new Point(0, 0),
                                                 new Size(50, 10),
                                                 new PictureAlternativeAnswer("os sternalis", false),
                                                 new PictureAlternativeAnswer("i dont know...", false)));

            set1q3.Answers.Add(new PictureAnswer("Musculus Brachialis", false, new Point(0, 50),
                                                 new Size(50, 10),
                                                 new PictureAlternativeAnswer("M. brachialis")));
            set1.Questions.Add(set1q3);

            var set1q4 = new PictureQuestion("Fill in the texts in the given image", null) {
                ImageStream = asm.GetManifestResourceStream("LazyApiPack.XmlTools.Tests.Resources.Sample2.jpg") ?? throw new NullReferenceException("Application Resource Sample2.jpg was not found."),
                Mode = PictureQuestionMode.MoveLabels
            };

            set1q4.Answers.Add(new PictureAnswer("Sternum", false, new Point(0, 0),
                                                 new Size(50, 10),
                                                 new PictureAlternativeAnswer("os sternalis", false),
                                                 new PictureAlternativeAnswer("i dont know...", false)));

            set1q4.Answers.Add(new PictureAnswer("Musculus Brachialis", false, new Point(0, 50),
                                                 new Size(50, 10),
                                                 new PictureAlternativeAnswer("M. brachialis")));
            set1.Questions.Add(set1q4);

            _model.Sets.Add(set1);


            set1.Statistics.Add(new QuestionSetStatistic() {
                CorrectAnswers = 43,
                IncorrectAnswers = 5,
                ExamDuration = TimeSpan.FromSeconds(1000),
                ExamTaken = DateTime.Now.AddDays(-23)
            });

            set1.Statistics.Add(new QuestionSetStatistic() {
                CorrectAnswers = 34,
                IncorrectAnswers = 7,
                ExamDuration = TimeSpan.FromSeconds(800),
                ExamTaken = DateTime.Now.AddDays(-12)
            });

            set1.Statistics.Add(new QuestionSetStatistic() {
                CorrectAnswers = 72,
                IncorrectAnswers = 1,
                ExamDuration = TimeSpan.FromSeconds(1500),
                ExamTaken = DateTime.Now.AddDays(-4)
            });

            set1.Statistics.Add(new QuestionSetStatistic() {
                CorrectAnswers = 43,
                IncorrectAnswers = 0,
                ExamDuration = TimeSpan.FromSeconds(500),
                ExamTaken = DateTime.Now
            });

        }

        [Test]
        public void SerializeWithDefaults() {
            var serializer = new ExtendedXmlSerializer<QuestionCatalog>();
            var stream = serializer.Serialize(_model);

            Assert.NotNull(stream, "Returned stream is null.");
            Assert.NotZero(stream.Length, "Stream is empty");
            Assert.Zero(stream.Position);

            var doc = ValidationHelper.GetXDoc(stream);
            ValidateHeader(doc);
            ValidateQuestionNamespace(doc, "LazyApiPack.XmlTools.Tests.Model.MultipleChoiceQuestion");

            var deserialized = serializer.Deserialize(stream, false);
            ValidateDeserialized(_model, deserialized);
            Assert.Pass();
        }

        [Test]
        public void SerializeWithShortNamespace() {
            var serializer = new ExtendedXmlSerializer<QuestionCatalog>(useFullNamespace: false);
            var stream = serializer.Serialize(_model);

            Assert.NotNull(stream, "Returned stream is null.");
            Assert.NotZero(stream.Length, "Stream is empty");
            Assert.Zero(stream.Position);

            var doc = ValidationHelper.GetXDoc(stream);
            ValidateHeader(doc);
            ValidateQuestionNamespace(doc, "MultipleChoiceQuestion");

            var deserialized = serializer.Deserialize(stream, false);

            ValidateDeserialized(_model, deserialized);
            Assert.Pass();
        }

        [Test]
        public void SerializeWithCustomCulture() {
            var ci = new CultureInfo("de-AT");
            var serializer = new ExtendedXmlSerializer<QuestionCatalog>(
                cultureInfo: ci);

            var stream = serializer.Serialize(_model);
            Assert.NotNull(stream, "Returned stream is null.");
            Assert.NotZero(stream.Length, "Stream is empty");
            Assert.Zero(stream.Position);
            var doc = ValidationHelper.GetXDoc(stream);
            ValidateHeader(doc);
            ValidateDateTime(doc, ci, null);

            var deserialized = serializer.Deserialize(stream, false);

            ValidateDeserialized(_model, deserialized, null, ci);
            Assert.Pass();
        }

        [Test]
        public void SerializeWithCustomCultureAndDTFormat() {
            var ci = new CultureInfo("de-AT");
            var dtFormat = "yyyyMMdd HHmmss";
            var serializer = new ExtendedXmlSerializer<QuestionCatalog>(
                cultureInfo: ci,
                dateTimeFormat: dtFormat);

            var stream = serializer.Serialize(_model);
            Assert.NotNull(stream, "Returned stream is null.");
            Assert.NotZero(stream.Length, "Stream is empty");
            Assert.Zero(stream.Position);
            //var xml = ValidationHelper.GetXml(stream, Encoding.UTF8);
            var doc = ValidationHelper.GetXDoc(stream);
            ValidateHeader(doc);
            ValidateDateTime(doc, ci, dtFormat);

            var deserialized = serializer.Deserialize(stream, false);

            ValidateDeserialized(_model, deserialized, dtFormat, ci);
            Assert.Pass();
        }

        void ValidateDateTime(XDocument doc, CultureInfo ci, string? format = null) {
            var root = doc.Element(XName.Get("ExtendedSerializedObjectFile"));
            var qc = root.Element(XName.Get("QuestionCatalog"));
            var dtCmp = qc.Attribute(XName.Get("LastLearned")).Value;
            if (format == null) {
                Assert.DoesNotThrow(() => DateTime.Parse(dtCmp, ci));
            } else {
                Assert.DoesNotThrow(() => DateTime.ParseExact(dtCmp, format, ci));
            }

        }


        void ValidateQuestionNamespace(XDocument doc, string expectedNamespace) {
            var qc = doc.Element(XName.Get("ExtendedSerializedObjectFile")).Element(XName.Get("QuestionCatalog"));
            var question = qc.Element(XName.Get("Sets")).Element(XName.Get("Set")).Element(XName.Get("Questions")).Element(XName.Get("Question"));
            var att = question.Attribute(XName.Get("clsType", "http://www.jodiewatson.net/xml/lzyxmlx/1.0"));
            Assert.That(expectedNamespace, Is.EqualTo(att.Value), "Namespace mismatch.");
        }

        void ValidateHeader(XDocument doc) {
            var root = doc.Elements().FirstOrDefault();
            Assert.NotNull(root, "Root element missing.");

            var header = root.Elements().FirstOrDefault();
            Assert.NotNull(header, "Header missing.");
            Assert.True(header.Name == "Header", "Header missing or not named correctly.");


        }

        public void ValidateDeserialized(QuestionCatalog original, QuestionCatalog deserialized, string? dtFormat = null, CultureInfo ci = null) {
            if (ci == null) ci = CultureInfo.InvariantCulture;
            Assert.That(deserialized.Sets?.Count, Is.EqualTo(original.Sets?.Count),
                $"Source set count = {original.Sets?.Count.ToString() ?? "NULL"} while Target set count = {deserialized.Sets?.Count.ToString() ?? "NULL"}.");

            Assert.NotNull(deserialized.Sets, "Deserialized sets are null.");
            Assert.NotNull(original.Sets, "Source sets are null.");
            Assert.That(deserialized.Sets.First().Name, Is.EqualTo(original.Sets.First().Name), "Name of the first set does not match the first set of the deserialized object.");

            if (deserialized.Sets.First().Questions[2] is PictureQuestion dq1 && original.Sets[0].Questions[2] is PictureQuestion sq1) {
                CollectionAssert.AreEqual(sq1.ImageData, dq1.ImageData, new BinaryComparer(), "Image (2) is not identical.");
            } else {
                Assert.Fail("Deserialized or serialized question (2) is not of type picture question.");
            }
            if (deserialized.Sets.First().Questions[3] is PictureQuestion dq2 && original.Sets[0].Questions[3] is PictureQuestion sq2) {
                CollectionAssert.AreEqual(sq2.ImageData, dq2.ImageData, new BinaryComparer(), "Image (3) is not identical.");
            } else {
                Assert.Fail("Deserialized or serialized question (3) is not of type picture question.");
            }
            if (dtFormat == null) {
                Assert.That(deserialized.LastLearned, Is.EqualTo(DateTime.Parse(_checkDateTime.ToString(ci), ci)));
            } else {
                Assert.That(deserialized.LastLearned, Is.EqualTo(DateTime.ParseExact(_checkDateTime.ToString(dtFormat, ci), dtFormat, ci)));
            }
        }

    }
}