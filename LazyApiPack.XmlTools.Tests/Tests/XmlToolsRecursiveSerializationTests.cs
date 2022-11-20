using LazyApiPack.XmlTools.Tests.Model;
using NUnit.Framework.Constraints;
using System.Text;

namespace LazyApiPack.XmlTools.Tests.Tests {
    public class XmlToolsRecursiveSerializationTests {
        RecursiveModel _model;
        [SetUp]
        public void Setup() {
            _model = new RecursiveModel("Root");
            _model.Children.Add(new RecursiveModel("M1"));
            RecursiveModel m2;
            _model.Children.Add(m2 = new RecursiveModel("M2"));
            m2.Children.Add(_model);
            _model.Children.Add(new RecursiveModel("M3"));
            _model.Children.Add(new RecursiveModel("M4"));
            _model.Children.Add(new RecursiveModel("M5"));

        }

        [Test]
        public void TestRecursion() {
            var serializer = new ExtendedXmlSerializer<RecursiveModel>();
            var stream = serializer.Serialize(_model);

            Assert.NotNull(stream, "Returned stream is null.");
            Assert.NotZero(stream.Length, "Stream is empty");
            Assert.Zero(stream.Position);
            //var xml = ValidationHelper.GetXml(stream, Encoding.UTF8);
            //var doc = ValidationHelper.GetXDoc(stream);


            var deserialized = serializer.Deserialize(stream, false);
            Assert.IsNotEmpty(deserialized.Children, "Deserialized children are empty.");
            Assert.That(_model.Children.Count, Is.EqualTo(deserialized.Children.Count), "Deserialized children length mismatch.");

            Assert.IsNotEmpty(deserialized.Children[1].Children, "M2 does not contain any children."); // M2

            Assert.That(deserialized.Children[1].Children.First(), Is.SameAs(deserialized), "M2 child is not the exact reference as root!");



            Assert.Pass();
        }

    }
}