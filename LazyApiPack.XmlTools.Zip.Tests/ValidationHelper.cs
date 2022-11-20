using System.Reflection;

namespace LazyApiPack.XmlTools.Zip.Tests {
    internal static class ValidationHelper {
        public static byte[] GetData(Stream stream) {
            var result = new byte[stream.Length - stream.Position];
            stream.Read(result, 0, result.Length);
            return result;
        }

        public static Stream? GetResource(string name) {
            if (name == null) return null;
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetManifestResourceStream($"LazyApiPack.XmlTools.Zip.Tests.Resources.{name}" ?? throw new NullReferenceException($"Application Resource {name} was not found."));
        }


    }
}