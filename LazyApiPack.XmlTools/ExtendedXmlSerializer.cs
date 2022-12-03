using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Exceptions;
using LazyApiPack.XmlTools.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools {
    /// <summary>
    /// Serializes and deserializes classes to xml
    /// </summary>
    /// <typeparam name="TClass">Class that this serializer can deserialize</typeparam>
    public partial class ExtendedXmlSerializer<TClass> where TClass : class {
        /// <summary>
        /// Contains the current instance of the XmlWriter while serializing.
        /// </summary>
        XmlWriter _writer;
        /// <summary>
        /// Serializes a class to an xml file.
        /// </summary>
        /// <param name="sourceClass">The class to serialize.</param>
        /// <param name="fileName">The fully qualified file name for the xml.</param>
        public void Serialize([NotNull] TClass sourceClass, string fileName) {
            using var serialized = Serialize(sourceClass);
            using var fs = File.OpenWrite(fileName);
            fs.SetLength(serialized.Length);
            serialized.CopyTo(fs);
        }

        /// <summary>
        /// Serializes a list of classes to an xml file
        /// </summary>
        /// <param name="sourceClasses">List of classes to serialize.</param>
        /// <param name="fileName">The fully qualified file name for the xml.</param>
        public void SerializeAll([NotNull] IEnumerable<TClass> sourceClasses, string fileName) {
            using var serialized = SerializeAll(sourceClasses);
            using var fs = File.OpenWrite(fileName);
            fs.SetLength(serialized.Length);
            serialized.CopyTo(fs);
        }


        /// <summary>
        /// Serializes a class to an xml stream.
        /// </summary>
        /// <param name="sourceClass">The class to serialize.</param>
        /// <returns>The stream containing the serialized class.</returns>
        /// <exception cref="NullReferenceException">If sourceClass is null.</exception>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        public Stream Serialize([NotNull] TClass sourceClass) {
            try {
                if (sourceClass == null) throw new NullReferenceException("Can not serialize a class which is null.");
                var targetStream = new MemoryStream();
                using (var ms = new MemoryStream()) {
                    using (_writer = CreateXmlWriter(ms)) {
                        WriteRoot();
                        var clsInfo = new SerializableClassInfo(
                                                sourceClass, CultureInfo,
                                                DateTimeFormat, null,
                                                EnableRecursiveSerialization);
                        if (clsInfo.SerializableClassAttribute == null) {
                            throw new ExtendedXmlSerializationException(
                                $"Cannot serialize the class {sourceClass.GetType().FullName}. It does not specify the SerializableClass Attribute.");
                        }
                        SerializeClass(clsInfo, true);

                        _writer.WriteEndElement();
                        _writer.WriteEndDocument();
                        _writer.Flush();
                        ms.Position = 0;
                        targetStream.SetLength(ms.Length);
                        ms.CopyTo(targetStream);
                    }
                }
                targetStream.Position = 0;
                return targetStream;
            } finally {
                ClearSerializationCache();
            }
        }

        /// <summary>
        /// Serializes a list of classes to an xml stream.
        /// </summary>
        /// <param name="sourceClasses">The classes to serialize.</param>
        /// <returns>The stream containing the serialized classes.</returns>
        /// <exception cref="NullReferenceException">If sourceClasses is null.</exception>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        public Stream SerializeAll([NotNull] IEnumerable<TClass> sourceClasses) {
            try {
                if (sourceClasses == null) throw new NullReferenceException("Can not serialize a list of classes which is null.");
                var targetStream = new MemoryStream();
                using (var ms = new MemoryStream()) {
                    using (_writer = CreateXmlWriter(ms)) {
                        WriteRoot();
                        foreach (var sourceClass in sourceClasses) {
                            var clsInfo = new SerializableClassInfo(
                                                    sourceClass, CultureInfo,
                                                    DateTimeFormat, null,
                                                    EnableRecursiveSerialization);
                            if (clsInfo.SerializableClassAttribute == null) {
                                throw new ExtendedXmlSerializationException(
                                    $"Cannot serialize the class {sourceClass.GetType().FullName}. It does not specify the SerializableClass Attribute.");
                            }
                            SerializeClass(clsInfo, false);

                        }
                        _writer.WriteEndElement();
                        _writer.WriteEndDocument();
                        _writer.Flush();
                        ms.Position = 0;
                        targetStream.SetLength(ms.Length);
                        ms.CopyTo(targetStream);
                    }
                }
                targetStream.Position = 0;
                return targetStream;
            } finally {
                ClearSerializationCache();
            }
        }

        /// <summary>
        /// Serializes an object
        /// </summary>
        /// <param name="classInfo">The class info representing the object.</param>
        /// <param name="includeObjectNamespace">Speficies, if the clsClass attribute is written to the xml.</param>
        /// <param name="propertyName">The name of the Xml Element. Null if the class is the root class of the xml.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private void SerializeClass(SerializableClassInfo classInfo, bool includeObjectNamespace,
                                    string? propertyName = null, Action? writeAttributeToCurrentElement = null) {
            if (classInfo.Object == null) return;
            _writer.WriteStartElement(string.IsNullOrWhiteSpace(propertyName) ? classInfo.ClassName : propertyName);
            if (EnableRecursiveSerialization) {
                _writer.WriteAttributeString("objId", LZYNS, classInfo.Id);
            }
            if (includeObjectNamespace) {
                SetTypeAttribute(classInfo.ClassType, writeAttributeToCurrentElement);

            }

            writeAttributeToCurrentElement?.Invoke();

            if (EnableRecursiveSerialization) {
                if (_serializedObjects.Any(c => c.Id == classInfo.Id && c.ClassType.FullName == classInfo.ClassType.FullName)) {
                    // This class was already serialized
                    _writer.WriteEndElement();
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
                    SerializeProperty(propertyInfo, propertyInfo.PropertyValue, propertyInfo.PropertyName, propertyInfo.PropertyTypeName, propertyInfo.PropertyType, propertyInfo.PropertyInfo.PropertyType.IsInterface, null);

                }
                _writer.WriteEndElement();
            } catch (Exception ex) {
                extendedClass?.OnSerialized(false);
                throw new ExtendedXmlSerializationException("Serialization failed. See inner exception for more details.", ex);
            }
            extendedClass?.OnSerialized(true);

        }


        /// <summary>
        /// Serializes a property.
        /// </summary>
        /// <param name="propertyInfo">The property that is serialized.</param>
        /// <param name="value">The property value that is serialized.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyTypeName">Full name of the property type.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="includeObjectNamespace">Speficies, if the clsClass attribute is written to the xml.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private void SerializeProperty(SerializablePropertyInfo propertyInfo, object? value,
                                       string propertyName, string propertyTypeName, Type propertyType,
                                       bool includeObjectNamespace, Action? writeAttributeToCurrentElement) {
            if (value == null) return; // Do not serialize null values

            if (SerializePropertyExternal(value, propertyInfo.IsXmlAttribute, propertyName, value.GetType())) {
                return;
            }

            if (propertyType.IsArray) {
                if (propertyType.GetElementType() == typeof(byte) && propertyType.GetArrayRank() == 1) {
                    SerializeBinary((byte[])value, propertyName, writeAttributeToCurrentElement);
                } else {
                    SerializeArrayProperty(value, propertyInfo, propertyName, writeAttributeToCurrentElement);
                }
            } else if (propertyType.IsGenericType) {
                SerializeGenericProperty(value, propertyInfo, propertyName, propertyTypeName, propertyType,
                    () => SetTypeAttribute(propertyType.IsGenericType ? value.GetType() : propertyType, writeAttributeToCurrentElement));
            } else if (IsSimpleType(propertyType)) {
                SerializeSimpleType(value, propertyInfo, propertyName, writeAttributeToCurrentElement);
            } else {
                // Is Class
                var propCi = new SerializableClassInfo(value, CultureInfo, DateTimeFormat, null, EnableRecursiveSerialization);
                if (propCi.SerializableClassAttribute != null) {
                    SerializeClass(propCi, !IsExactType(propCi.ClassType, value) || includeObjectNamespace, propertyName);
                } else {
                    throw new ExtendedXmlSerializationException($"There is not serializer that supports the type {propertyType.FullName} to serialize the value {value}");
                }
            }

        }


        /// <summary>
        /// Serializes the value as base64.
        /// </summary>
        /// <param name="value">Byte array that is serialized.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        private void SerializeBinary(byte[] value, string propertyName, Action? writeAttributeToCurrentElement) {
            _writer.WriteStartElement(propertyName);
            writeAttributeToCurrentElement?.Invoke();
            _writer.WriteAttributeString("format", LZYNS, "Base64");
            _writer.WriteValue(Convert.ToBase64String(value));
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the value as an array.
        /// </summary>
        /// <param name="value">Array that is serialized.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        private void SerializeArrayProperty([NotNull] object value, SerializablePropertyInfo propertyInfo, string propertyName, Action? writeAttributeToCurrentElement) {
            var array = ((Array)value);
            _writer.WriteStartElement(propertyName);
            writeAttributeToCurrentElement?.Invoke();
            var rankDescriptor = "";
            for (int i = 0; i < array.Rank; i++) {
                rankDescriptor += array.GetLength(i) + ";";
            }
            rankDescriptor = rankDescriptor.TrimEnd(';');
            _writer.WriteAttributeString("rankDescriptor", LZYNS, rankDescriptor);
            var arrayIterator = new SerializableArray(array);
            while (arrayIterator.MoveNext()) {
                SerializeProperty(propertyInfo, arrayIterator.Current, propertyInfo.ArrayPropertyItemName,
                    arrayIterator.ItemType.Name, arrayIterator.ItemType, arrayIterator.ItemType.IsInterface ||
                    arrayIterator.ItemType.IsAbstract, () => _writer.WriteAttributeString("index",
                        LZYNS, arrayIterator.CurrentIndexString));
            }
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the value as a generic type.
        /// </summary>
        /// <param name="value">Generic value that is serialized.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyTypeName">Full name of the property type.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        /// <exception cref="ExtendedXmlSerializationException">If the generic type is not supported.</exception>
        /// <remarks>Supported Generics: Nullable, IList, Dictionary</remarks>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private void SerializeGenericProperty([NotNull] object value, SerializablePropertyInfo propertyInfo, string propertyName, string propertyTypeName, Type propertyType, Action? writeAttributeToCurrentElement) {
            var typedef = propertyType.GetGenericTypeDefinition();
            var typeArgs = propertyType.GetGenericArguments();

            if (typedef == typeof(Nullable<>)) {
                // Decapsulate value type
                propertyType = typeArgs[0];
                propertyTypeName = propertyType.Name;
                SerializeProperty(propertyInfo, value, propertyName, propertyTypeName, propertyType, propertyType.IsInterface || propertyType.IsAbstract, writeAttributeToCurrentElement);
            } else if (typedef == typeof(Dictionary<,>)) {
                var att = propertyType.GetCustomAttribute<DictionaryEqualityComparerAttribute>();
                var comparerType = att?.EqualityComparerType;
                SerializeDictionary((IDictionary)value, typeArgs[0], typeArgs[1], propertyInfo, propertyName, comparerType, writeAttributeToCurrentElement);
            } else if (typeof(IList).IsAssignableFrom(typedef) ||
                       typeof(IList<>).IsAssignableFrom(typedef) ||
                       typeof(ICollection).IsAssignableFrom(typedef) ||
                       typeof(ICollection<>).IsAssignableFrom(typedef)) {
                propertyType = typeArgs[0];
                propertyTypeName = propertyType.Name;
                SerializeListProperty((IList)value, propertyInfo, propertyName, propertyTypeName, propertyType, propertyType.IsInterface || propertyType.IsAbstract, writeAttributeToCurrentElement);

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
        /// <param name="value">The dictionary that is serialized.</param>
        /// <param name="keyType">The type of the key.</param>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="equalityComparer">If the DictionaryEqualityComparerAttribute is present, this type will be stored with the dictionary element.</param>
        /// <exception cref="ExtendedXmlSerializationException">If the generic type is not supported.</exception>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private void SerializeDictionary(IDictionary value, Type keyType, Type valueType, SerializablePropertyInfo propertyInfo, string propertyName, Type? equalityComparer, Action? writeAttributeToCurrentElement) {
            _writer.WriteStartElement(propertyName);
            _writer.WriteAttributeString("keyType", LZYNS, keyType.FullName);
            _writer.WriteAttributeString("valueType", LZYNS, valueType.FullName);
            if (equalityComparer != null) {
                _writer.WriteAttributeString("comparerType", LZYNS, equalityComparer.FullName);
            }

            writeAttributeToCurrentElement?.Invoke();
            foreach (DictionaryEntry item in value) {
                _writer.WriteStartElement("Item");
                SerializeProperty(propertyInfo, item.Key, "Key", keyType.FullName ?? keyType.Name, keyType, keyType.IsInterface || keyType.IsAbstract, null);

                SerializeProperty(propertyInfo, item.Value, "Value", valueType.FullName ?? keyType.Name, valueType, valueType.IsInterface || valueType.IsAbstract, null);
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the value as an IList.
        /// </summary>
        /// <param name="value">The List that is serialized.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyTypeName">Full name of the property type.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="includeObjectNamespace">Speficies, if the clsClass attribute is written to the xml.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private void SerializeListProperty(IList value, SerializablePropertyInfo propertyInfo, string propertyName, string propertyTypeName, Type propertyType, bool includeObjectNamespace, Action? writeAttributeToCurrentElement) {
            _writer.WriteStartElement(propertyName);
            writeAttributeToCurrentElement?.Invoke();
            foreach (var item in value) {
                SerializeProperty(propertyInfo, item, propertyInfo.ArrayPropertyItemName, propertyTypeName, propertyType, includeObjectNamespace, null);
            }
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the value as a value type.
        /// </summary>
        /// <param name="value">The value type that is serialized.</param>
        /// <param name="propertyInfo">Property info representing value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="writeAttributeToCurrentElement">Is called after an element is created. The caller can attach custom attributes (eg. Item-Index, Item-Key) to the created element.</param>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private void SerializeSimpleType(object value, SerializablePropertyInfo propertyInfo, string propertyName, Action? writeAttributeToCurrentElement) {
            if (SerializationHelper.TrySerializeSimpleType(value, out string? serializedSimpleType, out string? serializedSimpleTypeDataType, CultureInfo, DateTimeFormat)) {
                if (propertyInfo.IsXmlAttribute) {
                    writeAttributeToCurrentElement?.Invoke();
                    _writer.WriteAttributeString(propertyName, serializedSimpleType);
                } else {
                    _writer.WriteStartElement(propertyName);
                    writeAttributeToCurrentElement?.Invoke();
                    _writer.WriteValue(serializedSimpleType);
                    _writer.WriteEndElement();
                }
            } else {
                var propCi = new SerializableClassInfo(value, CultureInfo, DateTimeFormat, null, EnableRecursiveSerialization);
                if (propCi.SerializableClassAttribute != null) {
                    SerializeClass(propCi, false, propertyName);
                } else {
                    throw new ExtendedXmlSerializationException($"Cannot serialize type {value.GetType().FullName} with value {value}.");
                }
            }
        }

        /// <summary>
        /// Serializes a value that is not known to the serializer but to an IExternalObjectSerializer
        /// </summary>
        /// <param name="value">The value type that is serialized.</param>
        /// <param name="serializeAsAttribute">Is true if the XmlWriter is in WriteAttribute mode - creating an element is not possible.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private bool SerializePropertyExternal(object value, bool serializeAsAttribute, string propertyName, Type propertyType) {
            var externalSerializer = ExternalSerializers.FirstOrDefault(d => d.SupportsType(propertyType, null));
            if (externalSerializer == null) {
                return false;
            }

            if (serializeAsAttribute) {
                _writer.WriteStartAttribute(propertyName);
            } else {
                _writer.WriteStartElement(propertyName);
            }

            if (!externalSerializer.Serialize(_writer, value, serializeAsAttribute, CultureInfo, DateTimeFormat,
                EnableRecursiveSerialization, 
                    (dataFormat) => {
                        if (serializeAsAttribute) {
                            throw new NotSupportedException($"Byte[] can not be serialized as attribute. Use {nameof(XmlPropertyAttribute)} or {nameof(XmlElementAttribute)} instead.");
                        }
                        _writer.WriteAttributeString("format", ExtendedXmlSerializer.LZYNS, dataFormat);
                    }
            )) {
                throw new ExtendedXmlSerializationException($"Cannot serialize type {propertyType.FullName} with value {value}.");
            }

            if (serializeAsAttribute) {
                _writer.WriteEndAttribute();
            } else {
                _writer.WriteEndElement();
            }
            return true;

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
        /// <summary>
        /// Creates an instance of the XmlWriter with the default configuration.
        /// </summary>
        /// <param name="output">The stream which is used to write the xml to.</param>
        /// <returns>The XmlWriter with the default configuration</returns>
        private XmlWriter CreateXmlWriter(Stream output) {
            return XmlWriter.Create(output,
                new XmlWriterSettings() {
                    Encoding = Encoding.UTF8,
                    Indent = true
                });
        }

        /// <summary>
        /// Creates the root node of the xml with a header
        /// </summary>
        private void WriteRoot() {
            _writer.WriteStartDocument();
            _writer.WriteStartElement(_rootElementName);
            _writer.WriteAttributeString("xmlns", "lzyxmlx", null, LZYNS);
            var header = new ExtendedXmlHeader(AppName, AppVersion);
            WriteHeader(header);
        }

        /// <summary>
        /// Determines if an object is exactly the same type as the parameter type
        /// </summary>
        /// <param name="type">The expected type.</param>
        /// <param name="object">The current object to compare with type.</param>
        /// <returns>True, if the object is not null and the exact type.</returns>
        bool IsExactType(Type type, object? @object) {
            return @object != null && @object.GetType() == type;
        }

        /// <summary>
        /// Writes the clsType attribute to the current element.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="writeAdditionalAttributesToCurrentElement">Additional attributes from parent.</param>
        /// <remarks>Uses the UseFullNamespace property.</remarks>
        private void SetTypeAttribute(Type classType, Action? writeAdditionalAttributesToCurrentElement) {
            _writer.WriteAttributeString("clsType", LZYNS, GetTypeName(classType));
            writeAdditionalAttributesToCurrentElement?.Invoke();

        }
    }
}