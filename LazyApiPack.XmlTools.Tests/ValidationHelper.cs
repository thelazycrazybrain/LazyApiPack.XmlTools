using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Tests {
    internal static class ValidationHelper {

        internal static XDocument GetXDoc(Stream stream) {
            var pos = stream.Position;
            var doc = XDocument.Load(XmlReader.Create(stream));
            if (stream.CanSeek) {
                stream.Position = pos;
            }
            return doc;
        }


        internal static string GetXml(Stream stream, Encoding enc) {
            var pos = stream.Position;
            var buff = new byte[stream.Length - pos];
            stream.Read(buff, 0, buff.Length);

            if (stream.CanSeek) {
                stream.Position = pos;
            }

            return enc.GetString(buff);

        }
    }
}