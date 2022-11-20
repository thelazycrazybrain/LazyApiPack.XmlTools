using LazyApiPack.XmlTools.Tests.Model;

namespace LazyApiPack.XmlTools.Tests.Tests {
    public class XmlToolsMultipleObjectSerializationTests {
        List<RecursiveModel> _models = new List<RecursiveModel>();
        [SetUp]
        public void Setup() {

            var model1 = new RecursiveModel("Root 1");
            model1.Children.Add(new RecursiveModel("M11"));
            RecursiveModel m12;
            model1.Children.Add(m12 = new RecursiveModel("M12"));
            m12.Children.Add(model1);
            model1.Children.Add(new RecursiveModel("M13"));
            model1.Children.Add(new RecursiveModel("M14"));
            model1.Children.Add(new RecursiveModel("M15"));


            var model2 = new RecursiveModel("Root 2");
            model2.Children.Add(new RecursiveModel("M21"));
            RecursiveModel m22;
            model2.Children.Add(m22 = new RecursiveModel("M22"));
            m22.Children.Add(model2);
            model2.Children.Add(new RecursiveModel("M23"));
            model2.Children.Add(new RecursiveModel("M24"));
            m22.Children.Add(model1);
            model2.Children.Add(new RecursiveModel("M25"));

            _models.Add(model1);
            _models.Add(model2);
            _models.Add(model1);
        }

        [Test]
        public void TestMultipleRecursion() {
            var serializer = new ExtendedXmlSerializer<RecursiveModel>();
            var stream = serializer.SerializeAll(_models);

            Assert.NotNull(stream, "Returned stream is null.");
            Assert.NotZero(stream.Length, "Stream is empty");
            Assert.Zero(stream.Position);
            //var xml = ValidationHelper.GetXml(stream, Encoding.UTF8);
            //var doc = ValidationHelper.GetXDoc(stream);


            var deserialized = serializer.DeserializeAll(stream, false);

            Assert.That(deserialized.Count(), Is.EqualTo(3), "The amount of deserialized objects does not match the serialized objects.");
            Assert.IsNotEmpty(deserialized.ElementAt(0).Children, "Deserialized children are empty.");
            Assert.That(_models.ElementAt(0).Children.Count, Is.EqualTo(deserialized.ElementAt(0).Children.Count), "Deserialized children length mismatch.");

            Assert.IsNotEmpty(deserialized.ElementAt(0).Children[1].Children, "M2 does not contain any children."); // M2

            Assert.That(deserialized.ElementAt(0).Children[1].Children.First(), Is.SameAs(deserialized.ElementAt(0)), "M2 child is not the exact reference as root!");

            Assert.That(deserialized.ElementAt(2), Is.SameAs(deserialized.ElementAt(0)), "Item 3 on root is not the exact same object as Item 1!");

            Assert.Pass();
        }

    }
}