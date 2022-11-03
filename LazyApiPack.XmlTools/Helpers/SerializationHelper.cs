using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyApiPack.XmlTools.Helpers {
    public static class SerializationHelper {

        /// <summary>
        /// Gets the DateTime as a formatted string using a cultureInfo and a DateTime format.
        /// </summary>
        /// <param name="dateTime">The DateTime that is converted to a string.</param>
        /// <param name="cultureInfo">Culture Info that the string represents.</param>
        /// <param name="dateTimeFormat">The format that the string is using.</param>
        /// <returns></returns>
        public static string? DateTimeToFormattedString(DateTime? dateTime, IFormatProvider? cultureInfo, string? dateTimeFormat) {
            if (!string.IsNullOrWhiteSpace(dateTimeFormat)) {
                return dateTime?.ToString(dateTimeFormat, cultureInfo);
            }
            return dateTime?.ToString(dateTimeFormat, cultureInfo);
        }


        /// <summary>
        /// Tries to serialize a simple type
        /// </summary>
        /// <param name="value">The object to serialize</param>
        /// <param name="serialized">The serialized string representation of value.</param>
        /// <param name="dataType">Type of value.</param>
        /// <param name="cultureInfo">Culture info of the serialized string.</param>
        /// <param name="dateTimeFormat">DateTime format if value is serialized as a DateTime string.</param>
        /// <returns></returns>
        public static bool TrySerializeSimpleType(object? value, out string? serialized, out string? dataType, IFormatProvider cultureInfo, string? dateTimeFormat) {
            if (value == null) {
                serialized = null;
                dataType = null;
                return true;
            }
            switch (value) {
                case Guid g:
                    serialized = g.ToString("", cultureInfo);
                    dataType = "Guid";
                    break;
                case Version v:
                    serialized = v?.ToString(4);
                    dataType = "Version";
                    break;
                case string str:
                    serialized = str;
                    dataType = "string";
                    break;
                case int i:
                    serialized = i.ToString(cultureInfo);
                    dataType = "int";
                    break;
                case byte b:
                    serialized = b.ToString(cultureInfo);
                    dataType = "byte";
                    break;
                case sbyte sb:
                    serialized = sb.ToString(cultureInfo);
                    dataType = "sbyte";
                    break;
                case uint ui:
                    serialized = ui.ToString(cultureInfo);
                    dataType = "uint";
                    break;
                case short s:
                    serialized = s.ToString(cultureInfo);
                    dataType = "short";
                    break;
                case ushort us:
                    serialized = us.ToString(cultureInfo);
                    dataType = "ushort";
                    break;
                case long l:
                    serialized = l.ToString(cultureInfo);
                    dataType = "long";
                    break;
                case ulong ul:
                    serialized = ul.ToString(cultureInfo);
                    dataType = "ulong";
                    break;
                case float f:
                    serialized = f.ToString(cultureInfo);
                    dataType = "float";
                    break;
                case double d:
                    serialized = d.ToString(cultureInfo);
                    dataType = "double";
                    break;
                case char c:
                    serialized = c.ToString(cultureInfo);
                    dataType = "char";
                    break;
                case bool byt:
                    serialized = byt.ToString(cultureInfo);
                    dataType = "bool";
                    break;
                case decimal dec:
                    serialized = dec.ToString(cultureInfo);
                    dataType = "decimal";
                    break;
                case DateTime dt:
                    serialized = DateTimeToFormattedString(dt, cultureInfo, dateTimeFormat);
                    dataType = "datetime";
                    break;
                case Point pt:
                    serialized = pt.X.ToString(cultureInfo) + ";" + pt.Y.ToString(cultureInfo);
                    dataType = "drawing.point";
                    break;
                case Size sz:
                    serialized = sz.Width.ToString(cultureInfo) + ";" + sz.Height.ToString(cultureInfo);
                    dataType = "drawing.size";
                    break;
                case TimeSpan ts:
                    serialized = ts.TotalMilliseconds.ToString(cultureInfo);
                    dataType = "timespan";
                    break;
                default:
                    serialized = null;
                    dataType = null;
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Deserializes a value type.
        /// </summary>
        /// <param name="objectType">The type of the value.</param>
        /// <param name="value">The value represented as a string.</param>
        /// <param name="simpleValue">The parsed simple value, if the deserialization succeeded.</param>
        /// <param name="format">The format that is used to parse the value string.</param>
        /// <param name="dateTimeFormat">The dateTime format that is used to parse the value string.</param>
        /// <returns>True, if the value was deserialized, otherwise false.</returns>
        public static bool TryDeserializeValueType(Type objectType, string value, out object simpleValue, IFormatProvider format, string? dateTimeFormat) {
            if (objectType == typeof(string)) {
                simpleValue = value;
                return true;
            } else if (objectType == typeof(Guid)) {
                simpleValue = Guid.Parse(value);
                return true;
            } else if (objectType == typeof(int)) {
                simpleValue = int.Parse(value, format);
                return true;
            } else if (objectType == typeof(byte)) {
                simpleValue = byte.Parse(value, format);
                return true;
            } else if (objectType == typeof(sbyte)) {
                simpleValue = sbyte.Parse(value, format);
                return true;
            } else if (objectType == typeof(uint)) {
                simpleValue = uint.Parse(value, format);
                return true;
            } else if (objectType == typeof(short)) {
                simpleValue = short.Parse(value, format);
                return true;
            } else if (objectType == typeof(ushort)) {
                simpleValue = ushort.Parse(value, format);
                return true;
            } else if (objectType == typeof(long)) {
                simpleValue = long.Parse(value, format);
                return true;
            } else if (objectType == typeof(ulong)) {
                simpleValue = ulong.Parse(value, format);
                return true;
            } else if (objectType == typeof(float)) {
                simpleValue = float.Parse(value, format);
                return true;
            } else if (objectType == typeof(double)) {
                simpleValue = double.Parse(value, format);
                return true;
            } else if (objectType == typeof(char)) {
                simpleValue = char.Parse(value);
                return true;
            } else if (objectType == typeof(bool)) {
                simpleValue = bool.Parse(value);
                return true;
            } else if (objectType == typeof(decimal)) {
                simpleValue = decimal.Parse(value, format);
                return true;
            } else if (objectType == typeof(Point)) {
                simpleValue = new Point(int.Parse(value.Split(";")[0]), int.Parse(value.Split(";")[1]));
            } else if (objectType == typeof(Size)) {
                simpleValue = new Size(int.Parse(value.Split(";")[0]), int.Parse(value.Split(";")[1]));
            } else if (objectType ==typeof(DateTime)) {
                if (string.IsNullOrWhiteSpace(dateTimeFormat)) {
                    simpleValue = DateTime.Parse(value, format);
                } else {
                    simpleValue = DateTime.ParseExact(value, dateTimeFormat, format);
                }
            } else if (objectType == typeof(TimeSpan)) {
                simpleValue = TimeSpan.FromMilliseconds(double.Parse(value, format));
            } else if (objectType == typeof(Version)) {
                simpleValue = Version.Parse(value);
            } else { 
                simpleValue = null;
                return false;
            }
            return true;
        }



    }
}
