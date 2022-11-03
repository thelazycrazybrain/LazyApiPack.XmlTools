# About this project
This Serializer is designed to make xml serialization easier with excessive type support.
It can serialize properties with abstract or interface types, generics etc.
The serializer also supports recursive object serialization. If an object has already been serialized, the element value in the xml is only a reference to the object serialized in the document above.

This project was originally designed as a bodge, but grew over time into something that I can share online.
It is not fully tested and I don't give any guarantee that it works in any case.

# Contribute
If you want to fix bugs or extend it, feel free to contribute.

# TODO:
Use the lzyxmlx namespace for Array Indexer attributes and Dictionary Key and Value attributes.
Test the Migration functionality.
Performace improvements (if needed).

# Serialization and Deserialization
If you have a class decorated with the XmlClassAttribute, the Serializer can serialize and deserialize the object.
This serializer supports recursive serialization (Can be deactivated with the suppressId flag). It caches all serialized classes with an Id (Either ObjectHash or a property marked with the SerializableKeyAttribute).
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

- object? Deserialize(string? value, Type type, IFormatProvider format, string dateTimeFormat, bool suppressId);
	Is called from the serializer when the type (that is checked against SupportsType) needs to be deserialized.
	Value is the string (or xml) representation of the object that needs to be deserialized.

- bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string dateTimeFormat, bool suppressId);
	Is called from the serializer when the type (that is checked against SupportsType) needs to be serialized.
	The writer is either in the WriteStartElement() or WriteAttributeStart() state at this point (Depending whether serializeAsAttribute is True or not)
	Use writer.WriteValue() to write the serialized content to the xml.
	You can create subnodes if you need to, but only if serializeAsAttribute is set to False.
	Do not close the current element, otherwise the serializer will fail because it would run into generating an invalid xml document.
	Return true, if the object (value) was serialized successfully. Returning false aborts the serialization process.

### Example
```csharp
public class WindowsThicknessSerializer : IExternalObjectSerializer {
	public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string dateTimeFormat, bool suppressId) {
		if (value == null) {
			writer.WriteValue(null);
		} else {
			var thk = (Thickness)value;
			writer.WriteValue($"{thk.Left},{thk.Top},{thk.Right},{thk.Bottom}");
		}
		return true;
	}

	public object? Deserialize(string? value, Type type, IFormatProvider format, string dateTimeFormat, bool suppressId) {
		if (string.IsNullOrWhiteSpace(value)) return default(Thickness);

		var s = value.Split(",");
		if (s.Length == 4 && double.TryParse(s[0], out var l) && double.TryParse(s[0], out var t)&& double.TryParse(s[0], out var r)&& double.TryParse(s[0], out var b)) {
			return new Thickness(l, t, r, b);
		} else {
			throw new InvalidCastException($"Cannot convert {value} to {typeof(Thickness).FullName}.");
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

# Missing Properties in the class
If an xml contains an element that is not represented by the class given to the serializer, the PropertyNotFound event is raised. The deserialization will not fail. You can use this event handler, to handle the property manually with
the unresolvedPropertyNode and the target class.

# The lzyxmlx namespace
Attributes that are used to store meta information the serializer needs to deserialize the xml are stored
in the namespace lzyxmlx. So you can use an attribute "objId" and "clsType" and you do not interfere with the
Serializers infrastructure
- lzyxmlx:objId 	Used as the object identifier (Recursive serialization support)
- lzyxmlx:clsType 	Supports serialization and deserialization of abstract and interface types
