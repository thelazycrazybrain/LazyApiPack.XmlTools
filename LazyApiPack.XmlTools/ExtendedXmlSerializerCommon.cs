using LazyApiPack.XmlTools.Delegates;
using LazyApiPack.XmlTools.Exceptions;
using LazyApiPack.XmlTools.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools {
    public partial class ExtendedXmlSerializer<TClass> : ExtendedXmlSerializer where TClass : class {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. _writer is set in Serialize() method
        private ExtendedXmlSerializer() {
            _serializedObjects = new List<SerializableClassInfo>();
            _deserializedObjects = new List<SerializedClassContainer>();
            _cachedTypes = new Dictionary<string, Type>();
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

        /// <summary>
        /// Creates an instance of the extended xml serializer.
        /// </summary>
        /// <param name="enableRecursiveSerialization">Enables objects to be serialized recurively with an Id</param>
        /// <param name="cultureInfo">Localized format for numbers, dates etc. used in the xml.</param>
        /// <param name="dateTimeFormat">Format in which DateTimes are serialized in the xml.</param>
        /// <param name="appName">Name of the app for compatiblity checks.</param>
        /// <param name="appVersion">Version of the app for compatibility checks.</param>
        /// <param name="xmlFormatting">Xml Format</param>
        public ExtendedXmlSerializer(bool enableRecursiveSerialization = true,
                                     CultureInfo? cultureInfo = null,
                                     string? dateTimeFormat = null,
                                     string? appName = null,
                                     Version? appVersion = null,
                                     Formatting xmlFormatting = Formatting.Indented,
                                     bool useFullNamespace = true) : this() {
            EnableRecursiveSerialization = enableRecursiveSerialization;
            CultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
            DateTimeFormat = dateTimeFormat;

#pragma warning disable CS8601 // Possible null reference assignment. If null is specified, the Getter provides a fallback value
            AppVersion = appVersion;
#pragma warning restore CS8601 // Possible null reference assignment.

            AppName = appName;
            XmlFormatting = xmlFormatting;
            UseFullNamespace = useFullNamespace;
        }

        #region Properties
        /// <summary>
        /// The formatting rule for the produced xml.
        /// </summary>
        public Formatting XmlFormatting { get; set; } = Formatting.Indented;
        /// <summary>
        /// The DateTime format that the serializer uses to represent dates in the xml.
        /// </summary>
        public string? DateTimeFormat { get; set; } = null;
        /// <summary>
        /// If enabled, recursive serialization is enabled
        /// </summary>
        public bool EnableRecursiveSerialization { get; set; } = true;

        /// <summary>
        /// Contains a list of serializer extensions that are used to serialize and deserialize custom types.
        /// </summary>
        public List<IExternalObjectSerializer> ExternalSerializers { get; } = new List<IExternalObjectSerializer>();
        /// <summary>
        /// Specifies if the serializer uses the full namespace or just the class name.
        /// </summary>
        /// <remarks>Use False, if you want to serialize / deserialize in different programs with different namespaces.</remarks>
        /// <remarks>If true, the classes need to be in the same namespace as the root class.</remarks>
        public bool UseFullNamespace { get; set; }


        string _rootElementName = "ExtendedSerializedObjectFile";
        /// <summary>
        /// Name of the root element.
        /// </summary>
        public string RootElementName
        {
            get { return _rootElementName; }
            set
            {
                if (IsValidElementName(value)) {
                    _rootElementName = value;
                } else {
                    throw new FormatException($"{value} is not a valid name for an element.");
                }
            }
        }

        /// <summary>
        /// The format that the serializer uses for the produced xml.
        /// </summary>
        public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;

        Version? _appVersion;
        /// <summary>
        /// Version of the app or the assembly version of the entry assembly.
        /// </summary>
        public Version AppVersion
        {
            get => _appVersion ?? Assembly.GetEntryAssembly()?.GetName().Version ?? throw new NullReferenceException("App version is not specified and can not determined.");
            set => _appVersion = value;
        }
        string? _appName;
        /// <summary>
        /// Name of the app or the assembly title of the entry assembly.
        /// </summary>
        public string? AppName
        {
            get => !string.IsNullOrEmpty(_appName) ?
                    _appName :
                    Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
            set => _appName = value;

        }

        /// <summary>
        /// Contains a cache of the classes that have been already serialized (to support xml compression and recursive serialization.
        /// </summary>
        /// <remarks>Is only used when recursive serialization is enabled.</remarks>
        private readonly List<SerializableClassInfo> _serializedObjects;
        /// <summary>
        /// Contains a cache of the classes that have been already deserialized (to support xml compression and recursive serialization.
        /// </summary>
        /// <remarks>Is only used when recursive serialization is enabled.</remarks>
        private readonly List<SerializedClassContainer> _deserializedObjects;
        /// <summary>
        /// Types, that have already been reflected.
        /// </summary>
        private readonly Dictionary<string, Type> _cachedTypes;
        #endregion

        #region Helper Functions

        /// <summary>
        /// Returns a cached type or creates a cached item.
        /// </summary>
        /// <param name="fullOrRelativeTypeName">Full or relative name of the type.</param>
        /// <param name="assemblyName">Contains the namespace of the type, if the fullOrRelativeTypeName is relative.</param>
        /// <returns>The type of the typeName.</returns>
        /// <exception cref="ExtendedXmlSerializationException">Is thrown when the type is not known to the Assembly or AppDomain.</exception>
        private Type GetCachedType(string fullOrRelativeTypeName, string? assemblyName) {
            if (!UseFullNamespace) {
                fullOrRelativeTypeName = string.IsNullOrEmpty(assemblyName) ? fullOrRelativeTypeName : (assemblyName  + "." +  fullOrRelativeTypeName);
            }

            if (_cachedTypes.ContainsKey(fullOrRelativeTypeName)) {
                return _cachedTypes[fullOrRelativeTypeName];
            } else {
                var type = Type.GetType(fullOrRelativeTypeName);
                if (type == null) {
                    type = GetTypeFromDomain(fullOrRelativeTypeName);
                    if (type == null) {
                        throw new ExtendedXmlSerializationException($"Type {fullOrRelativeTypeName} could neither be found in the executing assembly nor in the appdomain.");
                    }
                }
                _cachedTypes.Add(fullOrRelativeTypeName, type);
                return type;
            }
        }

        /// <summary>
        /// Gets the class type from AppDomain.
        /// </summary>
        /// <param name="fullName">Full name of the class.</param>
        /// <returns>The type of the requested class or null if the type is unknown to the AppDomain.</returns>
        private Type? GetTypeFromDomain(string fullName) {
            return AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(fullName)).FirstOrDefault(a => a != null);
        }

        /// <summary>
        /// Clears the Serialized and Deserialized objects cache.
        /// </summary>
        private void ClearSerializationCache() {
            _serializedObjects?.Clear();
            _deserializedObjects?.Clear();
            _cachedTypes?.Clear();
        }

        /// <summary>
        /// Writes the xml header that is needed for xml migration
        /// </summary>
        /// <param name="header">Header that is written to the xml.</param>
        private void WriteHeader(ExtendedXmlHeader header) {
            _writer.WriteStartElement("Header");
            _writer.WriteAttributeString("AppName", header.AppName);
            _writer.WriteAttributeString("AppVersion", header.AppVersion.ToString(4));
            _writer.WriteEndElement();
        }


        /// <summary>
        /// Gets the DateTime from a formatted string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DateTime? ParseFormattedDateTime(string? value) {
            if (string.IsNullOrEmpty(value)) return null;
            if (!string.IsNullOrWhiteSpace(DateTimeFormat)) {
                return DateTime.ParseExact(value, DateTimeFormat, CultureInfo);
            }
            return DateTime.Parse(value, CultureInfo);

        }

        /// <summary>
        /// Determines, if the serializer can treat this type as a value
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>True, if the Serializer can treat this type as a value type</returns>
        /// <remarks>The serializer considers some reference types and complex types such as Point, DateTime or Version as value types.</remarks>
        public bool IsSimpleType(Type objectType) {
            return objectType.IsValueType ||
                   objectType.IsEnum ||
                   objectType == typeof(string) ||
                   objectType == typeof(Version) ||
                   objectType == typeof(Point) ||
                   objectType == typeof(Size) ||
                   objectType == typeof(DateTime) ||
                   objectType == typeof(TimeSpan);
        }

        /// <summary>
        /// Returns the header information from the given xml element.
        /// </summary>
        /// <param name="headerElement">Xml Element representing the ExtendedXmlHeader.</param>
        /// <returns>The ExtendedXmlHeader or null, if the xml does not contain a header.</returns>
        /// <exception cref="ExtendedXmlFileException">Is thrown, if the header is in an invalid format.</exception>
        private ExtendedXmlHeader? GetHeader([NotNull] XElement headerElement) {
            try {
                return new ExtendedXmlHeader(
                                headerElement.Attribute(XName.Get("AppName"))?.Value
                                                                        ?? throw new NullReferenceException("AppName missing in xml."),
                                Version.Parse(headerElement.Attribute(XName.Get("AppVersion"))?.Value
                                                                        ?? throw new NullReferenceException("AppVersion missing in xml.")));
            } catch {
                throw new ExtendedXmlFileException("Invalid Header Format");
            }
        }

        /// <summary>
        /// Checks if the version in the xml does not match the current app version.
        /// </summary>
        /// <returns>-1 -> given version is smaller; 0 given version is equal; 1 given version is greater</returns>
        int VersionMismatch(ExtendedXmlHeader header) {
            if (header.AppVersion < AppVersion) return -1;
            if (header.AppVersion > AppVersion) return 1;
            return 0;
        }

        /// <summary>
        /// Checks if a given element name is suited for a well-formated xml.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>True, if the name of the element is suited for a well-formated xml.</returns>
        bool IsValidElementName(string name) {
            // Check first char
            if (string.IsNullOrWhiteSpace(name) || !(char.IsLetter(name[0]) || name[0] == '_') || name.ToUpper().StartsWith("XML")) {
                return false;
            } else {
                // Check for invalid characters
                return new Regex(@"^[a-zA-Z0-9-_(.)]*$").IsMatch(name);
            }
        }

        /// <summary>
        /// Gets the full or relative name of the type.
        /// </summary>
        /// <param name="type">Type to determine the name.</param>
        /// <returns>Full or relative name of type</returns>
        string? GetTypeName(Type type) {
            return UseFullNamespace ? type.FullName : type.Name;
        }
        #endregion

    }
}