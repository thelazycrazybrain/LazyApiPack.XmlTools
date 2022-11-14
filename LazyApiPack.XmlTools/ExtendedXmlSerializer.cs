using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Exceptions;
using LazyApiPack.XmlTools.Helpers;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace LazyApiPack.XmlTools {
    /// <summary>
    /// Serializes and deserializes classes to xml
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class ExtendedXmlSerializer<T> where T : class {
        public void Serialize(object sourceClass, string fileName) {
            using (var serialized = Serialize(sourceClass))
            using (var fs = File.OpenWrite(fileName)) {
                fs.SetLength(serialized.Length);
                serialized.CopyTo(fs);
            }
        }

        /// <summary>
        /// Serializes a class to an xml stream
        /// </summary>
        /// <param name="sourceClass">The class to serialize</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">If sourceclass is null.</exception>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        public Stream Serialize([NotNull] object sourceClass) {
            try {
                if (sourceClass == null) throw new NullReferenceException("Can not serialize a class which is null.");
                var targetStream = new MemoryStream();

                using (var ms = new MemoryStream()) {
                    var xset = new XmlWriterSettings() {
                        Encoding = Encoding.UTF8,
                        Indent = true
                    };


                    using (var writer = XmlWriter.Create(ms, xset)) {

                        writer.WriteStartDocument();
                        writer.WriteStartElement(_rootElementName);
                        writer.WriteAttributeString("xmlns", "lzyxmlx", null, "http://www.jodiewatson.net/xml/lzyxmlx/1.0");


                        var header = new ExtendedXmlHeader(AssemblyName, AssemblyVersion, DateTime.Now);

                        WriteHeader(header, writer);
                        var clsInfo = new SerializableClassInfo(sourceClass, CultureInfo, DateTimeFormat, null, SuppressId);
                        if (clsInfo.SerializableClassAttribute == null) {
                            throw new ExtendedXmlSerializationException($"Cannot serialize the class {sourceClass.GetType().FullName}. It does not specify the SerializableClass Attribute.");
                        }
                        SerializeClass(writer, clsInfo, false);

                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Flush();
                        ms.Position = 0;
                        targetStream.SetLength(ms.Length);
                        ms.CopyTo(targetStream);
                    }
                }

                targetStream.Position = 0;
                return targetStream;
            }
            finally {
                ClearSerializationCache();
            }
        }

        /// <summary>
        /// Serializes an object
        /// </summary>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        /// <param name="classInfo">The class info representing the object.</param>
        /// <param name="isAbstractOrInterface">If the class is abstract or an interface, the clsType attribute is set to make the object deserializable.</param>
        /// <param name="propertyName">The name of the Xml Element. Null if the class is the root class of the xml.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private void SerializeClass(XmlWriter writer, SerializableClassInfo classInfo, bool isAbstractOrInterface,
                                    string? propertyName = null, Action<XmlWriter>? writeAttributeToCurrentElement = null) {
            if (classInfo.Object == null) return;
            writer.WriteStartElement(string.IsNullOrWhiteSpace(propertyName) ? classInfo.ClassName : propertyName);
            if (!SuppressId) {
                writer.WriteAttributeString("objId", "http://www.jodiewatson.net/xml/lzyxmlx/1.0", classInfo.Id);
            }
            if (isAbstractOrInterface) {
                writer.WriteAttributeString("clsType", "http://www.jodiewatson.net/xml/lzyxmlx/1.0", UseFullNamespace ? classInfo.ClassType.FullName : classInfo.ClassType.Name);
            }

            writeAttributeToCurrentElement?.Invoke(writer);
            if (!SuppressId) {
                if (_serializedObjects.Any(c => c.Id == classInfo.Id && c.ClassType.FullName == classInfo.ClassType.FullName)) {
                    // This class was already serialized
                    writer.WriteEndElement();
                    return;
                }
            }
            // Class was not serialized or ID is not supported (Non Recursive Serialization)
            _serializedObjects.Add(classInfo);

            var extendedClass = classInfo.Object as IExtendedXmlSerializable;
            extendedClass?.OnSerializing();
            try {
                // Serialize Properties
                foreach (var propertyInfo in classInfo.Properties.OrderByDescending(x => x.IsXmlAttribute).ThenBy(x => GetPropertySerializationPriority(x))) {
                    SerializeProperty(writer, propertyInfo, propertyInfo.PropertyValue, propertyInfo.PropertyName, propertyInfo.PropertyTypeName, propertyInfo.PropertyType, propertyInfo.PropertyInfo.PropertyType.IsInterface, null);

                }
                writer.WriteEndElement();
            } catch (Exception ex) {
                extendedClass?.OnSerialized(false);
                throw new ExtendedXmlSerializationException("Serialization failed. See inner exception for more details.", ex);
            }
            extendedClass?.OnSerialized(true);

        }


        /// <summary>
        /// Serializes a property.
        /// </summary>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        /// <param name="propertyInfo">The property that is serialized.</param>
        /// <param name="value">The property value that is serialized.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyTypeName">Full name of the property type.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="isAbstractOrInterface">If the class is abstract or an interface, the clsType attribute is set to make the object deserializable.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private void SerializeProperty(XmlWriter writer, SerializablePropertyInfo propertyInfo, object? value,
                                       string propertyName, string propertyTypeName, Type propertyType,
                                       bool isAbstractOrInterface, Action<XmlWriter>? writeAttributeToCurrentElement) {
            if (value == null) return; // Do not serialize null values

            if (propertyType.IsArray) {
                if (propertyType.GetElementType() == typeof(byte) && propertyType.GetArrayRank() == 1) {
                    // Exception: Byte arrays will be stored as base64 blobs
                    SerializeBinary(writer, (byte[])value, propertyName, writeAttributeToCurrentElement);
                } else {
                    SerializeArrayProperty(writer, value, propertyInfo, propertyName, writeAttributeToCurrentElement);
                }
            } else if (propertyType.IsGenericType) {
                SerializeGenericProperty(writer, value, propertyInfo, propertyName, propertyTypeName, propertyType, writeAttributeToCurrentElement);
            } else if (propertyType.IsEnum) {
                SerializeEnumProperty(writer, (Enum)value, propertyName, writeAttributeToCurrentElement);
            } else if (IsValueType(propertyType)) {
                SerializeValueType(writer, value, propertyInfo, propertyName, writeAttributeToCurrentElement);
            } else {
                // Is Class
                var propCi = new SerializableClassInfo(value, CultureInfo, DateTimeFormat, null, SuppressId);
                if (propCi.SerializableClassAttribute != null) {
                    SerializeClass(writer, propCi, isAbstractOrInterface, propertyName);
                } else {
                    SerializePropertyExternal(writer, propCi.Object, propertyInfo.IsXmlAttribute, propertyName, propCi.ClassType);
                }

            }
        }

        /// <summary>
        /// Serializes the value as base64.
        /// </summary>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        /// <param name="value">Byte array that is serialized.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        private void SerializeBinary(XmlWriter writer, byte[] value, string propertyName, Action<XmlWriter>? writeAttributeToCurrentElement) {
            writer.WriteStartElement(propertyName);
            writeAttributeToCurrentElement?.Invoke(writer);
            writer.WriteAttributeString("format", "http://www.jodiewatson.net/xml/lzyxmlx/1.0", "Base64");
            writer.WriteValue(Convert.ToBase64String(value));
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the value as an enum value to the xml
        /// </summary>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        /// <param name="value">Enum that is serialized.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        private void SerializeEnumProperty(XmlWriter writer, Enum value, string propertyName, Action<XmlWriter>? writeAttributeToCurrentElement) {
            writer.WriteStartElement(propertyName);
            writeAttributeToCurrentElement?.Invoke(writer);
            writer.WriteValue(value.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the value as an array.
        /// </summary>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        /// <param name="value">Array that is serialized.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        private void SerializeArrayProperty(XmlWriter writer, [NotNull] object value, SerializablePropertyInfo propertyInfo, string propertyName, Action<XmlWriter>? writeAttributeToCurrentElement) {
            var array = ((Array)value);
            writer.WriteStartElement(propertyName);
            writeAttributeToCurrentElement?.Invoke(writer);
            var rankDescriptor = "";
            for (int i = 0; i < array.Rank; i++) {
                rankDescriptor += array.GetLength(i) + ";";
            }
            rankDescriptor = rankDescriptor.TrimEnd(';');
            writer.WriteAttributeString("rankDescriptor", "http://www.jodiewatson.net/xml/lzyxmlx/1.0", rankDescriptor);
            var arrayIterator = new SerializableArray(array);
            while (arrayIterator.MoveNext()) {
                SerializeProperty(writer, propertyInfo, arrayIterator.Current, propertyInfo.ArrayPropertyItemName,
                    arrayIterator.ItemType.Name, arrayIterator.ItemType, arrayIterator.ItemType.IsInterface ||
                    arrayIterator.ItemType.IsAbstract, (w) => writer.WriteAttributeString("index",
                        "http://www.jodiewatson.net/xml/lzyxmlx/1.0", arrayIterator.CurrentIndexString));
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the value as a generic type.
        /// </summary>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        /// <param name="value">Generic value that is serialized.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyTypeName">Full name of the property type.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        /// <exception cref="ExtendedXmlSerializationException">If the generic type is not supported.</exception>
        /// <remarks>Supported Generics: Nullable, IList, Dictionary</remarks>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private void SerializeGenericProperty(XmlWriter writer, [NotNull] object value, SerializablePropertyInfo propertyInfo, string propertyName, string propertyTypeName, Type propertyType, Action<XmlWriter>? writeAttributeToCurrentElement) {
            var typedef = propertyType.GetGenericTypeDefinition();
            var typeArgs = propertyType.GetGenericArguments();

            if (typedef == typeof(Nullable<>)) {
                // Decapsulate value type
                propertyType = typeArgs[0];
                propertyTypeName = propertyType.Name;
                SerializeProperty(writer, propertyInfo, value, propertyName, propertyTypeName, propertyType, propertyType.IsInterface || propertyType.IsAbstract, writeAttributeToCurrentElement);
            } else if (typeof(IList).IsAssignableFrom(typedef)) {
                propertyType = typeArgs[0];
                propertyTypeName = propertyType.Name;
                SerializeListProperty(writer, (IList)value, propertyInfo, propertyName, propertyTypeName, propertyType, propertyType.IsInterface || propertyType.IsAbstract, writeAttributeToCurrentElement);
            } else if (typedef == typeof(Dictionary<,>)) {
                var att = propertyType.GetCustomAttribute<DictionaryEqualityComparerAttribute>();
                var comparerType = att?.EqualityComparerType;
                SerializeDictionary(writer, (IDictionary)value, typeArgs[0], typeArgs[1], propertyInfo, propertyName, comparerType, writeAttributeToCurrentElement);
            } else {
                if (Debugger.IsAttached) {
                    Debugger.Break();
                } else {
                    throw new ExtendedXmlSerializationException($"The generic Type {propertyTypeName} of property {propertyName} is not supported.");
                }
            }
        }

        /// <summary>
        /// Serializes the value as a Dictionary.
        /// </summary>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        /// <param name="value">The dictionary that is serialized.</param>
        /// <param name="keyType">The type of the key.</param>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="equalityComparer">If the DictionaryEqualityComparerAttribute is present, this type will be stored with the dictionary element.</param>
        /// <exception cref="ExtendedXmlSerializationException">If the generic type is not supported.</exception>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private void SerializeDictionary(XmlWriter writer, IDictionary value, Type keyType, Type valueType, SerializablePropertyInfo propertyInfo, string propertyName, Type? equalityComparer, Action<XmlWriter>? writeAttributeToCurrentElement) {
            writer.WriteStartElement(propertyName);
            writer.WriteAttributeString("keyType", "http://www.jodiewatson.net/xml/lzyxmlx/1.0", keyType.FullName);
            writer.WriteAttributeString("valueType", "http://www.jodiewatson.net/xml/lzyxmlx/1.0", valueType.FullName);
            if (equalityComparer != null) {
                writer.WriteAttributeString("comparerType", "http://www.jodiewatson.net/xml/lzyxmlx/1.0", equalityComparer.FullName);
            }

            writeAttributeToCurrentElement?.Invoke(writer);
            foreach (DictionaryEntry item in value) {
                writer.WriteStartElement("Item");
                SerializeProperty(writer, propertyInfo, item.Key, "Key", keyType.FullName ?? keyType.Name, keyType, keyType.IsInterface || keyType.IsAbstract, null);
                SerializeProperty(writer, propertyInfo, item.Value, "Value", valueType.FullName ?? keyType.Name, valueType, valueType.IsInterface || valueType.IsAbstract, null);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the value as an IList.
        /// </summary>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        /// <param name="value">The List that is serialized.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyTypeName">Full name of the property type.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="isAbstractOrInterface">If the class is abstract or an interface, the clsType attribute is set to make the object deserializable.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private void SerializeListProperty(XmlWriter writer, IList value, SerializablePropertyInfo propertyInfo, string propertyName, string propertyTypeName, Type propertyType, bool isAbstractOrInterface, Action<XmlWriter>? writeAttributeToCurrentElement) {
            writer.WriteStartElement(propertyName);
            foreach (var item in value) {
                SerializeProperty(writer, propertyInfo, item, propertyInfo.ArrayPropertyItemName, propertyTypeName, propertyType, isAbstractOrInterface, writeAttributeToCurrentElement);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the value as a value type.
        /// </summary>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        /// <param name="value">The value type that is serialized.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private void SerializeValueType(XmlWriter writer, object value, SerializablePropertyInfo propertyInfo, string propertyName, Action<XmlWriter>? writeAttributeToCurrentElement) {
            if (SerializationHelper.TrySerializeSimpleType(value, out string? serializedSimpleType, out string? serializedSimpleTypeDataType, CultureInfo, DateTimeFormat)) {
                if (propertyInfo.IsXmlAttribute) {
                    writeAttributeToCurrentElement?.Invoke(writer);
                    writer.WriteAttributeString(propertyName, serializedSimpleType);
                } else {
                    writer.WriteStartElement(propertyName);
                    writeAttributeToCurrentElement?.Invoke(writer);
                    writer.WriteValue(serializedSimpleType);
                    writer.WriteEndElement();
                }
            } else {
                var propCi = new SerializableClassInfo(value, CultureInfo, DateTimeFormat, null, SuppressId);
                if (propCi.SerializableClassAttribute != null) {
                    SerializeClass(writer, propCi, false, propertyName);
                } else {
                    SerializePropertyExternal(writer, propCi.Object, propertyInfo.IsXmlAttribute, propertyName, propCi.ClassType);
                }
            }
        }

        /// <summary>
        /// Serializes a value that is not known to the serializer but to an IExternalObjectSerializer
        /// </summary>
        /// <param name="writer">Xml writer that is used for the serialization.</param>
        /// <param name="value">The value type that is serialized.</param>
        /// <param name="serializeAsAttribute">Is true if the XmlWriter is in WriteAttribute mode - creating an element is not possible.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private void SerializePropertyExternal(XmlWriter writer, object value, bool serializeAsAttribute, string propertyName, Type propertyType) {
            var serializer = ExternalSerializers.FirstOrDefault(d => d.SupportsType(propertyType));
            if (serializer != null) {
                if (serializeAsAttribute) {
                    writer.WriteStartAttribute(propertyName);
                } else {
                    writer.WriteStartElement(propertyName);
                }

                if (!serializer.Serialize(writer, value, serializeAsAttribute, CultureInfo, DateTimeFormat, SuppressId)) {
                    throw new ExtendedXmlSerializationException($"Cannot serialize type {propertyType.FullName} with value {value}.");
                }
                if (serializeAsAttribute) {
                    writer.WriteEndAttribute();
                } else {
                    writer.WriteEndElement();
                }
            } else {
                throw new ExtendedXmlSerializationException($"There is not serializer that supports the type {propertyType.FullName} to serialize the value {value}");
            }
        }

        /// <summary>
        /// Returns the order in which the properties are sorted, if properties are ordered with the XmlSerializationPriorityAttribute
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo for the property.</param>
        /// <returns>The order number of the item or int.MaxValue to deprioritize.</returns>
        private object GetPropertySerializationPriority([NotNull] SerializablePropertyInfo propertyInfo) {
            var spa = propertyInfo.PropertyType.GetCustomAttribute<XmlSerializationPriorityAttribute>();
            if (spa != null) {
                return spa.Priority;
            } else {
                return int.MaxValue;
            }
        }

    }
}