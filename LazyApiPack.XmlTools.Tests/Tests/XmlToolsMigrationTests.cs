using System.IO;
using System.Text;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Tests.Tests {
    public class XmlToolsMigrationTests {
        Model.SimpleModelV1.SimpleModel _modelV1;

        [SetUp]
        public void Setup() {
            _modelV1 = new Model.SimpleModelV1.SimpleModel("Super Model V1");
        }

        [Test]
        public void Migrate() {
            var serializer = new ExtendedXmlSerializer<Model.SimpleModelV1.SimpleModel>(
                useFullNamespace: false, // Serialize ModelV1 and deserialize it to ModelV2
                appName: "App1",
                appVersion: new Version(1, 0, 0, 0));

            var modelV1Stream = serializer.Serialize(_modelV1);
            Assert.NotNull(modelV1Stream, "Returned stream is null.");
            Assert.NotZero(modelV1Stream.Length, "Stream is empty");
            Assert.Zero(modelV1Stream.Position);
            // var xml = ValidationHelper.GetXml(modelV1Stream, Encoding.UTF8);
            // var doc = ValidationHelper.GetXDoc(modelV1Stream);


            var deserializer = new ExtendedXmlSerializer<Model.SimpleModelV2.SimpleModel>(
                useFullNamespace: false, // Serialize ModelV1 and deserialize it to ModelV2
                appName: "App2",
                appVersion: new Version(2, 0, 0, 0));



            bool handlerExecuted = false;
            deserializer.MigrateXmlDocument +=
                (sender,
                xmlAppName, currentAppName,
                xmlAppVersion, currentAppVersion,
                doc) =>
            {
                handlerExecuted = true;
                Assert.That(xmlAppName, Is.EqualTo("App1"), "App name in xml invalid.");
                Assert.That(currentAppName, Is.EqualTo("App2"), "App name in serializer invalid.");

                Assert.That(xmlAppVersion, Is.EqualTo(new Version(1, 0, 0, 0)), "App name in xml invalid.");
                Assert.That(currentAppName, Is.EqualTo("App2"), "App name in serializer invalid.");

                var root = doc.Element(XName.Get("ExtendedSerializedObjectFile"));
                var simpleModel = root.Element(XName.Get("SimpleModel"));
                var nameAttribute = simpleModel.Attribute(XName.Get("Name"));
                simpleModel.Add(new XAttribute("Caption", nameAttribute.Value));
                nameAttribute.Remove();

                return true;
            };
            var modelV2 = deserializer.Deserialize(modelV1Stream, true);

            Assert.True(handlerExecuted, "Migration handler was not executed.");

            Assert.That(_modelV1.Name, Is.EqualTo(modelV2.Caption), "Conversion was not successful");
            Assert.Pass();

        }

        [Test]
        public void MigrateNoHandler() {
            var serializer = new ExtendedXmlSerializer<Model.SimpleModelV1.SimpleModel>(
                useFullNamespace: false, // Serialize ModelV1 and deserialize it to ModelV2
                appName: "App1",
                appVersion: new Version(1, 0, 0, 0));

            var modelV1Stream = serializer.Serialize(_modelV1);
            Assert.NotNull(modelV1Stream, "Returned stream is null.");
            Assert.NotZero(modelV1Stream.Length, "Stream is empty");
            Assert.Zero(modelV1Stream.Position);



            var deserializer = new ExtendedXmlSerializer<Model.SimpleModelV2.SimpleModel>(
                useFullNamespace: false, // Serialize ModelV1 and deserialize it to ModelV2
                appName: "App2",
                appVersion: new Version(2, 0, 0, 0));

            Assert.Catch(() => deserializer.Deserialize(modelV1Stream, true), "Deserialization should fail with migration enabled and no handler.");

            Assert.Pass();
        }
    }
}