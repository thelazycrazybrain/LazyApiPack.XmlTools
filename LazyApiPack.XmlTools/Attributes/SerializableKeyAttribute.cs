namespace LazyApiPack.XmlTools.Attributes
{
    /// <summary>
    /// Marks this property as the unique identifier of this class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class XmlClassKeyAttribute : Attribute
    {
        public XmlClassKeyAttribute()
        {

        }
    }
}