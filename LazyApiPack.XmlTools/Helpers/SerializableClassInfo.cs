using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using LazyApiPack.XmlTools.Attributes;

namespace LazyApiPack.XmlTools.Helpers {
    /// <summary>
    /// Represents a class that is being serialized.
    /// </summary>
    public class SerializableClassInfo {
        /// <summary>
        /// Creates an instance of the SerializableClassInfo that represents a class that is serialized with formatting information and identification.
        /// </summary>
        /// <param name="sourceObject">The object that is being serialized</param>
        /// <param name="format">The format information the serializer uses to generate the xml for this object.</param>
        /// <param name="dateTimeFormat">The date time format used in the serialized xml.</param>
        /// <param name="id">The identifier (Simple type or Guid) for this object if the Id property is specified or the object hash.</param>
        /// <param name="enableRecuriveSerialization">Enables objects to be serialized recursively (Uses an object Id).</param>
        /// <exception cref="NotSupportedException">If an Id was given that is neither a simple type nor a Guid.</exception>
        public SerializableClassInfo([NotNull] object sourceObject, IFormatProvider? format, 
                                     string? dateTimeFormat, string? id = null, bool enableRecuriveSerialization = true) {
            if (sourceObject == null) throw new ArgumentNullException(nameof(sourceObject));
            Format = format;
            Object = sourceObject;
            ClassType = sourceObject.GetType();
            if (enableRecuriveSerialization) {
                if (string.IsNullOrWhiteSpace(id)) {
                    // Check if class provides a key property (Only if the class is serializable
                    if (ClassType.GetCustomAttribute<XmlClassAttribute>() != null) {
                        var keyProperty = ClassType.GetProperties().FirstOrDefault(p => p.GetCustomAttribute<XmlClassKeyAttribute>() != null);
                        if (keyProperty != null) {
                            if (SerializationHelper.TrySerializeSimpleType(keyProperty.GetValue(Object), out var key, out var dataType, CultureInfo.InvariantCulture, dateTimeFormat)) {
                                // Use Key property
                                Id = key ?? throw new NullReferenceException($"Key in serialized document can not be null on {sourceObject?.GetType().FullName}.");
                            } else {
                                throw new NotSupportedException("Only simple types and Guids are supported for Key properties.");
                            }
                        } else {
                            Id = sourceObject.GetHashCode().ToString(CultureInfo.InvariantCulture);
                        }
                    } else {
                        // Use Hash
                        Id = sourceObject.GetHashCode().ToString(CultureInfo.InvariantCulture);
                    }
                } else {
                    // Use predefined Id
                    Id = id;
                }
            }
            SerializableClassAttribute = ClassType.GetCustomAttribute<XmlClassAttribute>();
            ClassName = string.IsNullOrWhiteSpace(SerializableClassAttribute?.CustomName) ?
                                    ClassType.Name :
                                    SerializableClassAttribute.CustomName;

            var properties = ClassType.GetProperties().Where(p => ShouldSerializeProperty(p)).ToList().AsReadOnly();
            var propertyInfos = new List<SerializablePropertyInfo>();

            foreach (var property in properties) {
                propertyInfos.Add(new SerializablePropertyInfo(property, sourceObject));
            }

            Properties = new ReadOnlyCollection<SerializablePropertyInfo>(propertyInfos);
        }


        /// <summary>
        /// Determines if the property should be serialized.
        /// </summary>
        /// <param name="property">Property to check.</param>
        /// <returns>True, if the serializer should serialize this property. Otherwise false.</returns>
        private bool ShouldSerializeProperty(PropertyInfo property) {
            var atts = property.GetCustomAttributes();
            return atts.Any(a =>
            {
                return !(a is XmlObsoleteAttribute) && 
                        a is XmlPropertyAttribute ||
                        a is XmlElementAttribute ||
                        a is XmlArrayAttribute ||
                        a is XmlAttributeAttribute;
            });
        }

        /// <summary>
        /// Data format that is used for the xml.
        /// </summary>
        public IFormatProvider? Format { get; }
        /// <summary>
        /// Identifier for this class (Enables recursive serialization).
        /// </summary>
        public string? Id { get; }
        /// <summary>
        /// The object that is serialized.
        /// </summary>
        public object Object { get; }
        /// <summary>
        /// Full name of the object class.
        /// </summary>
        public string ClassName { get; }
        /// <summary>
        /// Type of the object class.
        /// </summary>
        public Type ClassType { get; }
        /// <summary>
        /// The SerializableClassAttribute that was specified at the class declaration.
        /// </summary>
        public XmlClassAttribute? SerializableClassAttribute { get; }
        /// <summary>
        /// Properties that are serializable.
        /// </summary>
        public ReadOnlyCollection<SerializablePropertyInfo> Properties { get; }

        /// <summary>
        /// String representation of the SerializableClassInfo
        /// </summary>
        /// <returns>Full name of the ClassType or Key and full name of the ClassType.</returns>
        public override string ToString() {
            if (string.IsNullOrEmpty(Id)) {
                return $"Type: {ClassType.FullName}";
            }
            return $"Key: {Id}; Type: {ClassType.FullName}";
        }
    }
}