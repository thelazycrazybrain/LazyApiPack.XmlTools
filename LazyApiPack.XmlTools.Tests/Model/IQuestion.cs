using System.Collections.ObjectModel;

namespace LazyApiPack.XmlTools.Tests.Model {
    public interface IQuestion {
        Guid Id { get; set; }
        string? Question { get; set; }
        string? Description { get; set; }
        ObservableCollection<IAnswer> Answers { get; set; }
    }
}
