using LazyApiPack.XmlTools.Zip.Tests.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.IO.Compression;

namespace LazyApiPack.XmlTools.Zip.Tests.Tests {
    public class ZipSerializerByteArrayClassesTests {
        List<ImageModelByte> _models;

        [SetUp]
        public void Setup() {
            _models = new List<ImageModelByte>();
            var model1 = new ImageModelByte();
            model1.Images.Add(ZipValidationHelper.GetResourceBytes("Sample1.jpg"));
            model1.Images.Add(ZipValidationHelper.GetResourceBytes("Sample2.jpg"));
            model1.Images.Add(ZipValidationHelper.GetResourceBytes("Sample3.jpg"));
            model1.Images.Add(ZipValidationHelper.GetResourceBytes("Sample4.jpg"));
            model1.Images.Add(ZipValidationHelper.GetResourceBytes("Sample1.jpg"));
            model1.MainImage = ZipValidationHelper.GetResourceBytes("Sample4.jpg");
            _models.Add(model1);
            var model2 = new ImageModelByte();
            model2.Images.Add(ZipValidationHelper.GetResourceBytes("Sample3.jpg"));
            model2.Images.Add(ZipValidationHelper.GetResourceBytes("Sample2.jpg"));
            model2.Images.Add(ZipValidationHelper.GetResourceBytes("Sample3.jpg"));
            model2.Images.Add(ZipValidationHelper.GetResourceBytes("Sample3.jpg"));
            model2.Images.Add(ZipValidationHelper.GetResourceBytes("Sample1.jpg"));
            model2.MainImage = ZipValidationHelper.GetResourceBytes("Sample2.jpg");
            _models.Add(model2);

            _models.Add(model1);
        }

        [Test]
        public void SerializeAsZip() {
            var xmlSerializer = new ExtendedXmlSerializer<ImageModelByte>();
            var zipSerializer = new ZipSerializer<ImageModelByte>(xmlSerializer);

            var strm = zipSerializer.SerializeAll(_models, true, CompressionLevel.Optimal);

            ValidateStream(strm);
           
            //var archive = GetData(strm);

            ValidateDeserialized(strm, zipSerializer, true, 5);
            
        }

        [Test]
        public void SerializeAsByteArray() {
            var xmlSerializer = new ExtendedXmlSerializer<ImageModelByte>();
            var zipSerializer = new ZipSerializer<ImageModelByte>(xmlSerializer);

            var strm = zipSerializer.SerializeAll(_models, true, CompressionLevel.Optimal, false);

            ValidateStream(strm);

            //var data = ValidationHelper.GetData(strm);

            ValidateDeserialized(strm, zipSerializer, false, 1);

        }

        void ValidateStream(Stream strm) {
            Assert.NotNull(strm, "Serialized is null.");
            Assert.That(strm.Position, Is.EqualTo(0), "Stream is not at position 0.");
            Assert.Greater(strm.Length, 0, "Stream is empty.");

        }

        void ValidateDeserialized(Stream archiveStream, ZipSerializer<ImageModelByte> zipSerializer, bool byteAsZip, int expectedFiles) {
            var deserialized = zipSerializer.Deserialize(archiveStream, byteArrayAsZipEntry: byteAsZip);
            var bc = new ByteArrayComparer();
            Assert.NotNull(deserialized.MainImage);
            Assert.NotNull(_models.ElementAt(0).MainImage);
            Assert.True(bc.Equals(deserialized.MainImage, _models.ElementAt(0).MainImage));

            var savedArchive = new ZipArchive(archiveStream);
            Assert.That(savedArchive.Entries.Count, Is.EqualTo(expectedFiles), "The file count in the archive is not expected."); // 0 resources and one index
            Assert.Pass();
        }
    }
}