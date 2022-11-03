using LazyApiPack.XmlTools;
using LazyApiPack.XmlTools.Tests;
using LazyApiPack.XmlTools.Wpf;
using System.Data;
using System.Drawing;
using System.IO;


var data = new ExtendedXmlSerializer<SimpleDataModel>()
    .UseWpfTypeSupport()
    .Deserialize("C:\\temp\\sdm.xml");

return;
//var sdm = new SimpleDataModel();



//sdm.Location = new System.Windows.Point(1, 12);
//sdm.Padding = new System.Windows.Thickness(50);
//sdm.Version = new Version(1, 2, 3, 4);
//new ExtendedXmlSerializer<SimpleDataModel>()
//    .UseWpfTypeSupport()
//    .Serialize(sdm, "C:\\temp\\sdm.xml");

//return;

//var qc1 = QuestionCatalog.Load("C:\\temp\\questionaire.xml");

//return;

var qc = new QuestionCatalog() { Name = "Qc 1" };
var setn = new QuestionSet("This is hidden");
qc.SetSingle(setn);

var set1 = new QuestionSet("Standard random questions...");

var set1q1 = new MultipleChoiceQuestion("How much wood would a woodchuck chuck, if a woodchuck could chuck wood?", "Specify the correct answer");
set1q1.Answers.Add(new MultipleChoiceAnswer("None", false));
set1q1.Answers.Add(new MultipleChoiceAnswer("A woodchuck cant chuck wood.", true));
set1q1.Answers.Add(new MultipleChoiceAnswer("Shut up..", true));
set1.Questions.Add(set1q1);


var set1q2 = new VocabularyQuestion("What does 'in front of' mean in latin?", null);
set1q2.Answers.Add(new VocabularyAnswer("ventral", false));
set1q2.Answers.Add(new VocabularyAnswer("ventr.", false));
set1.Questions.Add(set1q2);


var set1q3 = new PictureQuestion("Fill in the texts in the given image", "Do something...") {
    ImageData = File.ReadAllBytes(@"F:\Pictures\Wallpapers\uwp2465872.jpeg"),
    Mode = PictureQuestionMode.EnterText
};

set1q3.Answers.Add(new PictureAnswer("Sternum", false, new Point(0, 0), new Size(50, 10), new PictureAlternativeAnswer("os sternalis", false), new PictureAlternativeAnswer("i dont know...", false)));
set1q3.Answers.Add(new PictureAnswer("Musculus Brachialis", false, new Point(0, 50), new Size(50, 10), new PictureAlternativeAnswer("M. brachialis")));
set1.Questions.Add(set1q3);

var set1q4 = new PictureQuestion("Fill in the texts in the given image", null) {
    ImageData = File.ReadAllBytes(@"C:\temp\image.jpg"),
    Mode = PictureQuestionMode.MoveLabels
};

set1q4.Answers.Add(new PictureAnswer("Sternum", false, new Point(0, 0), new Size(50, 10), new PictureAlternativeAnswer("os sternalis", false), new PictureAlternativeAnswer("i dont know...", false)));
set1q4.Answers.Add(new PictureAnswer("Musculus Brachialis", false, new Point(0, 50), new Size(50, 10), new PictureAlternativeAnswer("M. brachialis")));
set1.Questions.Add(set1q4);

qc.Sets.Add(set1);


set1.Statistics.Add(new QuestionSetStatistic() { CorrectAnswers = 43, IncorrectAnswers = 5, ExamDuration = TimeSpan.FromSeconds(1000), ExamTaken = DateTime.Now.AddDays(-23) });
set1.Statistics.Add(new QuestionSetStatistic() { CorrectAnswers = 34, IncorrectAnswers = 7, ExamDuration = TimeSpan.FromSeconds(800), ExamTaken = DateTime.Now.AddDays(-12) });
set1.Statistics.Add(new QuestionSetStatistic() { CorrectAnswers = 72, IncorrectAnswers = 1, ExamDuration = TimeSpan.FromSeconds(1500), ExamTaken = DateTime.Now.AddDays(-4) });
set1.Statistics.Add(new QuestionSetStatistic() { CorrectAnswers = 43, IncorrectAnswers = 0, ExamDuration = TimeSpan.FromSeconds(500), ExamTaken = DateTime.Now });

//var set2 = new QuestionSet();

//qc.Sets.Add(set2);

qc.Save("C:\\Temp\\questionaire.xml");