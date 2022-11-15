using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Delegates;
using LazyApiPack.XmlTools.Exceptions;
using LazyApiPack.XmlTools.Helpers;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace LazyApiPack.XmlTools {
    public partial class ExtendedXmlSerializer<TClass> where TClass : class {
        /// <summary>
        /// Is raised if the file version does not match the given app version.
        /// </summary>
        public event FileVersionMismatchDelegate<TClass>? MigrateXmlDocument;

        /// <summary>
        /// Is raised, if a property in the xml is not found in the target class.
        /// </summary>
        public event UnresolvedPropertyDelegate<TClass>? PropertyNotFound;

        /// <summary>
        /// Deserializes an xml file to an object.
        /// </summary>
        /// <param name="file">Full file name of the xml.</param>
        /// <param name="checkAppCompatibility">If true, the serializer checks the application compatibility (migration).</param>
        /// <returns>The object that has been deserialized from the given xml.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public TClass Deserialize(string file, bool checkAppCompatibility = false) {
            if (!File.Exists(file)) throw new FileNotFoundException($"The specified file ({file}) does not exist");
            using (var fs = File.OpenRead(file)) {
                return Deserialize(fs, checkAppCompatibility);
            }
        }
        /// <summary>
        /// Deserializes a class from an xml stream.
        /// </summary>
        /// <param name="checkAppCompatibility">If true, the serializer checks the application compatibility for migration.</param>
        /// <returns>The object that has been deserialized from the given xml.</returns>
        public TClass Deserialize([NotNull] Stream sourceStream, bool checkAppCompatibility = false) {
            if (sourceStream == null) throw new ArgumentNullException(nameof(sourceStream));
            var pos = sourceStream.Position;

            try {

                var nsmgr = new XmlNamespaceManager(new NameTable());
                nsmgr.AddNamespace("lzyxmlx", "http://www.jodiewatson.net/xml/lzyxmlx/1.0");
                var context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);
                var xset = new XmlReaderSettings() {
                    ConformanceLevel = ConformanceLevel.Fragment
                };

                var document = XDocument.Load(XmlReader.Create(sourceStream, xset, context), LoadOptions.None);
                var root = document.Element(XName.Get(RootElementName)) ?? throw new ExtendedXmlSerializationException("Root element in xml not found.");
                var headerElement = root.Nodes().OfType<XElement>().First(e => e.Name == XName.Get("Header"));
                var header = GetHeader(headerElement);

                if (checkAppCompatibility) {
                    if (header == null) {
                        throw new ExtendedXmlSerializationException($"The header section in the xml was not found but the {nameof(checkAppCompatibility)} parameter was set to true.");
                    }

                    bool requiredMigration = VersionMismatch(header) != 0;
                    bool appNameMismatch = string.Compare(header.AppName, AppName) != 0;

                    // If version does not match, the update is required and the MigrateXmlDocument Handler must not be null
                    // If only the app name mismatches, the upgrade handler is invoked if it is there, otherwise the name mismatch
                    // is not treated as a problem.
                    if ((requiredMigration && MigrateXmlDocument == null) ||
                        ((appNameMismatch || requiredMigration) &&
                        MigrateXmlDocument?.Invoke(this, header.AppName, AppName, header.AppVersion, AppVersion, document) != true
                        )) {
                        throw new ExtendedXmlFileException(
@$"File can not be deserialized.
Either the xml version {header.AppVersion} does not match the app version {AppVersion} and the MigrateXmlDocument event is not handled, or the MigrateXmlDocument migration has failed.");

                    }
                }

                var rootType = GetType().GetGenericArguments()[0];
                var att = rootType.GetCustomAttribute<XmlClassAttribute>();
                var className = att?.CustomName ?? rootType.Name;
                var contents = root.Nodes().OfType<XElement>().First(e => e.Name == XName.Get(className));
                return (TClass)DeserializeClass(contents, rootType);

            }
            finally {
                ClearSerializationCache();
                if (sourceStream.CanSeek) {
                    sourceStream.Position = pos;
                }
            }
        }

        /// <summary>
        /// Deserializes a class.
        /// </summary>
        /// <param name="objectNode">Current node that represents this class.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private object DeserializeClass(XElement objectNode, Type objectType) {
            string? id = null;
            if (EnableRecursiveSerialization) {
                id = objectNode.Attribute(XName.Get("objId", "http://www.jodiewatson.net/xml/lzyxmlx/1.0"))?.Value
                    ?? throw new NullReferenceException($"Id of {objectType.FullName} can not be null because recursive serialization is enabled.");

                if (objectType.IsAbstract || objectType.IsInterface) {
                    var attType = GetTypeFromAttribute(objectNode) ?? throw new InvalidOperationException($"Class Type was not found in xml for {objectType.FullName}.");
                    objectType = GetCachedType(attType, objectType.Namespace);
                }
            }

            var deserializedClass = _deserializedObjects.FirstOrDefault(c => c.Type == objectType.FullName && c.Id == id);
            if (deserializedClass != null) return deserializedClass.Object;

            var instance = Activator.CreateInstance(objectType) ?? throw new NullReferenceException($"Can not create instance of {objectType.FullName}.");
            if (EnableRecursiveSerialization) {
                _deserializedObjects.Add(new SerializedClassContainer(id, objectType.FullName ?? objectType.Name, instance));

                // Set Key Property
                var keyProperty = objectType.GetProperties().FirstOrDefault(p => p.GetCustomAttribute<XmlClassKeyAttribute>() != null);
                if (keyProperty != null) {
                    if (!SerializationHelper.TryDeserializeValueType(keyProperty.PropertyType, id, out var key, CultureInfo, DateTimeFormat)) {
                        throw new ExtendedXmlSerializationException("Cannot deserialize key property.");
                    } else {
                        keyProperty.SetValue(instance, key);
                    }
                }
            }

            var extended = instance as IExtendedXmlSerializable;
            extended?.OnDeserializing();
            try {
                var ci = new SerializableClassInfo(instance, CultureInfo, DateTimeFormat, id, EnableRecursiveSerialization);
                foreach (var xatt in objectNode.Attributes().Where(a => string.IsNullOrWhiteSpace(a.Name.Namespace.NamespaceName))) {
                    var simpleProperty = ci.Properties.FirstOrDefault(p => p.PropertyName == xatt.Name);
                    if (simpleProperty != null) {
                        if (SerializationHelper.TryDeserializeValueType(simpleProperty.PropertyInfo.PropertyType, xatt.Value, out object value, CultureInfo, DateTimeFormat)) {
                            simpleProperty.PropertyInfo.SetValue(instance, value);
                        } else if (TryDeserializePropertyExternal(xatt.Value, simpleProperty.PropertyType, out var deserialized)) {
                            simpleProperty.PropertyInfo.SetValue(instance, deserialized);
                        } else {
                            throw new ExtendedXmlSerializationException($"Property {xatt.Value} can not be deserialized, because it is not a simple type.");
                        }
                    }
                }
                foreach (var xelement in objectNode.Elements()) {
                    var property = ci.Properties.FirstOrDefault(p => p.PropertyName == xelement.Name);
                    if (property != null) {
                        var createdOnConstruction = property.PropertyInfo.GetValue(instance);
                        var obj = DeserializeProperty(xelement, property.PropertyInfo.PropertyType, createdOnConstruction);
                        property.PropertyInfo.SetValue(instance, obj);
                    } else {
                        PropertyNotFound?.Invoke(this, objectNode, instance);
                    }

                }
            } catch (Exception ex) {
                extended?.OnDeserialized(false);
                throw new ExtendedXmlSerializationException("Deserialization failed. See inner exception for more details.", ex);
            }
            extended?.OnDeserialized(true);
            //} else {
            //    if (Debugger.IsAttached) {
            //       // Debugger.Break(); // This class should have been searialized earlier.
            //    }
            //}
            return instance;
        }

        private string? GetTypeFromAttribute([NotNull] XElement objectNode) {
            return objectNode.Attributes().FirstOrDefault(a => a.Name == XName.Get("clsType", "http://www.jodiewatson.net/xml/lzyxmlx/1.0"))?.Value;
        }

        /// <summary>
        /// Deserializes a property
        /// </summary>
        /// <param name="objectNode"></param>
        /// <param name="objectType"></param>
        /// <param name="createdOnConstruction"></param>
        /// <returns></returns>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private object? DeserializeProperty(XElement objectNode, Type objectType, object? createdOnConstruction) {
            if (objectType.IsArray) {
                return DeserializeArray(objectNode, objectType, createdOnConstruction);
            } else if (objectType.IsGenericType) {
                return DeserializeGeneric(objectNode, objectType, createdOnConstruction);
            } else if (objectType.IsEnum) {
                return DeserializeEnum(objectNode, objectType);
            } else if (IsValueType(objectType)) {
                return DeserializeValueType(objectNode, objectType);
            } else {
                if (TryDeserializeComplexClass(objectNode, objectType, out object? complex)) {
                    return complex;
                } else {
                    throw new ExtendedXmlSerializationException("The type {type.FullName} could not be deserialized and there were no events overloaded to deserialize it.");
                }

            }
        }

        /// <summary>
        /// Deserializes a base64 string to byte[]
        /// </summary>
        /// <param name="objectNode">The xml node representing the object.</param>
        /// <returns>The byte array or null.</returns>
        private byte[]? DeserializeBinary(XElement objectNode) {
            var b64 = objectNode.Value;
            if (string.IsNullOrEmpty(b64)) {
                return null;
            } else {
                return Convert.FromBase64String(b64);
            }
        }

        /// <summary>
        /// Deserializes an enum
        /// </summary>
        /// <param name="objectNode">The xml node representing the object.</param>
        /// <param name="objectType">The type of the enum.</param>
        /// <returns>The deserialized enum or the default value.</returns>
        private object DeserializeEnum(XElement objectNode, Type objectType) {
            if (string.IsNullOrWhiteSpace(objectNode.Value))
                return Activator.CreateInstance(objectType) ?? throw new NullReferenceException($"Can not create instance of {objectType.FullName}.");
            return Enum.Parse(objectType, objectNode.Value);

        }

        /// <summary>
        /// Deserializes an array.
        /// </summary>
        /// <param name="objectNode">The xml node representing the object.</param>
        /// <param name="objectType">The type of the array</param>
        /// <param name="createdOnConstruction">If true, the array was created by the target class and the function does not need to create its own instance of the array (only fills it with data).</param>
        /// <returns>The deserialized array.</returns>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private object? DeserializeArray(XElement objectNode, Type objectType, object? createdOnConstruction) {
            var rank = objectNode.Attribute(XName.Get("rankDescriptor", "http://www.jodiewatson.net/xml/lzyxmlx/1.0"))?.Value;

            if (rank == null) {
                if (objectNode.Attribute(XName.Get("format", "http://www.jodiewatson.net/xml/lzyxmlx/1.0"))?.Value == "Base64") {
                    return DeserializeBinary(objectNode);
                } else {
                    throw new ExtendedXmlSerializationException($"Cannot deserialize array {objectNode.Name} of type {objectType.FullName} because no rank descriptor was found and no format descriptor to deserialize");
                }
            }
            Array array;
            if (rank.Contains(";")) {
                // Multidimensional
                var lenghtsStr = rank.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var lengths = lenghtsStr.Select(s => int.Parse(s)).ToArray();
                array = Array.CreateInstance(objectType.GetElementType() ?? throw new NullReferenceException($"Element type of array {objectType.FullName} is null."), lengths);
            } else {
                // Single dimensional
                var length = int.Parse(rank);
                array = Array.CreateInstance(objectType.GetElementType() ?? throw new NullReferenceException($"Element type of array {objectType.FullName} is null."), length);
            }

            var items = objectNode.Elements();
            var descriptor = new SerializableArray(array);
            while (descriptor.MoveNext()) {
                var node = items.FirstOrDefault(i => i.Attribute(XName.Get("index", "http://www.jodiewatson.net/xml/lzyxmlx/1.0"))?.Value == descriptor.CurrentIndexString);


                if (node != null) {

                    descriptor.Current = DeserializeProperty(node, descriptor.ItemType, createdOnConstruction);

                    // Nullable Properties are not serialized

                }
            }

            return array;
        }

        /// <summary>
        /// Deserializes a generic.
        /// </summary>
        /// <param name="objectNode">The xml node representing the object.</param>
        /// <param name="objectType">The type of the generic.</param>
        /// <param name="createdOnConstruction">If true, the generic was created by the target class and the function does not need to create its own instance of the generic (only fills it with data).</param>
        /// <returns>The deserialized generic.</returns>
        /// <exception cref="ExtendedXmlSerializationException">If the generic type is not supported.</exception>
        /// <remarks>Supported Generics: Nullable, IList, Dictionary</remarks>
        /// <remarks >The value, names etc. can be different from propertyInfo.</remarks>
        private object? DeserializeGeneric(XElement objectNode, Type objectType, object? createdOnConstruction) {
            var typedef = objectType.GetGenericTypeDefinition();
            var typeargs = objectType.GetGenericArguments();

            if (typedef == typeof(Nullable<>)) {
                return DeserializeProperty(objectNode, typeargs[0], createdOnConstruction);
                //} else if (typedef == typeof(List<>) || typedef == typeof(Collection<>) || typedef == typeof(ReadOnlyCollection<>) || typedef == typeof(ObservableCollection<>)) {
            } else if (typedef == typeof(Dictionary<,>)) {
                return DeserializeDictionary(objectNode, objectType, typeargs[0], typeargs[1], createdOnConstruction);
            } else if (typeof(IList).IsAssignableFrom(typedef) ||
                typeof(IList<>).IsAssignableFrom(typedef) ||
                typeof(ICollection).IsAssignableFrom(typedef) ||
                typeof(ICollection<>).IsAssignableFrom(typedef)) {
                return DeserializeList(objectNode, objectType, typeargs[0], createdOnConstruction);
            } else {
                if (TryDeserializePropertyExternal(objectNode.Value, objectType, out var deserialized)) {
                    return deserialized;
                }
                throw new ExtendedXmlSerializationException($"The generic type {typedef.FullName} is not supported.");
            }
        }

        /// <summary>
        /// Deserializes a dictionary.
        /// </summary>
        /// <param name="objectNode">The xml node representing the object.</param>
        /// <param name="dictionaryType">The type of the dictionary.</param>
        /// <param name="keyType">The type of the key.</param>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="createdOnConstruction">If true, the array was created by the target class and the function does not need to create its own instance of the array (only fills it with data).</param>
        /// <returns>The deserialized dictionary.</returns>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private object DeserializeDictionary(XElement objectNode, Type dictionaryType, Type keyType, Type valueType, object? createdOnConstruction) {
            var dictionary = Activator.CreateInstance(dictionaryType) as IDictionary ?? throw new ExtendedXmlSerializationException($"Could not create an instance of {dictionaryType.FullName}.");

            foreach (var item in objectNode.Elements()) {
                var keyElement = item.Element(XName.Get("Key")) ?? throw new ExtendedXmlSerializationException("Could not find Key element in xml.");
                var key = DeserializeProperty(keyElement, keyType, createdOnConstruction);
                var valueElement = item.Element(XName.Get("Value")) ?? throw new ExtendedXmlSerializationException("Could not find Value element in xml.");
                var value = DeserializeProperty(valueElement, valueType, createdOnConstruction);
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        /// <summary>
        /// Deserializes a list.
        /// </summary>
        /// <param name="objectNode">The xml node representing the object.</param>
        /// <param name="listType">The type of the list.</param>
        /// <param name="itemType">The type of the items.</param>
        /// <param name="createdOnConstruction">If true, the array was created by the target class and the function does not need to create its own instance of the array (only fills it with data).</param>
        /// <returns>The deserialized list.</returns>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private object DeserializeList(XElement objectNode, Type listType, Type itemType, object? createdOnConstruction) {
            if (listType.GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>)) {
                var tempListType = typeof(List<>).MakeGenericType(itemType);
                var tempList = Activator.CreateInstance(tempListType) as IList ?? throw new ExtendedXmlSerializationException($"Could not create an instance of {tempListType.FullName}.");
                foreach (var element in objectNode.Elements().ToList()) {
                    var attType = GetTypeFromAttribute(element);
                    var currentItemType = itemType;
                    if (attType != null) {
                        currentItemType = GetCachedType(attType, itemType.Namespace);
                    }
                    tempList.Add(DeserializeProperty(element, currentItemType, createdOnConstruction));
                }
                return Activator.CreateInstance(listType, tempList) ?? throw new ExtendedXmlSerializationException($"Could not create an instance of {listType.FullName}."); ;
            } else {
                var list = createdOnConstruction as IList;
                if (list == null) {
                    if (listType.IsGenericType || listType.IsAbstract || listType.IsInterface) {
                        var strType = GetTypeFromAttribute(objectNode);
                        var type = GetCachedType(strType, null);
                        list  = Activator.CreateInstance(type) as IList;
                    } else {

                        list = Activator.CreateInstance(listType) as IList ?? throw new ExtendedXmlSerializationException($"Could not create an instance of {listType.FullName}.");
                    }
                }
                // TODO: CreatedOnConstruction is not of type IList

                foreach (var element in objectNode.Elements().ToList()) {
                    var attType = element.Attributes().FirstOrDefault(a => a.Name == XName.Get("clsType", "http://www.jodiewatson.net/xml/lzyxmlx/1.0"));
                    var currentItemType = itemType;
                    if (attType != null) {
                        currentItemType = GetCachedType(attType.Value, itemType.Namespace);
                    }
                    var value = DeserializeProperty(element, currentItemType, createdOnConstruction);
                    list.Add(value);
                }
                return list;
            }
        }

        /// <summary>
        /// Deserializes a value type.
        /// </summary>
        /// <param name="objectNode">The xml node representing the object.</param>
        /// <param name="objectType">The type of the value type.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="ExtendedXmlSerializationException"></exception>
        private object? DeserializeValueType(XElement objectNode, Type objectType) {
            if (SerializationHelper.TryDeserializeValueType(objectType, objectNode.Value, out object? value, CultureInfo, DateTimeFormat)) {
                return value;
            }

            // Enums with datatype "int" etc.
            if (TryDeserializeComplexClass(objectNode, objectType, out value)) {
                return value;
            } else {
                throw new ExtendedXmlSerializationException("Cannot deserialize value.");
            }

        }

        /// <summary>
        /// Deserializes a class.
        /// </summary>
        /// <param name="objectNode">The xml node representing the object.</param>
        /// <param name="objectType">The type of the class.</param>
        /// <param name="deserialized">The object that has been deserialized.</param>
        /// <returns>True, if the class could be deserialized. Otherwise false.</returns>
        private bool TryDeserializeComplexClass(XElement objectNode, Type objectType, out object? deserialized) {
            var attType = GetTypeFromAttribute(objectNode);

            if (attType != null) {
                objectType = GetCachedType(attType, objectType.Namespace);
            }
            var att = objectType.GetCustomAttribute<XmlClassAttribute>();
            if (att != null) {
                deserialized = DeserializeClass(objectNode, objectType);
                return true;
            } else {
                return TryDeserializePropertyExternal(objectNode.Value, objectType, out deserialized);

            }

        }

        /// <summary>
        /// Deserializes a value that is not known to the serializer but to an IExternalObjectSerializer
        /// </summary>
        /// <param name="value">The xml node representing the object.</param>
        /// <param name="objectType">The type of the class.</param>
        /// <param name="deserialized">The object that has been deserialized.</param>
        /// <returns>True, if the class could be deserialized or false, if the deserialization failed or no suitable extension was found.</returns>
        private bool TryDeserializePropertyExternal(string value, Type objectType, out object? deserialized) {
            var deserializer = ExternalSerializers.FirstOrDefault(d => d.SupportsType(objectType));
            if (deserializer != null) {
                deserialized = deserializer.Deserialize(value, objectType, CultureInfo, DateTimeFormat, EnableRecursiveSerialization);
                return true;
            } else {
                deserialized = null;
                return false;
            }
        }








    }
}