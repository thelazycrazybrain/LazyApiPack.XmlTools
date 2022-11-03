namespace LazyApiPack.XmlTools.Attributes
{
    /// <summary>
    /// Is used to instantiate a Dictionary with a specific EqualityComparer
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class DictionaryEqualityComparerAttribute : Attribute
    {
        /// <summary>
        /// Creates an instance of the DictionaryEqualityComparerAttribute
        /// </summary>
        /// <param name="equalityComparerType">The EqualityComparer type that is used for this property.</param>
        public DictionaryEqualityComparerAttribute(Type equalityComparerType)
        {
            EqualityComparerType = equalityComparerType;
        }

        /// <summary>
        /// Type of the EqualityComparer.
        /// </summary>
        public Type EqualityComparerType { get; set; }
    }
}