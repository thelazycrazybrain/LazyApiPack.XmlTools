using LazyApiPack.XmlTools.Attributes;
using LazyApiPack.XmlTools.Tests.ModelBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LazyApiPack.XmlTools.Tests.Model {
    [XmlClass]
    public class ComplexTypesModel : ExtendedXmlSerializableTestBase {
        public ComplexTypesModel() {
        }

        Dictionary<string, ICollection<IItem>> _dictionary = new Dictionary<string, ICollection<IItem>>();
        [XmlArray("Dictionary")]
        [XmlArrayItem("DictionaryItem")]
        public Dictionary<string, ICollection<IItem>> Dictionary
        {
            get => _dictionary;
            set => SetPropertyValue(ref _dictionary, value);
        }

        public ICollection<IItem>[] _array;
        [XmlArray("Array")]
        [XmlArrayItem("ArrayItem")]
        public ICollection<IItem>[] Array
        {
            get => _array;
            set => SetPropertyValue(ref _array, value);
        }

        private IList<string> _list = new List<string>();
        [XmlArray("List")]
        [XmlArrayItem("ListItem")]
        public IList<string> List
        {
            get => _list;
            set => SetPropertyValue(ref _list, value);
        }
    }

    [XmlClass]
    public class CarItem : XmlSerializableTestBase, IItem {
        private string _name;
        [XmlAttribute("Name")]
        public string Name
        {
            get => _name;
            set => SetPropertyValue(ref _name, value);
        }
    }

    [XmlClass]
    public class PlaneItem : XmlSerializableTestBase, IItem {
        private string _name;
        [XmlAttribute("Name")]
        public string Name
        {
            get => _name;
            set => SetPropertyValue(ref _name, value);
        }
    }

    public interface IItem {
        string Name { get; set; }
    }
}
