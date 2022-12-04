using LazyApiPack.XmlTools.Wpf.Tests.Model;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows;
using LazyApiPack.XmlTools.Tests;

namespace LazyApiPack.XmlTools.Wpf.Tests.Tests {
    public class SerializationTests {
        WpfModel _model;
        [SetUp]
        public void Setup() {
            _model = new WpfModel() {
                MatrixProperty = new Matrix(),
                BitmapImageProperty = new BitmapImage(),
                BrushProperty = new RadialGradientBrush(),
                ColorProperty = Color.FromArgb(255, 123, 42, 15),
                DashStyleProperty = new DashStyle(),
                DrawingAttributesProperty = new DrawingAttributes(),
                MatrixTransformProperty = new MatrixTransform(),
                PenProperty = new Pen(),
                PointCollectionProperty = new PointCollection(),
                RectProperty = new Rect(),
                RotateTransformProperty = new RotateTransform(),
                ScaleTransformProperty = new ScaleTransform(),
                SkewTransformProperty = new SkewTransform(),
                StrokeCollectionProperty = new StrokeCollection(),
                StylusPointCollectionProperty = new StylusPointCollection(),
                TextDecorationCollectionProperty = new TextDecorationCollection(),
                ThicknessProperty = new Thickness(),
                TranslateTransformProperty = new TranslateTransform(),
                PointProperty = new Point(1,1),
                StrokeCollectionInterfaceProperty = new StrokeCollectionEx()
            };
        }

        [Test]
        public void SerializeWpf() {
            var serializer = new ExtendedXmlSerializer<WpfModel>()
                .UseWpfTypeSupport();
            var strm = serializer.Serialize(_model);

            var data = ValidationHelper.GetData(strm);

            var deserialized = serializer.Deserialize(strm, false);
        }
    }
}