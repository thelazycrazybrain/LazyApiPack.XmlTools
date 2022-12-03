using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools {
    /// <summary>
    /// Provides functionality that are used outside of the serializer but are not generic
    /// </summary>
    public abstract class ExtendedXmlSerializer {
        /// <summary>
        /// Provides the namespace of the element attributes that are serializer specific.
        /// </summary>
        public const string LZYNS = "http://www.jodiewatson.net/xml/lzyxmlx/1.0";



    }
}