namespace LazyApiPack.XmlTools.Exceptions
{
    [Serializable]
    public class ExtendedXmlSerializationException : Exception
    {
        public ExtendedXmlSerializationException() { }
        public ExtendedXmlSerializationException(string message) : base(message) { }
        public ExtendedXmlSerializationException(string message, Exception inner) : base(message, inner) { }
        protected ExtendedXmlSerializationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}