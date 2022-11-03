namespace LazyApiPack.XmlTools.Helpers {
    /// <summary>
    /// Represents the xml header that is used for compatibily checks
    /// </summary>
    public class ExtendedXmlHeader {
        /// <summary>
        /// The Version of the assembly with which the xml was created.
        /// </summary>
        public Version AssemblyVersion { get; set; }
        /// <summary>
        /// The Assembly that created the xml file.
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        /// The timestamp when the xml was created.
        /// </summary>
        public DateTime? CreationTimeStamp { get; set; }
    }
}