using LazyApiPack.XmlTools.Zip.Tests.Models;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Zip.Tests.Tests {
    public class ZipSerializerTests {
        ImageModel _model;
        [SetUp]
        public void Setup() {
            _model = new ImageModel();
            _model.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample1.jpg")));
            _model.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample2.jpg")));
            _model.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample3.jpg")));
            _model.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample4.jpg")));
            _model.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample1.jpg")));

            _model.MainImage = new ZipResource(ZipValidationHelper.GetResource("Sample4.jpg"));
        }

        [Test]
        public void TestHashed() {
            var xmlSerializer = new ExtendedXmlSerializer<ImageModel>();
            var zipSerializer = new ZipSerializer<ImageModel>(xmlSerializer);

            var strm = zipSerializer.Serialize(_model, true, CompressionLevel.Optimal);
            Assert.NotNull(strm, "Serialized is null.");
            Assert.That(strm.Position, Is.EqualTo(0), "Stream is not at position 0.");
            Assert.Greater(strm.Length, 0, "Stream is empty.");

            //var archive = GetData(strm);

            var deserialized = zipSerializer.Deserialize(strm);
            var bc = new ByteArrayComparer();
            Assert.NotNull(deserialized.MainImage?.Data);
            Assert.NotNull(_model.MainImage?.Data);
            Assert.True(bc.Equals(deserialized.MainImage.Data, _model.MainImage.Data));

            var savedArchive = new ZipArchive(strm);
            Assert.That(savedArchive.Entries.Count, Is.EqualTo(5), "The file count in the archive is not expected."); // 4 resources and one index
            Assert.Pass();
        }

        [Test]
        public void TestUnhashed() {
            var xmlSerializer = new ExtendedXmlSerializer<ImageModel>();
            var zipSerializer = new ZipSerializer<ImageModel>(xmlSerializer);

            var strm = zipSerializer.Serialize(_model, false, CompressionLevel.Optimal);
            Assert.NotNull(strm, "Serialized is null.");
            Assert.That(strm.Position, Is.EqualTo(0), "Stream is not at position 0.");
            Assert.Greater(strm.Length, 0, "Stream is empty.");

            //var archive = GetData(strm);

            var deserialized = zipSerializer.Deserialize(strm);
            var bc = new ByteArrayComparer();
            Assert.NotNull(deserialized.MainImage?.Data);
            Assert.NotNull(_model.MainImage?.Data);
            Assert.True(bc.Equals(deserialized.MainImage.Data, _model.MainImage.Data));

            var savedArchive = new ZipArchive(strm);
            Assert.That(savedArchive.Entries.Count, Is.EqualTo(7), "The file count in the archive is not expected."); // 6 (duplicate) resources and one index
            Assert.Pass();
        }

        
    }
}