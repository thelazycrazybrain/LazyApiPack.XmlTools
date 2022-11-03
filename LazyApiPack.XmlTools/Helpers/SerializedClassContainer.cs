namespace LazyApiPack.XmlTools.Helpers {
    /// <summary>
    /// Contains an object that has already been deserialized (for xml compression and recursive serialization support).
    /// </summary>
    public class SerializedClassContainer {
        /// <summary>
        /// Creates an instance of the SerializedClassContainer.
        /// </summary>
        /// <param name="id">The id of the object.</param>
        /// <param name="type">The full name of the object type.</param>
        /// <param name="obj">The deserialized object.</param>
        public SerializedClassContainer(string? id, string type, object obj) {
            Id = id;
            Type = type;
            Object = obj;
        }
        /// <summary>
        /// The Id of the object.
        /// </summary>
        public string? Id { get; }
        /// <summary>
        /// The full name of the object type.
        /// </summary>
        public string Type { get; }
        /// <summary>
        /// The deserialized object.
        /// </summary>
        public object Object { get; }
        /// <summary>
        /// Returns a string representation of the SerializedClassContainer
        /// </summary>
        /// <returns>Returns the Id, Type and the string representation of the deserialized object.</returns>
        public override string ToString() {
            return $"Id {Id} Type {Type} Object {Object?.ToString()}";
        }
    }
}