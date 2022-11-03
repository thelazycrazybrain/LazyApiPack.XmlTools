namespace LazyApiPack.XmlTools.Attributes
{
    /// <summary>
    /// Marks this class as serializable for the ExtendedXmlSerializer
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class XmlClassAttribute : Attribute
    {
        /// <summary>
        /// Creates an instance of the XmlClassAttribute
        /// </summary>
        public XmlClassAttribute() { }
        /// <summary>
        /// Creates an instance of the XmlClassAttribute
        /// </summary>
        /// <param name="customName">The root name of the class in the xml.</param>
        public XmlClassAttribute(string customName)
        {
            CustomName = customName;
        }
        /// <summary>
        /// The root name of the class in the xml.
        /// </summary>
        public string? CustomName { get; set; }
    }
}