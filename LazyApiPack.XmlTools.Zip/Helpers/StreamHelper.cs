using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LazyApiPack.XmlTools.Zip.Helpers {
    public static class StreamHelper {
        public static Stream AsSeekable(this Stream stream, int bufferSize = 1024) {
            if (stream.CanSeek) {
                return stream;
            } else {
                byte[] data = new byte[bufferSize];
                var ms = new MemoryStream();

                int numBytesRead;
                while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0) {
                    ms.Write(data, 0, numBytesRead);

                }
                ms.Position = 0;
                return ms;

            }
        }
    }
}
