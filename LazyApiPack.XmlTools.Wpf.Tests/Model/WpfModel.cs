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
    [XmlClass]
    public class WpfModel : XmlSerializableTestBase {
       
        PointCollection _pointCollection;
        [XmlProperty]
        public PointCollection PointCollection { get => _pointCollection; set => SetPropertyValue(ref _pointCollection, value); }

        private Point _point;
        [XmlProperty]
        public Point Point { get => _point; set => SetPropertyValue(ref _point, value); }

        private Thickness _thickness;
        [XmlProperty]
        public Thickness Thickness { get => _thickness; set => SetPropertyValue(ref _thickness, value); }

        private Brush _brush;
        [XmlProperty]
        public Brush Brush { get => _brush; set => SetPropertyValue(ref _brush, value); }

        private Color _color;
        [XmlProperty]
        public Color Color { get => _color; set => SetPropertyValue(ref _color, value); }

        private StrokeCollection _strokeCollection;
        [XmlProperty]
        public StrokeCollection StrokeCollection { get => _strokeCollection; set => SetPropertyValue(ref _strokeCollection, value); }

        private BitmapImage _bitmapImage;
        [XmlProperty]
        public BitmapImage BitmapImage { get => _bitmapImage; set => SetPropertyValue(ref _bitmapImage, value); }

        private Rect _rect;
        [XmlProperty]
        public Rect Rect { get => _rect; set => SetPropertyValue(ref _rect, value); }

        private Matrix _matrix;
        [XmlProperty]
        public Matrix Matrix { get => _matrix; set => SetPropertyValue(ref _matrix, value); }

        private DrawingAttributes _drawingAttributes;
        [XmlProperty]
        public DrawingAttributes DrawingAttributes { get => _drawingAttributes; set => SetPropertyValue(ref _drawingAttributes, value); }

        private StylusPointCollection _stylusPointCollection;
        [XmlProperty]
        public StylusPointCollection StylusPointCollection { get => _stylusPointCollection; set => SetPropertyValue(ref _stylusPointCollection, value); }

        private TextDecorationCollection _textDecorationCollection;
        [XmlProperty]
        public TextDecorationCollection TextDecorationCollection { get => _textDecorationCollection; set => SetPropertyValue(ref _textDecorationCollection, value); }

        private Pen _pen;
        [XmlProperty]
        public Pen Pen { get => _pen; set => SetPropertyValue(ref _pen, value); }

        private DashStyle _dashStyle;
        [XmlProperty]
        public DashStyle DashStyle { get => _dashStyle; set => SetPropertyValue(ref _dashStyle, value); }

        private MatrixTransform _matrixTransform;
        [XmlProperty]
        public MatrixTransform MatrixTransform { get => _matrixTransform; set => SetPropertyValue(ref _matrixTransform, value); }

        private RotateTransform _rotateTransform;
        [XmlProperty]
        public RotateTransform RotateTransform { get => _rotateTransform; set => SetPropertyValue(ref _rotateTransform, value); }

        private ScaleTransform _scaleTransform;
        [XmlProperty]
        public ScaleTransform ScaleTransform { get => _scaleTransform; set => SetPropertyValue(ref _scaleTransform, value); }

        private SkewTransform _skewTransform;
        [XmlProperty]
        public SkewTransform SkewTransform { get => _skewTransform; set => SetPropertyValue(ref _skewTransform, value); }

        private TranslateTransform _translateTransform;
        [XmlProperty]
        public TranslateTransform TranslateTransform { get => _translateTransform; set => SetPropertyValue(ref _translateTransform, value); }
    }
}
