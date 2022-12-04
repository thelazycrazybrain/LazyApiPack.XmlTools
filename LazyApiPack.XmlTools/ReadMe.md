# About this project
This Serializer is designed to make xml serialization easier with excessive type support.
It can serialize properties with abstract or interface types, generics etc.
The serializer also supports recursive object serialization. If an object has already been serialized, the element value in the xml is only a reference to the object serialized in the document above.

This project was originally designed as a bodge, but grew over time into something that I can share online.
It is not fully tested and I don't give any guarantee that it works in any case.

# Contribute
If you want to fix bugs or extend it, feel free to contribute.

# Serialization and Deserialization
If you have a class decorated with the XmlClassAttribute, the Serializer can serialize and deserialize the object.
This serializer supports recursive serialization (Can be deactivated with the enableRecursiveSerialization flag). It caches all serialized classes with an Id (Either ObjectHash or a property marked with the SerializableKeyAttribute).
If the serializer comes across an object that has already been serialized, it only writes a reference to this object in the xml document.

Interface and abstract type serialization is also supported. The serializer writes the Type-FullName or Name to the xml.
If you dont want to include the whole namespace in the xml, you can suppress the full namespace. All objects must be in the same namespace and the Serializer must know the namespace in the program where it can find all serializable classes.


## Serialization
```csharp
var model = new SimpleDataModel();

new ExtendedXmlSerializer<SimpleDataModel>()
    .Serialize(model, "[PathToFile]");

```

## Deserialization
```csharp
var model = new ExtendedXmlSerializer<SimpleDataModel>()
    .Deserialize("[PathToFile");
```

# Attributes
## XmlClassAttribute
Marks a class as serializable
A name can be specified to name the element in the xml

## SerializableKeyAttribute
Marks a property or public field (Guid or any other simple type) that is used to identify the serialized object in the xml.
If recursive serialization is enabled and no key property is used, the object is identified by its hash code.

## XmlPropertyAttribute, XmlElementAttribute
Marks a property or public field as serializable. All other properties and fields are not serialized.

## XmlSerializationPriorityAttribute
All properties can be prioritized in the xml. The higher the number, the lower the priority.

The code below:
```csharp
[XmlAttribute("firstAttribute")]
[XmlSerializationPriority(-2)]
public string FirstAttribute { get; set; }


[XmlProperty("secondElement")]
[XmlSerializationPriority(2)]
public string SecondProperty { get; set; }

[XmlProperty("firstElement")]
[XmlSerializationPriority(-2)]
public string FirstProperty { get; set; }



[XmlAttribute("secondAttribute")]
[XmlSerializationPriority(1)]
public string SecondAttribute { get; set; }
```
will be serialized to this xml:

```xml
<serializedObject firstAttribute="value" secondAttribute="value">
    <firstElement>value</firstElement>
    <secondElement>value</secondElement>
</serializedObject>
```

Due to the nature of an xml file, attributes are first serialized. After that the xml properties (xmlelement). 
The priorities are therefore separated for XmlAttributes and XmlElements.

# XmlArrayAttribute, XmlArrayItemAttribute
Use this attribute to serialize a property or field as an xml array (simple properties only!)

# XmlObsoleteAttribute
Use this attribute to make a property obsolete in xml.
If this attribute is present, the property will be deserialized, but not serialized again.
This attribute is used if a property gets replaced by another property but this value is needed for the migration process.

-> See section "Migration"

# Interfaces
## IExtendedXmlSerializable
Classes that are serialized can implement the IExetndedXmlSerializable interface to make it aware of the serialization process (eg. Supress PropertyChanged notifications during deserialization).

- OnDeserializing()
	Is called, when the ExtendedXmlSerializer<>.Deserialize() method is invoked and the deserialization process is about to start.
	Use this method to set flags, that the class is deserializing to suppress INotifyPropertyChanged events for example.

- OnDeserialized(bool success)
	Is called, when the ExtendedXmlSerializer<>.Deserialize() method is invoked and the deserialization process has been completed
	If you set a flag that the class is deserializing, set the flag back in this method because this method is always called even if the deserialization process has failed.
	Success is true, if the deserialization was successful
	Success is false, if the deserialization process was unsuccessful and therefore incomplete.

- OnSerializing()
	Is called, when the ExtendedXmlSerializer<>.Serialize() method is invoked and the serialization process is about to start.
	Use this method to prepare the class (store all data properly) so the serializer has all data to serialize it to xml.
	You can set a "IsSerializing" flag if you want your class to behave differently when using getters or it is used in multithreading situations.
	
- OnSerialized(bool success)
	Is called, when the ExtendedXmlSerializer<>.Serialize() method is invoked and the serialization process has been completed.
	If you set a flag that the class is serializing, set the flag back in this method because this method is always called even if the deserialization process has failed.

# Supported Simple Types
## Simple types
Guid
Verision
string
sbyte
byte
ushort
short
uint
int
ulong
long
float
double
decimal
char
bool
DateTime
System.Drawing.Point
System.Drawing.Size
TimeSpan


# abstract and interface types
All abstract and interface types can be serialized and deserialized as long as they represent a concrete implementation that can be serialized and deserialized.
Abstract types and interfaces can also be used in collections


# Collections
Collections can be deserialized and serialized. If a collection in a class is already instantiated before it is deserialized, the Serializer uses the instance that is already created or it creates a list itself.
Collections can also contain abstract and interface type parameters as long all used types of the list items are implemented as serializable and deserializable types.

## Supported Collection Types
ObservableCollection<>
IList<>
byte[]
Dictionary<,>
Array
Enum

## Arrays
Arrays are serialized and indexed. An array can also be multi-dimensional 
(eg. SerializableType[,,,,,])


# Extending the serializer with custom types
The ExtendedXmlSerializer implements a way to support types that can normally not be serialized and are not under control of the developer (eg. BitmapImage).

To use existing extensions, you can use LazyApiPack.XmlTools.Wpf for example, to add support for types used in PresentationCore or System.Windows.

## Create own Extensions
To extend the XmlSerializer, you can create a class that implements the IExernalObjectSerializer attribute.

- bool SupportsType(Type type)
	Is called from the serializer to check, if the class supports to deserialize / serialize this type

- object? Deserialize(string? value, Type type, IFormatProvider format, string dateTimeFormat, bool enableRecursiveSerialization);
	Is called from the serializer when the type (that is checked against SupportsType) needs to be deserialized.
	Value is the string (or xml) representation of the object that needs to be deserialized.

- bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string dateTimeFormat, bool enableRecursiveSerialization);
	Is called from the serializer when the type (that is checked against SupportsType) needs to be serialized.
	The writer is either in the WriteStartElement() or WriteAttributeStart() state at this point (Depending whether serializeAsAttribute is True or not)
	Use writer.WriteValue() to write the serialized content to the xml.
	You can create subnodes if you need to, but only if serializeAsAttribute is set to False.
	Do not close the current element, otherwise the serializer will fail because it would run into generating an invalid xml document.
	Return true, if the object (value) was serialized successfully. Returning false aborts the serialization process.

### Example
```csharp
public class WindowsThicknessSerializer : IExternalObjectSerializer {
    public bool Serialize(XmlWriter writer, object? value, 
                          bool serializeAsAttribute, IFormatProvider format, 
                          string? dateTimeFormat, bool enableRecursiveSerialization, 
                          out string? dataFormat) {
        dataFormat = null;
        if (value == null) {
            writer.WriteValue(null);
        } else {
            var thk = (Thickness)value;
            writer.WriteValue($"{thk.Left},{thk.Top},{thk.Right},{thk.Bottom}");
        }
        return true;
    }

    public object? public object? Deserialize(XElement node, Type type, IFormatProvider format, 
                                              string? dateTimeFormat, bool enableRecursiveSerialization, 
                                              string? dataFormat) {
        if (string.IsNullOrWhiteSpace(node.Value)) return default(Thickness);

        var s = node.Value.Split(",");
        if (s.Length == 4 && double.TryParse(s[0], out var l) && double.TryParse(s[0], out var t)&& double.TryParse(s[0], out var r)&& double.TryParse(s[0], out var b)) {
            return new Thickness(l, t, r, b);
        } else {
            throw new InvalidCastException($"Cannot convert {node.Value} to {typeof(Thickness).FullName}.");
        }
    }

    public bool SupportsType(Type type) {
        return type == typeof(Thickness);
    }

}
```

Usage:
```csharp
var serializer = new ExtendedXmlSerializer<SimpleDataModel>();
serializer.ExternalSerializers.Add(new WindowsThicknessSerializer());

```

## Creating an ExtendedXmlSerializer that supports derived, abstract and interface types
If you want to support types, that are interfaces, abstract types or derived types, you can change your extension as follows:

```csharp
// The SupportsType function should not check a direct match, but should check, if the type is assignable
public bool SupportsType(Type type, string? dataFormat) {
    return type.IsAssignableTo(typeof(StrokeCollection)); // .NET 5 and above
    return typeof(StrokeCollection).IsAssignableTo(type); // Prior to .NET 5
}

// If you support derived types, take those types into consideration here.
public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, 
                       IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, 
                       Action<string> setDataFormat) {
    if (value == null) return true;
    // NOTE: value is compatible with StrokeCollection (See SupportsType) so the check here passes, 
    // however, value might be a more specific type at this point
    // since we only check for the type compatibility in SupportsType rather than checking for an exact type.
    if(value is StrokeCollection collection) {
        // Serialize your class as default
    }

    if (value is SpecialStrokeCollection specialCollection) {
        // If you want to handle special cases, just cast it to known types you want to support.
        // Serialize your class as SpecialStrokeCollection
    }
}

// IMPORTANT: Deserialization needs special attention!
public object? Deserialize(XElement node, Type type, IFormatProvider format, 
                           string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
    // IMPORTANT: You can not create StrokeCollection with new because it might be a derived type.
    var collection = Activator.CreateInstance(type) as StrokeCollection; // Create a specialized class with 
                                                                         // the Activator and cast it to a class that is compatible
                                                                         // and you can handle.
    // you can check the created type for special derived types you want to support
    
    // NOTE: Deserialize your class here and treat it literally as a normal StrokeCollection
    collection.Strokes.Add(...);

    // If you support derived types that you know, you can cast the object to 
    // a special class and set additional properties.
    if (collection is SpecialStrokeCollection specialCollection) {
        collection.StrokeDecorations.Add(...);
    }
    if(collection is IncredibleSpecialStrokeCollection incredibleSpecialCollection) {
        collection.StrokeEffects.Add(...);
    }
}
```
## Overriding default behavior of the ExtendedXmlSerializer
If you don't want to serialize certain data types the way the ExtendedXmlSerializer serializes it (eg. byte[] is stored as base64 text in the xml),
you can create an IExternalXmlSerializer, that supports a known type (eg. byte[]) and a custom dataFormat.

### Example
```csharp
// Adds support for the known type byte[]
// dataFormat is NULL when the object is being serialized.
// dataFormat is whatever you set it during your custom serialization
// Be careful! the dataFormat is not always how you've set it if for example the ExtendedXmlSerializer or a different ExternalXmlSerializer has serialized the object.
public bool SupportsType(Type type, string? dataFormat) {
    return type.IsAssignableTo(typeof(byte[])) && (dataFormat == null || dataFormat == "zipResource");
}

// Serializes the object
public bool Serialize(XmlWriter writer, object? value, 
                        bool serializeAsAttribute, IFormatProvider format, 
                        string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat) {
    string id;
    if (value is byte[] ba) { // if the value is the supported type, serialize it
        id = _resourceManager.AddResource(ba); // Serialization logic
        setDataFormat(ZIP_RESOURCE_DATA_FORMAT_NAME); // make sure, you set the dataFormat BEFORE you write anything to the writer!
    } else {
        return false; // Serialization is not supported or failed for whatever reason (The serializer will abort the serialization).
    }

    writer.WriteValue(id); // Write the actual value to the xml (see topic before).
    return true; // Serialization was successful.
}

// Deserializes the object
public object? Deserialize(XElement objectNode, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat) {
    if (node.Value == null) return null;
    if (type == typeof(byte[]) && dataFormat == "zipResource") { // datatype is supported, also the dataFormat as specified in the Serialize method
        return _resourceManager.GetResource(node.Value); // the actual deserialization.
    } else {
        // If type is not deserializable (this is only interesting, if the serializer supports multiple types and dataFormats and it needs a way to fail properly in case 
        // the SupportedType method and Serialize / Deserialize methods are not properly implemented).
        throw new NotSupportedException($"Type {type.FullName} with data format {dataFormat} is not supported by {nameof(ZipResourceSerializer)}.{nameof(Deserialize)}.");
    }
}

```


## Create multiple extensions
To create multiple extensions, I recommend using a class extension for the ExtendedXmlSerializer.

Example:
```csharp
public static class WpfExtendedXmlSerializer {
    /// <summary>
    /// Adds support for Wpf specific types.
    /// </summary>
    /// <typeparam name="TClass">Class Type of the serializer target object.</typeparam>
    /// <param name="serializer">The serializer that uses the extensions.</param>
    public static ExtendedXmlSerializer<TClass> UseWpfTypeSupport
        <TClass>(this ExtendedXmlSerializer<TClass> serializer) where TClass : class {
        serializer.ExternalSerializers.Add(new WindowsPointSerializer());
        serializer.ExternalSerializers.Add(new WindowsThicknessSerializer());
        return serializer;
    }
}
```

Usage:

```csharp
new ExtendedXmlSerializer<SimpleDataModel>()
    .UseWpfTypeSupport()
    .Serialize(model, "[PathToFile]");
	
```

# Xml Migration
The xml contains header information. If the program version does not match the xml version, the events UpgradeFileVersion or DowngradeFileVersion will be called.
These events give you the file version and the version the serializer was given to check the file version (program version).
The XDocument contains the loaded xml document. You can modify this document how you like in order for the serializer to deserialize it properly.
All changes to XDocument are not stored to the file, but the new file with the new format and version will be stored to the file, when the Serializer is called with the Serialize() function.

Example:
```csharp
[XmlClass("myClass")]
public class MyClass {
    [XmlProperty("value")] // The new property can not have the same name as the new property!
    [Obsolete] // Note: the System.Obsolete does not change the xml serializer behavior!
    public string OldValue { get; set; }

    [XmlProperty("intValue")] // The new property needs a new name!
    public int Value { get; set; }
}
```
The migration process could look like that:
```csharp
 private bool Serializer_MigrateXmlDocument(ExtendedXmlSerializer<MyClass> sender, 
                                            string? xmlAppName, 
                                            string? currentAppName, 
                                            Version xmlAppVersion, 
                                            Version currentAppVersion, 
                                            XDocument document) {
    if (xmlAppVersion >= new Version(1,2,3,4)) {
        var cls = document.Element("ExtendedSerializedObjectFile")
                         ?.Element("myClass");
        if (cls != null) {
            var value = cls.Element("value").Value;
            if (int.TryParse(value, NumberStyles.Number, 
                sender.CultureInfo, // optional
                out var intValue)) {
                    cls.Add(new XElement("intValue", intValue); // add the new value to the xml
                        return true;
            } else {
                return false;
            }
         } else {
             // Not the correct xml.
             return false;
         }
    }
}
```

## Handling obsolete properties with the XmlObsoleteAttribute
If a property gets obsolete or is replaced by another implementation, you can mark the old property with the
`XmlObsoleteAttribute`.
If this attribute is present, this property will be deserialized, but not serialized again.

Example of "self-migration":
```csharp
[XmlClass("myClass")]
public class MyClass {
    [XmlProperty("value")] // The new property can not have the same name as the new property!
    [XmlObsolete("Use Value instead.")] // Makes this property Deserialize-Only
    [Obsolete] // Note: the System.Obsolete does not change the xml serializer behavior!
    public string OldValue {  set => Value = int.Parse(value); } // Note: This property is write-only!

    [XmlProperty("intValue")] // The new property needs a new name!
    public int Value { get; set; } // this property can be deserialized or set by OldValue (depending if you use an old xml or a new one)
}
```


# Missing Properties in the class
If an xml contains an element that is not represented by the class given to the serializer, the PropertyNotFound event is raised. The deserialization will not fail. You can use this event handler, to handle the property manually with
the unresolvedPropertyNode and the target class.

# The lzyxmlx namespace
Attributes that are used to store meta information the serializer needs to deserialize the xml are stored
in the namespace lzyxmlx. So you can use an attribute "objId" and "clsType" and you do not interfere with the
Serializers infrastructure
- lzyxmlx:objId 	Used as the object identifier (Recursive serialization support)
- lzyxmlx:clsType 	Supports serialization and deserialization of abstract and interface types
