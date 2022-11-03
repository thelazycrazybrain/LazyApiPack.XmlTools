namespace LazyApiPack.XmlTools.Exceptions
{
    [Serializable]
    public class ExtendedXmlFileException : Exception
    {
        public ExtendedXmlFileException() { }
        public ExtendedXmlFileException(string message) : base(message) { }
        public ExtendedXmlFileException(string message, Exception inner) : base(message, inner) { }
        protected ExtendedXmlFileException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}