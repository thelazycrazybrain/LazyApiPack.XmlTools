using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace LazyApiPack.XmlTools.Zip.Tests {
    internal static class ZipValidationHelper {
        public static Stream GetResource([NotNull] string name) {
            if (name == null) throw new ArgumentNullException(nameof(name));
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetManifestResourceStream($"LazyApiPack.XmlTools.Zip.Tests.Resources.{name}") ?? throw new NullReferenceException($"Application Resource {name} was not found.");
        }

        public static byte[] GetResourceBytes([NotNull]string name) {
            if (name == null) throw new ArgumentNullException(nameof(name));
            var asm = Assembly.GetExecutingAssembly();
            var strm = asm.GetManifestResourceStream($"LazyApiPack.XmlTools.Zip.Tests.Resources.{name}") ?? throw new NullReferenceException($"Application Resource {name} was not found.");
            var buffer = new byte[strm.Length];
            strm.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}