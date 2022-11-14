using System.Collections;
using System.Net.NetworkInformation;

namespace LazyApiPack.XmlTools.Tests {
    internal class BinaryComparer : IComparer {
        public int Compare(object? x, object? y) {
            if (x is byte xx && y is byte yy) {
                return xx - yy;
            } else {
                throw new NotSupportedException();
            }
        }
    }
}