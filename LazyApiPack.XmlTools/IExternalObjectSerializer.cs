using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using LazyApiPack.XmlTools.Helpers;

namespace LazyApiPack.XmlTools {
    /// <summary>
    /// Creates an extension to the ExtendedXmlSerializer that supports serialization of various custom types.
    /// </summary>
    public interface IExternalObjectSerializer {
        /// <summary>
        /// Determines if this class supports serializing / deserializing the given type.
        /// </summary>
        /// <param name="type">The type that the serializer tries to serialize/deserialize</param>
        /// <param name="dataFormat">Specifies a format in which the data is stored (is null while serializing).</param>
        /// <returns>True, if the serializer supports the given type.</returns>
        bool SupportsType(Type type, string? dataFormat);
        /// <summary>
        /// Deserializes the value to an object.
        /// </summary>
        /// <param name="node">The node that contains the value.</param>
        /// <param name="type">The object type that value represents.</param>
        /// <param name="format">The format that the current serializer uses.</param>
        /// <param name="dateTimeFormat">The dateTime format that the current serializer uses.</param>
        /// <param name="enableRecursiveSerialization">Enables recursive object serialization.</param>
        /// <param name="dataFormat">Specifies a format in which the data was stored.</param>
        /// <returns>The deserialized object of value.</returns>
        object? Deserialize(XElement node, Type type, IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, string? dataFormat);
        /// <summary>
        /// Serializes the object to xml
        /// </summary>
        /// <param name="writer">Reference to the curernt writer with the starting element (XmlElement, XmlAttribute)</param>
        /// <param name="value">Value as object that needs to be serialized.</param>
        /// <param name="serializeAsAttribute">True, if the writer is in WriteStartAttribute context - creating elements is not possible!</param>
        /// <param name="format">Current format setting of the serializer.</param>
        /// <param name="dateTimeFormat">Current DateTime Format of the Serializer.</param>
        /// <param name="enableRecursiveSerialization">True if the serializer uses object ids to enable recursive serialization.</param>
        /// <param name="setDataFormat">Writes the data format property to the xml. (Call this method BEFORE you write anything to the writer!)</param>
        /// <returns>True, if the serialization succeeded.</returns>
        /// <remarks>use writer.WriteValue serializeAsAttribute is True. Otherwise the writer is in ElementStart position.</remarks>
        /// <remarks>Do not close the current node yourself</remarks>
        /// <remarks>If serializeAsAttribute is False, you can create as many subnodes as you want.</remarks>
        /// <remarks>If you need to set a data format, call the setDataFormat method BEFORE you write anything to the writer)</remarks>
        bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, 
                       IFormatProvider format, string? dateTimeFormat, bool enableRecursiveSerialization, Action<string> setDataFormat);
    }
}