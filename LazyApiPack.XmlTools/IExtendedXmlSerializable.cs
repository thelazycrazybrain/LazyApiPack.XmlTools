namespace LazyApiPack.XmlTools {
    /// <summary>
    /// Marks a class as Serializable that is aware of the serialization process.
    /// </summary>
    public interface IExtendedXmlSerializable {
        /// <summary>
        /// Is called, before the class is being deserialized.
        /// </summary>
        void OnDeserializing();
        /// <summary>
        /// Is called, when the deserialization process is completed.
        /// </summary>
        /// <param name="success">True, if the deserialization was successful. If False, the class is in an invalid state.</param>
        void OnDeserialized(bool success);
        /// <summary>
        /// Is called before the class is being serialized.
        /// </summary>
        void OnSerializing();
        /// <summary>
        /// Is called when the class is serialized.
        /// </summary>
        /// <param name="success">True, if the serialization was successful.</param>
        void OnSerialized(bool success);
    }
}