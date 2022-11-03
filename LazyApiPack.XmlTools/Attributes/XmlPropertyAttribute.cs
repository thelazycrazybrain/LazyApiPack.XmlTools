namespace LazyApiPack.XmlTools.Attributes
{

    /// <summary>
    /// Marks this property or field as serializable for the ExtendedXmlSerializer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class XmlPropertyAttribute : Attribute
    {
        /// <summary>
        /// Creates an instance of the XmlPropertyAttribute
        /// </summary>
        public XmlPropertyAttribute() { }

        /// <summary>
        /// Creates an instance of the XmlPropertyAttribute
        /// </summary>
        /// <param name="customName">The name of this property or field used in the xml.</param>
        public XmlPropertyAttribute(string customName)
        {
            CustomName = customName;

        }

        /// <summary>
        /// The name of this property or field used in the xml.
        /// </summary>
        public string CustomName { get; set; }
    }
}