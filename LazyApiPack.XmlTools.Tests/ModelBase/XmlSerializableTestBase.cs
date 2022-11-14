using System.ComponentModel;
using System.Runtime.CompilerServices;
using LazyApiPack.XmlTools.Tests.Model;

namespace LazyApiPack.XmlTools.Tests.ModelBase
{
    public abstract class XmlSerializableTestBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event MethodCalledEventHandler? MethodCalled;

        protected virtual void SetPropertyValue<TValue>(ref TValue backingField, TValue value, [CallerMemberName] string? propertyName = null)
        {
            backingField = value;
            OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void NotifyMethodCall([CallerMemberName] string? propertyName = null, params object[] parameters)
        {
            MethodCalled?.Invoke(this, new MethodCalledEventArgs(propertyName, parameters));
        }
    }
}
