using LazyApiPack.XmlTools.Tests;
using LazyApiPack.XmlTools.Zip.Tests.Models;
using System.IO.Compression;

namespace LazyApiPack.XmlTools.Zip.Tests.Tests {

    public class ZipSerializerMultipleClassesTests {
        List<ImageModel> _models;

        [SetUp]
        public void Setup() {
            _models = new List<ImageModel>();
            var model1 = new ImageModel();
            model1.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample1.jpg")));
            model1.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample2.jpg")));
            model1.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample3.jpg")));
            model1.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample4.jpg")));
            model1.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample1.jpg")));
            model1.MainImage = new ZipResource(ZipValidationHelper.GetResource("Sample4.jpg"));
            _models.Add(model1);
            var model2 = new ImageModel();
            model2.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample3.jpg")));
            model2.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample2.jpg")));
            model2.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample3.jpg")));
            model2.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample3.jpg")));
            model2.Images.Add(new ZipResource(ZipValidationHelper.GetResource("Sample1.jpg")));
            model2.MainImage = new ZipResource(ZipValidationHelper.GetResource("Sample2.jpg"));
            _models.Add(model2);

            _models.Add(model1);
        }

        [Test]
        public void TestMultipleHashed() {
            var xmlSerializer = new ExtendedXmlSerializer<ImageModel>();
            var zipSerializer = new ZipSerializer<ImageModel>(xmlSerializer);

            var strm = zipSerializer.SerializeAll(_models, true, CompressionLevel.Optimal);
            Assert.NotNull(strm, "Serialized is null.");
            Assert.That(strm.Position, Is.EqualTo(0), "Stream is not at position 0.");
            Assert.Greater(strm.Length, 0, "Stream is empty.");

            //var archive = GetData(strm);
            var data = ValidationHelper.GetData(strm);
            var deserialized = zipSerializer.Deserialize(strm);
            var bc = new ByteArrayComparer();
            Assert.NotNull(deserialized.MainImage?.Data);
            Assert.NotNull(_models.ElementAt(0).MainImage?.Data);
            Assert.True(bc.Equals(deserialized.MainImage.Data, _models.ElementAt(0).MainImage.Data));

            var savedArchive = new ZipArchive(strm);
            Assert.That(savedArchive.Entries.Count, Is.EqualTo(5), "The file count in the archive is not expected."); // 4 resources and one index
            Assert.Pass();
        }

        [Test]
        public void TestMultipleUnhashed() {
            var xmlSerializer = new ExtendedXmlSerializer<ImageModel>();
            var zipSerializer = new ZipSerializer<ImageModel>(xmlSerializer);

            var strm = zipSerializer.SerializeAll(_models, false, CompressionLevel.Optimal);
            Assert.NotNull(strm, "Serialized is null.");
            Assert.That(strm.Position, Is.EqualTo(0), "Stream is not at position 0.");
            Assert.Greater(strm.Length, 0, "Stream is empty.");


            var deserialized = zipSerializer.Deserialize(strm);
            var bc = new ByteArrayComparer();
            Assert.NotNull(deserialized.MainImage?.Data);
            Assert.NotNull(_models.ElementAt(0).MainImage?.Data);
            Assert.True(bc.Equals(deserialized.MainImage.Data, _models.ElementAt(0).MainImage.Data));

            Assert.That(_models.Count(), Is.EqualTo(3), "Amount of objects in xml is not correct.");
            Assert.That(_models.ElementAt(0), Is.EqualTo(_models.ElementAt(2)), "First element is not the same object as the third object.");
            var savedArchive = new ZipArchive(strm);
            Assert.That(savedArchive.Entries.Count, Is.EqualTo(13), "The file count in the archive is not expected."); // 12 resources and one index
            Assert.Pass();
        }



    }
}