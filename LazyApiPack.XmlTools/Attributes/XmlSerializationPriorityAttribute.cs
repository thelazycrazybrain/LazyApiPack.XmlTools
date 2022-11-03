using System.Net.Mail;

namespace LazyApiPack.XmlTools.Attributes
{
    /// <summary>
    /// Marks this property or field as sortable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class XmlSerializationPriorityAttribute : Attribute
    {
        /// <summary>
        /// Creates an instance of the XmlSerializationPriorityAttribute.
        /// </summary>
        /// <param name="priority">The order in which the property or field is written to the xml. (Higher value means lower priority).</param>
        /// <remarks>XmlAttributes have higher priority than XmlElements.</remarks>
        public XmlSerializationPriorityAttribute(int priority)
        {
            Priority = priority;
        }

        /// <summary>
        /// The order in which the property or field is written to the xml. (Higher value means lower priority).
        /// </summary>
        /// <remarks>XmlAttributes have higher priority than XmlElements.</remarks>
        public int Priority { get; set; }
    }
}