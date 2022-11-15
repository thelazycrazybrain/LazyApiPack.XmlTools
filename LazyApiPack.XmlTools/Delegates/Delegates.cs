using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using LazyApiPack.XmlTools.Helpers;

namespace LazyApiPack.XmlTools.Delegates {
    /// <summary>
    /// Delegate that is used to notify the caller that a property in the xml has no representation in the target class.
    /// </summary>
    /// <typeparam name="TExtendedXmlSerializer">The type of the Serializer.</typeparam>
    /// <param name="sender">The instance of the Xml Serializer.</param>
    /// <param name="unresolvedPropertyNode">Node that contains the unknown property.</param>
    /// <param name="targetClass">The target class that does not contain the property represented by the unresolvedPropertyNode.</param>
    public delegate void UnresolvedPropertyDelegate<TExtendedXmlSerializer>(ExtendedXmlSerializer<TExtendedXmlSerializer> sender, XElement unresolvedPropertyNode, object targetClass) where TExtendedXmlSerializer : class;

    /// <summary>
    /// Delegate that is used to notify the caller that the xml file version does not match the applications version.
    /// </summary>
    /// <typeparam name="TExtendedXmlSerializer">The type of the Serializer.</typeparam>
    /// <param name="sender">The instance of the Xml Serializer.</param>
    /// <param name="xmlAppName">Name of the app that was specified in the xml.</param>
    /// <param name="currentAppName">Name of the app given to the current xml serializer.</param>
    /// <param name="xmlAppVersion">The fileversion specified in the xml.</param>
    /// <param name="currentAppVersion">The version matchin the application version or the provided version to the Xml Serializer.</param>
    /// <param name="document">The document that needs to be converted.</param>
    /// <returns>True, if the document was converted and can be deserialized, or false, if the conversion did not succeed or was not conducted.</returns>
    public delegate bool FileVersionMismatchDelegate<TExternalXmlSerializer>(
        ExtendedXmlSerializer<TExternalXmlSerializer> sender,
        string? xmlAppName, string? currentAppName,
        Version xmlAppVersion, Version currentAppVersion,
        XDocument document) where TExternalXmlSerializer : class;
}
