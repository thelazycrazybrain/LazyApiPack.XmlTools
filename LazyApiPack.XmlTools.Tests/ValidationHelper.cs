using System.Reflection;
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

        internal static Stream? GetResource(string name) {
            if (name == null) return null;
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetManifestResourceStream($"LazyApiPack.XmlTools.Tests.Resources.{name}" ?? throw new NullReferenceException($"Application Resource {name} was not found."));
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