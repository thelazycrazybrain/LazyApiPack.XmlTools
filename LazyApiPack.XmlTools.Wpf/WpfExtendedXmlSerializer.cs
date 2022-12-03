using LazyApiPack.XmlTools;
using LazyApiPack.XmlTools.Helpers;
using LazyApiPack.XmlTools.Wpf.Serializers;
using System.CodeDom;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Wpf {
    /// <summary>
    /// Adds class extensions for the ExtendedXmlSerializer to add extended type support for wpf specific types.
    /// </summary>
    public static class WpfExtendedXmlSerializer {
        /// <summary>
        /// Adds support for Wpf specific types.
        /// </summary>
        /// <typeparam name="TClass">Class Type of the serializer target object.</typeparam>
        /// <param name="serializer">The serializer that uses the extensions.</param>
        public static ExtendedXmlSerializer<TClass> UseWpfTypeSupport
            <TClass>(this ExtendedXmlSerializer<TClass> serializer) where TClass : class {

            PointSerializer pointSerializer;
            serializer.ExternalSerializers.Add(pointSerializer = new PointSerializer());
            serializer.ExternalSerializers.Add(new PointCollectionSerializer(pointSerializer));
            serializer.ExternalSerializers.Add(new ThicknessSerializer());
            ColorSerializer colorSerializer;
            serializer.ExternalSerializers.Add(colorSerializer = new ColorSerializer());
            MatrixSerializer matrixSerializer;
            serializer.ExternalSerializers.Add(matrixSerializer =new MatrixSerializer());
            TransformSerializer transformSerializer;
            serializer.ExternalSerializers.Add(transformSerializer = new TransformSerializer(matrixSerializer));
            RectSerializer rectSerializer;
            serializer.ExternalSerializers.Add(rectSerializer =new RectSerializer());
            BitmapImageSerializer bitmapImageSerializer;
            serializer.ExternalSerializers.Add(bitmapImageSerializer = new BitmapImageSerializer());
            BrushSerializer brushSerializer;
            serializer.ExternalSerializers.Add(brushSerializer = 
                new BrushSerializer(colorSerializer, transformSerializer, rectSerializer, bitmapImageSerializer));
            serializer.ExternalSerializers.Add(new StrokeCollectionSerializer(colorSerializer, matrixSerializer));
            serializer.ExternalSerializers.Add(new DrawingAttributesSerializer(colorSerializer, matrixSerializer));
            serializer.ExternalSerializers.Add(new StylusPointCollectionSerializer());
            DashStyleSerializer dashStyleSerializer;
            serializer.ExternalSerializers.Add(dashStyleSerializer = new DashStyleSerializer());
            PenSerializer penSerializer;
            serializer.ExternalSerializers.Add(penSerializer = new PenSerializer(dashStyleSerializer, brushSerializer));
            serializer.ExternalSerializers.Add(new TextDecorationCollectionSerializer(penSerializer));

            return serializer;
        }

    }
}