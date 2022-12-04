using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LazyApiPack.XmlTools.Wpf.Tests.Model {
    public interface IStrokeCollection : ICollection<Stroke> {

    }

    public class StrokeCollectionEx : StrokeCollection, IStrokeCollection {

    }
    [XmlClass]
    public class WpfModel : XmlSerializableTestBase {
       
        PointCollection _pointCollectionProperty;
        [XmlProperty]
        public PointCollection PointCollectionProperty { get => _pointCollectionProperty; set => SetPropertyValue(ref _pointCollectionProperty, value); }

        private IStrokeCollection _strokeCollectionInterfaceProperty;
        [XmlProperty]
        public IStrokeCollection StrokeCollectionInterfaceProperty { get => _strokeCollectionInterfaceProperty; set => SetPropertyValue(ref _strokeCollectionInterfaceProperty, value); }

        private Point _pointProperty;
        [XmlProperty]
        public Point PointProperty { get => _pointProperty; set => SetPropertyValue(ref _pointProperty, value); }

        private Thickness _thicknessProperty;
        [XmlProperty]
        public Thickness ThicknessProperty { get => _thicknessProperty; set => SetPropertyValue(ref _thicknessProperty, value); }

        private Brush _brushProperty;
        [XmlProperty]
        public Brush BrushProperty { get => _brushProperty; set => SetPropertyValue(ref _brushProperty, value); }

        private Color _colorProperty;
        [XmlProperty]
        public Color ColorProperty { get => _colorProperty; set => SetPropertyValue(ref _colorProperty, value); }

        private StrokeCollection _strokeCollectionProperty;
        [XmlProperty]
        public StrokeCollection StrokeCollectionProperty { get => _strokeCollectionProperty; set => SetPropertyValue(ref _strokeCollectionProperty, value); }

        private BitmapImage _bitmapImageProperty;
        [XmlProperty]
        public BitmapImage BitmapImageProperty { get => _bitmapImageProperty; set => SetPropertyValue(ref _bitmapImageProperty, value); }

        private Rect _rectProperty;
        [XmlProperty]
        public Rect RectProperty { get => _rectProperty; set => SetPropertyValue(ref _rectProperty, value); }

        private Matrix _matrixProperty;
        [XmlProperty]
        public Matrix MatrixProperty { get => _matrixProperty; set => SetPropertyValue(ref _matrixProperty, value); }

        private DrawingAttributes _drawingAttributesProperty;
        [XmlProperty]
        public DrawingAttributes DrawingAttributesProperty { get => _drawingAttributesProperty; set => SetPropertyValue(ref _drawingAttributesProperty, value); }

        private StylusPointCollection _stylusPointCollectionProperty;
        [XmlProperty]
        public StylusPointCollection StylusPointCollectionProperty { get => _stylusPointCollectionProperty; set => SetPropertyValue(ref _stylusPointCollectionProperty, value); }

        private TextDecorationCollection _textDecorationCollectionProperty;
        [XmlProperty]
        public TextDecorationCollection TextDecorationCollectionProperty { get => _textDecorationCollectionProperty; set => SetPropertyValue(ref _textDecorationCollectionProperty, value); }

        private Pen _penProperty;
        [XmlProperty]
        public Pen PenProperty { get => _penProperty; set => SetPropertyValue(ref _penProperty, value); }

        private DashStyle _dashStyleProperty;
        [XmlProperty]
        public DashStyle DashStyleProperty { get => _dashStyleProperty; set => SetPropertyValue(ref _dashStyleProperty, value); }

        private MatrixTransform _matrixTransformProperty;
        [XmlProperty]
        public MatrixTransform MatrixTransformProperty { get => _matrixTransformProperty; set => SetPropertyValue(ref _matrixTransformProperty, value); }

        private RotateTransform _rotateTransformProperty;
        [XmlProperty]
        public RotateTransform RotateTransformProperty { get => _rotateTransformProperty; set => SetPropertyValue(ref _rotateTransformProperty, value); }

        private ScaleTransform _scaleTransformProperty;
        [XmlProperty]
        public ScaleTransform ScaleTransformProperty { get => _scaleTransformProperty; set => SetPropertyValue(ref _scaleTransformProperty, value); }

        private SkewTransform _skewTransformProperty;
        [XmlProperty]
        public SkewTransform SkewTransformProperty { get => _skewTransformProperty; set => SetPropertyValue(ref _skewTransformProperty, value); }

        private TranslateTransform _translateTransformProperty;
        [XmlProperty]
        public TranslateTransform TranslateTransformProperty { get => _translateTransformProperty; set => SetPropertyValue(ref _translateTransformProperty, value); }
    }
}
