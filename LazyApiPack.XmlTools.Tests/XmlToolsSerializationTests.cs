using LazyApiPack.XmlTools.Tests.Model;
using System.Drawing;
using System.Reflection;

namespace LazyApiPack.XmlTools.Tests {
    public class XmlToolsSerializationTests {
        QuestionCatalog _model;

        [SetUp]
        public void Setup() {
            var asm = Assembly.GetExecutingAssembly();
            _model = new QuestionCatalog() {
                Name = "Question Catalog 1"
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
            // TODO: Test with full and partial namespaces
            // TODO: Test with different CultureInfo and DateTime formats
            // TODO: Test with supressed Id
            // TODO: Test with recursive objects
            // TODO: Test xml migration up
            // TODO: Test xml migration down
            // TODO: Test dictionaries
            // TODO: Test arrays
            // TODO: Check if methods are invoked (_deserialized.MethodChanged).
            // TODO: Try to deserialize an invalid object and trigger an invaild object state and check the flag.

            var serializer = new ExtendedXmlSerializer<QuestionCatalog>();
            var stream = serializer.Serialize(_model);


            Assert.NotNull(stream, "Returned stream is null.");
            Assert.NotZero(stream.Length, "Stream is empty");
            Assert.Zero(stream.Position);

            var deserialized = serializer.Deserialize(stream, false);

            Assert.That(deserialized.Sets?.Count, Is.EqualTo(_model.Sets?.Count),
                $"Source set count = {_model.Sets?.Count.ToString() ?? "NULL"} while Target set count = {deserialized.Sets?.Count.ToString() ?? "NULL"}.");

            Assert.NotNull(deserialized.Sets, "Deserialized sets are null.");
            Assert.NotNull(_model.Sets, "Source sets are null.");
            Assert.That(deserialized.Sets.First().Name, Is.EqualTo(_model.Sets.First().Name), "Name of the first set does not match the first set of the deserialized object.");

            if (deserialized.Sets.First().Questions[2] is PictureQuestion dq1 && _model.Sets[0].Questions[2] is PictureQuestion sq1) {
                CollectionAssert.AreEqual(sq1.ImageData, dq1.ImageData, new BinaryComparer(), "Image (2) is not identical.");
            } else {
                Assert.Fail("Deserialized or serialized question (2) is not of type picture question.");
            }
            if (deserialized.Sets.First().Questions[3] is PictureQuestion dq2 && _model.Sets[0].Questions[3] is PictureQuestion sq2) {
                CollectionAssert.AreEqual(sq2.ImageData, dq2.ImageData, new BinaryComparer(), "Image (3) is not identical.");
            } else {
                Assert.Fail("Deserialized or serialized question (3) is not of type picture question.");
            }
            Assert.Pass();
        }

    }


}