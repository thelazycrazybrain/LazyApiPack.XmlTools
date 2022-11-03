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
    public partial class ExtendedXmlSerializer<T> where T : class {
        public ExtendedXmlSerializer() {
            _serializedObjects = new List<SerializableClassInfo>();
            _deserializedObjects = new List<SerializedClassContainer>();
            _cachedTypes = new Dictionary<string, Type>();
        }
        /// <summary>
        /// Creates an instance of the ExtendedXmlSerializer
        /// </summary>
        /// <param name="suppressId">If true, the serializer supports recursive serialization.</param>
        public ExtendedXmlSerializer(bool suppressId) : this() {
            SuppressId = suppressId;
        }

        /// <summary>
        /// Creates an instance of the ExtendedXmlSerializer.
        /// </summary>
        /// <param name="cultureInfo">Format that this instance uses to serialize and deserialize the xml.</param>
        /// <param name="suppressId">If true, the serializer supports recursive serialization.</param>
        public ExtendedXmlSerializer(CultureInfo cultureInfo, bool suppressId) : this(suppressId) {
            CultureInfo = cultureInfo;

        }

        /// <summary>
        /// Creates an instance of the ExtendedXmlSerializer.
        /// </summary>
        /// <param name="cultureInfo">Format that this instance uses to serialize and deserialize the xml.</param>
        /// <param name="dateTimeFormat">Format that this instance uses to serialize and deserialize DateTimes.</param>
        /// <param name="suppressId">If true, the serializer supports recursive serialization.</param>
        public ExtendedXmlSerializer(CultureInfo cultureInfo, string dateTimeFormat, bool suppressId) : this(cultureInfo, suppressId) {
            DateTimeFormat = dateTimeFormat;
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
        /// If the Id is suppressed, recursive serialization / deserialization is not supported.
        /// </summary>
        public bool SuppressId { get; set; } = false;

        /// <summary>
        /// Contains a list of serializer extensions that are used to serialize and deserialize custom types.
        /// </summary>
        public List<IExternalObjectSerializer> ExternalSerializers { get; } = new List<IExternalObjectSerializer>();
        /// <summary>
        /// Specifies if the serializer uses the full namespace or just the class name.
        /// </summary>
        /// <remarks>Use False, if you want to serialize / deserialize in different programs with different namespaces.</remarks>
        /// <remarks>If true, the classes need to be in the same namespace as the root class.</remarks>
        public bool UseFullNamespace { get; set; } = true;


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

        /// <summary>
        /// The current assembly version of the program (for migration support).
        /// </summary>
        public Version AssemblyVersion
        {
            get
            {
                if (_overwrittenAssemblyVersion != null) {
                    return _overwrittenAssemblyVersion;
                } else {
                    return _callingAssemblyVersion;
                }
            }
            set
            {
                _overwrittenAssemblyVersion = value;
            }
        }

        /// <summary>
        /// Assembly name of the program (for compatibility checks)
        /// </summary>
        public string AssemblyName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_overwrittenAssemblyName)) {
                    return _overwrittenAssemblyName;
                } else {
                    return _callingAssemblyName;
                }
            }
            set
            {
                _overwrittenAssemblyName = value;
            }
        }

        /// <summary>
        /// If version is overwritten, this version will be used instead of the Assembly Version.
        /// </summary>
        Version _overwrittenAssemblyVersion = null;
        /// <summary>
        /// Version of the calling assembly.
        /// </summary>
        Version _callingAssemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
        /// <summary>
        /// If name is overwritten, this assembly name will be used instead of Assembly Name.
        /// </summary>
        string _overwrittenAssemblyName = null;
        /// <summary>
        /// Title of the assembly.
        /// </summary>
        string _callingAssemblyName = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        /// <summary>
        /// Contains a cache of the classes that have been already serialized (to support xml compression and recursive serialization.
        /// </summary>
        /// <remarks>Is only used when SuppressId is set to False</remarks>
        List<SerializableClassInfo> _serializedObjects;
        /// <summary>
        /// Contains a cache of the classes that have been already deserialized (to support xml compression and recursive serialization.
        /// </summary>
        /// <remarks>Is only used when SuppressId is set to False</remarks>
        List<SerializedClassContainer> _deserializedObjects;
        /// <summary>
        /// Types, that have already been reflected.
        /// </summary>
        Dictionary<string, Type> _cachedTypes;
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
        /// Writes the xml root header with the assembly name, version and timestamp.
        /// </summary>
        /// <param name="header">Header that is written to the xml.</param>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        private void WriteHeader(ExtendedXmlHeader header, XmlWriter writer) {
            writer.WriteStartElement("Header");
            writer.WriteAttributeString("AssemblyName", header.AssemblyName);
            writer.WriteAttributeString("AssemblyVersion", header.AssemblyVersion.ToString(4));
            writer.WriteAttributeString("CreationTimeStamp", SerializationHelper.DateTimeToFormattedString(header.CreationTimeStamp, CultureInfo, DateTimeFormat));
            writer.WriteEndElement();
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
        public bool IsValueType(Type objectType) {
            return objectType.IsValueType ||
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
                var header = new ExtendedXmlHeader();
                header.AssemblyName = headerElement.Attribute(XName.Get("AssemblyName"))?.Value ?? throw new NullReferenceException("AssemblyName missing in xml.");
                header.AssemblyVersion = Version.Parse(headerElement.Attribute(XName.Get("AssemblyVersion"))?.Value ?? throw new NullReferenceException("AssemblyVersion missing in xml."));
                header.CreationTimeStamp = ParseFormattedDateTime(headerElement.Attribute(XName.Get("CreationTimeStamp"))?.Value);
                return header;
            } catch {
                throw new ExtendedXmlFileException("Invalid Header Format");
            }
        }

        /// <summary>
        /// Checks the version of the given header against the current assembly version
        /// </summary>
        /// <returns>-1 -> given version is smaller; 0 given version is equal; 1 given version is greater</returns>
        int VersionMismatch(ExtendedXmlHeader header) {
            if (header.AssemblyVersion < AssemblyVersion) {
                return -1;
            } else if (header.AssemblyVersion == AssemblyVersion) {
                return 0;
            } else {
                return 1;
            }
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

        #endregion

    }
}