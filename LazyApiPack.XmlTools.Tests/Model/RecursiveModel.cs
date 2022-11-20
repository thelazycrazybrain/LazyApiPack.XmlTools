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
    public class RecursiveModel : XmlSerializableTestBase {
        public RecursiveModel() {

        }

        public RecursiveModel(string name) : this() {
            Name = name;
        }
        private string _name = "Recursive Model";
        [XmlAttribute]
        public string Name
        {
            get => _name;
            set => SetPropertyValue(ref _name, value);
        }

        private ObservableCollection<RecursiveModel> _children = new ObservableCollection<RecursiveModel>();
        [XmlArray("children")]
        [XmlArrayItem("child")]
        public ObservableCollection<RecursiveModel> Children
        {
            get => _children;
            set => SetPropertyValue(ref _children, value);
        }

        public override string ToString() {
            return Name; // Do not try to show Children here -> infinite recursion!
        }
    }

}
