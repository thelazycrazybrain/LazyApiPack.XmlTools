namespace LazyApiPack.XmlTools.Helpers {
    /// <summary>
    /// Represents the xml header that is used for compatibily checks
    /// </summary>
    public class ExtendedXmlHeader {
        public ExtendedXmlHeader(string? assemblyName, Version assemblyVersion, DateTime? creationTimeStamp) {
            AssemblyName =assemblyName;
            AssemblyVersion =assemblyVersion;
            CreationTimeStamp =creationTimeStamp;
        }

        /// <summary>
        /// The Version of the assembly with which the xml was created.
        /// </summary>
        public Version AssemblyVersion { get; private set; }
        /// <summary>
        /// The Assembly that created the xml file.
        /// </summary>
        public string? AssemblyName { get; private set; }
        /// <summary>
        /// The timestamp when the xml was created.
        /// </summary>
        public DateTime? CreationTimeStamp { get; private set; }
    }
}