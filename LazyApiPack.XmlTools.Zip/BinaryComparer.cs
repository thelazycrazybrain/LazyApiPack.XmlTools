using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace LazyApiPack.XmlTools.Zip {
    public class ByteArrayComparer : IEqualityComparer<byte[]> {
        public bool Equals(byte[]? left, byte[]? right) {
            if (left == null || right == null) {
                return left == right;
            }
            return left.SequenceEqual(right);
        }
        public int GetHashCode(byte[] key) {
            if (key == null)
                throw new ArgumentNullException("key");
            return key.Sum(b => b);
        }
    }
}