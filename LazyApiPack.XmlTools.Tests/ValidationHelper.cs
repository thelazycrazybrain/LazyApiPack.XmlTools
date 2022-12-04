using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools.Tests {
    public static class ValidationHelper {

        public static XDocument GetXDoc(Stream stream) {
            var pos = stream.Position;
            var doc = XDocument.Load(XmlReader.Create(stream));
            if (stream.CanSeek) {
                stream.Position = pos;
            }
            return doc;
        }

        public static Stream? GetResource(string name) {
            if (name == null) return null;
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetManifestResourceStream($"LazyApiPack.XmlTools.Tests.Resources.{name}" ?? throw new NullReferenceException($"Application Resource {name} was not found."));
        }


        public static string GetXml(Stream stream, Encoding enc) {
            var pos = stream.Position;
            var buff = new byte[stream.Length - pos];
            stream.Read(buff, 0, buff.Length);

            if (stream.CanSeek) {
                stream.Position = pos;
            }

            return enc.GetString(buff);

        }

        public static byte[] GetData(Stream strm) {
            var buffer = new byte[strm.Length];
            var lastPos = strm.Position;
            strm.Read(buffer, 0, buffer.Length);
            strm.Position = lastPos;
            return buffer;
        }
    }
}