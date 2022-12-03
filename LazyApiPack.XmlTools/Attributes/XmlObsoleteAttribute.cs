namespace LazyApiPack.XmlTools.Attributes {
    /// <summary>
    /// This property or field will be deserialized, but not serialized again.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class XmlObsoleteAttribute : Attribute {
        /// <summary>
        /// Creates an instance of the XmlObsoleteAttribute
        /// </summary>
        public XmlObsoleteAttribute() { }

        /// <summary>
        /// Creates an instance of the XmlObsoleteAttribute
        /// </summary>
        /// <param name="reason">The reason why this property is obsolete.</param>
        public XmlObsoleteAttribute(string reason) {
            Reason = reason;

        }

        /// <summary>
        /// The reason why this property is obsolete.
        /// </summary>
        public string? Reason { get; set; }
    }
}