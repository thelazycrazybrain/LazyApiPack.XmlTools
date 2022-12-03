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
                Matrix  =new Matrix(),
                BitmapImage = new BitmapImage(),
                Brush = new RadialGradientBrush(),
                Color = Color.FromArgb(255, 123, 42, 15),
                DashStyle = new DashStyle(),
                DrawingAttributes = new DrawingAttributes(),
                MatrixTransform = new MatrixTransform(),
                Pen = new Pen(),
                PointCollection = new PointCollection(),
                Rect = new System.Windows.Rect(),
                RotateTransform = new RotateTransform(),
                ScaleTransform = new ScaleTransform(),
                SkewTransform = new SkewTransform(),
                StrokeCollection  = new StrokeCollection(),
                StylusPointCollection = new StylusPointCollection(),
                TextDecorationCollection = new TextDecorationCollection(),
                Thickness = new Thickness(),
                TranslateTransform = new TranslateTransform(),
                Point = new Point()
            };
        }

        [Test]
        public void SerializeWpf() {
            var serializer = new ExtendedXmlSerializer<WpfModel>()
                .UseWpfTypeSupport();
            var strm = serializer.Serialize(_model);

            var data = ValidationHelper.GetData(strm);

            var deserialized = serializer.Deserialize(strm);
        }
    }
}