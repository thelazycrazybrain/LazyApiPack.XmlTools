//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;

//namespace LazyApiPack.XmlTools.ExtendedObjectSerializers {
//    public static class ExtendedTypeXmlSerializer {
//        public static ExtendedXmlSerializer<TClass> UseExtendedTypeSupport<TClass>(this ExtendedXmlSerializer<TClass> serializer) where TClass : class {
//            serializer.ExternalSerializers.Add(new VersionSerializer());
//            return serializer;
//        }

//        public class VersionSerializer : IExternalObjectSerializer {
//            public bool Serialize(XmlWriter writer, object? value, bool serializeAsAttribute, IFormatProvider format, string dateTimeFormat, bool suppressId) {
//                if (value == null) {
//                    writer.WriteValue("");
//                } else {
//                    writer.WriteValue(((Version)value).ToString(4));
//                }
//                return true;
//            }

//            public object? Deserialize(string? value, Type type, IFormatProvider format, string dateTimeFormat, bool suppressId) {
//                return string.IsNullOrWhiteSpace(value) ? null : Version.Parse(value);
//            }

//            public bool SupportsType(Type type) {
//                return type == typeof(Version);
//            }


//        }
//    }
//}
