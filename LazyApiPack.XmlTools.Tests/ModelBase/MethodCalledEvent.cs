namespace LazyApiPack.XmlTools.Tests.ModelBase {
    public delegate void MethodCalledEventHandler(object sender, MethodCalledEventArgs e);
    public class MethodCalledEventArgs : EventArgs {
        public MethodCalledEventArgs(string? methodName, params object[] parameters) : base() {
            MethodName = methodName;
            Parameters = parameters ?? Array.Empty<object>();
        }

        public string? MethodName { get; private set; }

        public object[] Parameters { get; private set; }

    }
}
