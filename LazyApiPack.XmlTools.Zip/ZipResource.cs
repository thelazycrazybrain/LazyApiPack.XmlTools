namespace LazyApiPack.XmlTools.Zip {
    public class ZipResource {
        public ZipResource() {

        }

        public ZipResource(byte[]? data) : this() {
            Data = data;
        }
        public ZipResource(Stream? stream) : this() {
            SetStream(stream);
        }
        public Stream? GetStream() {
            if (_data == null) return null;
            return new MemoryStream(_data);
        }

        byte[]? _data;
        public byte[]? Data
        {
            get => _data;
            set => _data = value;
        }

        /// <summary>
        /// Sets the data with a stream (current position).
        /// </summary>
        /// <param name="stream">Stream to store.</param>
        public void SetStream(Stream? stream) {
            if (stream == null) {
                _data = null;
                return;
            }

            var pos = stream.Position;
            _data = new byte[stream.Length - stream.Position];
            stream.Read(_data, 0, _data.Length);
            if (stream.CanSeek) {
                stream.Position = pos;
            }
        }

    }
}