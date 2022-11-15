namespace LazyApiPack.XmlTools.Helpers {
    /// <summary>
    /// Represents the xml header that is used for compatibily checks
    /// </summary>
    public class ExtendedXmlHeader {
        /// <summary>
        /// Header for the xml to provide functionality for xml migration.
        /// </summary>
        /// <param name="appName">Name of the app that created the xml.</param>
        public ExtendedXmlHeader(string? appName, Version appVersion) {
            AppName =appName;
            AppVersion =appVersion;
        }

        /// <summary>
        /// Version of the app that created the xml
        /// </summary>
        public Version AppVersion { get; private set; }
        /// <summary>
        /// Name of the app that created the xml.
        /// </summary>
        public string? AppName { get; private set; }
    }
}