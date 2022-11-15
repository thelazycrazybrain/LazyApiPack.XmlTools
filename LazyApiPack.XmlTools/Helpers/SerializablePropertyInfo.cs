using LazyApiPack.XmlTools.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Helpers {
    /// <summary>
    /// Represents a property that is being serialized.
    /// </summary>
    public class SerializablePropertyInfo {
        /// <summary>
        /// Creates an instance of the SerializablePropertyInfo that represents a property that is serialized with formatting information and identification.
        /// </summary>
        /// <param name="property">The property information of the instance.</param>
        /// <param name="instance">The object that is being serialized.</param>
        public SerializablePropertyInfo([NotNull] PropertyInfo property, object instance) {
            PropertyInfo = property;
            PropertyName = GetCustomPropertyName();
            PropertyType = property.PropertyType;
            ArrayPropertyItemName = property.GetCustomAttribute<XmlArrayItemAttribute>()?.ElementName ?? "Item";
            PropertyValue = property.GetValue(instance);
            IsXmlAttribute =  property.GetCustomAttribute<XmlAttributeAttribute>() != null;
            IsInstantiable =  !(PropertyType.IsInterface || PropertyType.IsAbstract);

        }

        /// <summary>
        /// Gets the name of the property
        /// </summary>
        /// <returns>Either the name that was specified with the XmlPropertyAttribute, the XmlElementAttribute, the XmlArrayAttribute, the XmlAttributeAttribute or the name of the property itself.</returns>
        private string GetCustomPropertyName() {

            string customName = null;
            if (PropertyInfo.GetCustomAttribute<XmlPropertyAttribute>() is XmlPropertyAttribute pa) {
                customName = pa.CustomName;
            } else if (PropertyInfo.GetCustomAttribute<XmlElementAttribute>() is XmlElementAttribute ea) {
                customName = ea.ElementName;
            } else if (PropertyInfo.GetCustomAttribute<XmlArrayAttribute>() is XmlArrayAttribute xa) {
                customName = xa.ElementName;
            } else if (PropertyInfo.GetCustomAttribute<XmlAttributeAttribute>() is XmlAttributeAttribute xaa) {
                customName = xaa.AttributeName;
            }

            if (string.IsNullOrWhiteSpace(customName)) {
                customName = PropertyInfo.Name;
            }

            return customName;
        }

        /// <summary>
        /// The PropertyInfo that is represented by this object.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }
        /// <summary>
        /// The type of the Property.
        /// </summary>
        public Type PropertyType { get; }
        /// <summary>
        /// Is false, if the class can not be instantiated (abstract or interface).
        /// </summary>
        public bool IsInstantiable { get; }
        /// <summary>
        /// The name of the property.
        /// </summary>
        public string PropertyName { get; }
        /// <summary>
        /// The name of the item of an array.
        /// </summary>
        public string ArrayPropertyItemName { get; }
        /// <summary>
        /// The name of the property type.
        /// </summary>
        public string PropertyTypeName { get => PropertyInfo.PropertyType.Name; }

        /// <summary>
        /// The full name of the property type.
        /// </summary>
        public string PropertyTypeFullName { get => PropertyInfo.PropertyType.FullName ?? PropertyTypeName; }
        /// <summary>
        /// The value of the property of the current instance.
        /// </summary>
        public object? PropertyValue { get; }
        /// <summary>
        /// Is true, if this property is serialized as an XmlAttribute.
        /// </summary>
        public bool IsXmlAttribute { get; }

        public override string ToString() {
            return $"{PropertyName}: {PropertyType.FullName ?? PropertyType.Name}";
        }

    }
}