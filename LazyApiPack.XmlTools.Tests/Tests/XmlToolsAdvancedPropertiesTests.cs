using LazyApiPack.XmlTools.Tests.Model;
using System.Collections.ObjectModel;
using System.Text;

namespace LazyApiPack.XmlTools.Tests.Tests {
    public class XmlToolsAdvancedPropertiesTests {
        ComplexTypesModel _model;

        [SetUp]
        public void Setup() {
            _model = new ComplexTypesModel();
            ObservableCollection<IItem> d1;
            _model.Dictionary.Add("My key 1", d1 = new ObservableCollection<IItem>());
            d1.Add(new CarItem() { Name = "Car 1 of key 1" });
            d1.Add(new PlaneItem() { Name = "Plane 1 of key 1" });

            IItem sharedCar;
            d1.Add(sharedCar = new CarItem() { Name = "Shared Car 2 of key 1 and 4 and array 1." });
            d1.Add(new PlaneItem() { Name = "Plane 3 of key 1" });

            ObservableCollection<IItem> d2;
            _model.Dictionary.Add("My key 3", d2 = new ObservableCollection<IItem>());
            d2.Add(new CarItem() { Name = "Car 1 of key 2" });
            d2.Add(new PlaneItem() { Name = "Plane 1 of key 2" });
            d2.Add(new CarItem() { Name = "Car 2 of key 2" });
            d2.Add(new PlaneItem() { Name = "Plane 3 of key 2" });

            ObservableCollection<IItem> d3;
            _model.Dictionary.Add("My key 5", d3 = new ObservableCollection<IItem>());
            d3.Add(new CarItem() { Name = "Car 1 of key 3" });
            d3.Add(new PlaneItem() { Name = "Plane 1 of key 3" });
            d3.Add(new CarItem() { Name = "Car 2 of key 3" });
            d3.Add(new PlaneItem() { Name = "Plane 3 of key 3" });

            ObservableCollection<IItem> d4;
            _model.Dictionary.Add("My key 7", d4 = new ObservableCollection<IItem>());
            d4.Add(new CarItem() { Name = "Car 1 of key 4" });
            d4.Add(new PlaneItem() { Name = "Plane 1 of key 4" });
            d4.Add(sharedCar);
            d4.Add(new PlaneItem() { Name = "Plane 3 of key 4" });


            _model.Array = new ICollection<IItem>[4];
            ObservableCollection<IItem> a1;
            _model.Array[0] = a1 = new ObservableCollection<IItem>();
            a1.Add(new CarItem() { Name = "Car 1 of array key 1" });
            a1.Add(new PlaneItem() { Name = "Plane 1 of array key 1" });
            a1.Add(sharedCar);
            a1.Add(new PlaneItem() { Name = "Plane 2 of array key 1" });

            ObservableCollection<IItem> a2;
            _model.Array[1] = a2 = new ObservableCollection<IItem>();
            a2.Add(new CarItem() { Name = "Car 1 of array key 2" });
            a2.Add(new PlaneItem() { Name = "Plane 1 of array key 2" });
            a2.Add(new CarItem() { Name = "Car 2 of array key 2" });
            a2.Add(new PlaneItem() { Name = "Plane 3 of array key 2" });


            ObservableCollection<IItem> a3;
            _model.Array[2] = a3 = new ObservableCollection<IItem>();
            a3.Add(new CarItem() { Name = "Car 1 of array key 3" });
            a3.Add(new PlaneItem() { Name = "Plane 1 of array key 3" });
            a3.Add(new CarItem() { Name = "Car 2 of array key 3" });
            a3.Add(new PlaneItem() { Name = "Plane 3 of array key 3" });

            ObservableCollection<IItem> a4;
            _model.Array[3] = a4 = new ObservableCollection<IItem>();
            a4.Add(new CarItem() { Name = "Car 1 of array key 4" });
            a4.Add(new PlaneItem() { Name = "Plane 1 of array key 4" });
            a4.Add(new CarItem() { Name = "Car 2 of array key 4" });
            a4.Add(new PlaneItem() { Name = "Plane 3 of array key 4" });

            _model.List.Add("Hello Item 1");
            _model.List.Add("Hello Item 2");
            _model.List.Add("Hello Item 3");
            _model.List.Add("Hello Item 4");

        }

        [Test]
        public void SerializeComplexObjects() {
            var serializer = new ExtendedXmlSerializer<ComplexTypesModel>();
            bool isSerializingCalled = false;
            bool isSerializedCalled = false;
            _model.MethodCalled += (s, e) =>
            {
                if (e.MethodName == nameof(ComplexTypesModel.OnSerialized)) {
                    Assert.IsTrue(isSerializingCalled, "OnSerializing was not called first.");
                    isSerializedCalled = true;
                } else if (e.MethodName == nameof(ComplexTypesModel.OnSerializing)) {
                    isSerializingCalled = true;
                }
            };
            var stream = serializer.Serialize(_model);


            Assert.NotNull(stream, "Returned stream is null.");
            Assert.NotZero(stream.Length, "Stream is empty");
            Assert.Zero(stream.Position);
            Assert.IsTrue(isSerializingCalled, "Serializing was not called.");
            Assert.IsTrue(isSerializedCalled, "Serialized was not called.");

            var xml = ValidationHelper.GetXml(stream, Encoding.UTF8);

            var deserialized = serializer.Deserialize(stream, false); // TODO: Hier ist noch blöd.
            Assert.IsFalse(deserialized.IsSerializing);
            Assert.IsFalse(deserialized.IsDeserializing);


            // TODO: check dictionaries and stuff.

            Assert.Pass();
        }

        private void _model_MethodCalled(object sender, ModelBase.MethodCalledEventArgs e) {
            throw new NotImplementedException();
        }
    }
}