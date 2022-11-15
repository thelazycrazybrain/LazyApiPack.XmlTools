# About this project
This library enables class serialization to a zip file.
The class is serialized with the ExtendedXmlSerializer and the resources within the class (ZipResource) are stored as separate files within the zip file.

# How to design a Zip serializable class
```csharp
    [XmlClass] // Marks this class as serializable (ExtendedXmlSerializer)
    public class ImageModel {
        List<ZipResource> _images = new List<ZipResource>();
        [XmlArray("Images")]
        [XmlArrayItem("Image")]
        public List<ZipResource> Images { get => _images; set => _images = value; } // A list of resources

        private ZipResource _mainImage;
        [XmlAttribute]
        public ZipResource MainImage { get => _mainImage; set => _mainImage=value; } // A resource can be an attribute!
    }

```
To fill the resource with data, just set the property with 
```csharp
_model.MainImage = new ZipResource(streamOrByteArrayOfResource);
```
or 
```csharp
_model.MainImage.SetStream(streamOfResource);
```
or
```csharp
_model.MainImage.Data = byteArrayOfResource;
```

# How to serialize to a Zip file

```csharp
    var xmlSerializer = new ExtendedXmlSerializer<ImageModel>(); // Create the xml serializer and configure it as you like
    var zipSerializer = new ZipSerializer<ImageModel>(xmlSerializer); // Create the zip serializer and configure it as you like
    var strm = zipSerializer.Serialize(_model, true, System.IO.Compression.CompressionLevel.Optimal); // Serialize the model
```

# How to deserialize from a Zip file

```csharp
    var xmlSerializer = new ExtendedXmlSerializer<ImageModel>(); // Create the xml serializer and configure it as you like
    var zipSerializer = new ZipSerializer<ImageModel>(xmlSerializer); // Create the zip serializer and configure it as you like
    var deserialized = zipSerializer.Deserialize(strm);

```

# Compression and performance
You can optimize the serializer in two ways:
- CompressionLevel: the .NET zip archive uses this to configure the compression algorithm
- CheckDuplicates: the serializer will compare every resource and calculates a sha256 hash. If a resource produces the same hash, the serializer will not store this resource in the zip file but will use the existing file instead.

