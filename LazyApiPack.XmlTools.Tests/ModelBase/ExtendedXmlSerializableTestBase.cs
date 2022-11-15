using System.Runtime.CompilerServices;

namespace LazyApiPack.XmlTools.Tests.ModelBase
{
    public abstract class ExtendedXmlSerializableTestBase : XmlSerializableTestBase, IExtendedXmlSerializable
    {
        internal bool IsSerializing { get; private set; }
        internal bool IsDeserializing { get; private set; }
        internal bool IsInvalid { get; private set; }

        public virtual void OnDeserialized(bool success)
        {
            IsDeserializing = false;
            IsInvalid = !success;
            NotifyMethodCall(nameof(OnDeserialized), success);
        }
        public virtual void OnDeserializing()
        {
            IsDeserializing = true;
            NotifyMethodCall();
        }
        public virtual void OnSerialized(bool success)
        {
            IsSerializing = false;
            NotifyMethodCall(nameof(OnSerialized), success);
        }
        public virtual void OnSerializing()
        {
            IsSerializing = true;
            NotifyMethodCall();
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (!IsSerializing && !IsDeserializing)
            {
                base.OnPropertyChanged(propertyName);
            }
        }
    }
}
