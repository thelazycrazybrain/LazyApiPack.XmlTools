using LazyApiPack.XmlTools.Tests.Model.SimpleModelV1;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Tests.Tests {
    public class XmlToolsObsoleteMigrationTests {
        ObsoleteModel _model;
        [SetUp]
        public void Setup() {
            _model = new ObsoleteModel();
        }

        [Test]
        public void ObsoleteTest() {
            var serializer = new ExtendedXmlSerializer<ObsoleteModel>();
            //var result = serializer.Serialize(_model);
            //using (var fs = File.OpenWrite("C:\\Temp\\obsolete.xml")) {
            //    fs.SetLength(result.Length);
            //    result.CopyTo(fs);
            //    throw new InvalidOperationException("Use this block only to create the xml file to test.");
            //}

            var oldXml = ValidationHelper.GetResource("obsolete.xml") ?? throw new FileNotFoundException("Can not find resource obsolete.xml.");
            var ms = new MemoryStream();
            oldXml.CopyTo(ms);
            ms.Position = 0;

            var deserialized = serializer.Deserialize(ms, false);
            ms.Dispose();
            ms = null;
            Assert.That(deserialized.Value, Is.EqualTo(1337));


            var serialized = serializer.Serialize(deserialized);
            var doc = ValidationHelper.GetXDoc(serialized);
            var obsoleteModelNode = doc.Element("ExtendedSerializedObjectFile")
                                       .Element("ObsoleteModel")
                                       .Elements();
            Assert.True(obsoleteModelNode.Any(x => x.Name == "intValue"));
            Assert.False(obsoleteModelNode.Any(x => x.Name == "value"));




            Assert.Pass();

        }


    }
}