using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;

namespace CoreEx
{
    public static class ExtensionMethods
    {
        public static string ToStringShort(TimeSpan time)
        {
            string res = String.Format("{0}:{1}:{2}", time.Hours, time.Minutes, time.Seconds);
            return res;
        }

        public static bool EqualsYearMonthDay(DateTime date, DateTime date2)
        {
            bool bRes = (date.Year == date2.Year && date.Month == date2.Month && date.Day == date2.Day);
            return bRes;
        }

        public static DateTime? ParseDate(object value)
        {
            DateTime? res = null;
            string separator = WebConfigurationManager.AppSettings["DateSeparator"];
            if(string.IsNullOrEmpty(separator))
            {
                separator = "/";
            }
            if (null != value && (value.ToString() != String.Empty))
            {
                if (typeof(DateTime) == value.GetType())
                {
                    res = (DateTime)value;
                }
                else if (value.ToString().Contains("Date"))
                {
                    string val = value.ToString();
                    int idx = val.IndexOf('(');
                    val = val.Substring(idx + 1);
                    idx = val.IndexOf(')');
                    val = val.Substring(0, idx);
                    long msSinceEpoch = long.Parse(val);
                    DateTime dt = new DateTime(1970, 1, 1) + new TimeSpan(msSinceEpoch * 10000);
                    res = dt;
                }
                else if (value.ToString().Contains(separator))
                {
                    string[] arr = value.ToString().Split(separator[0]);
                    Debug.Assert(arr.Length == 3);
                    if (arr.Length == 3)
                    {
                        int month = int.Parse(arr[0]);
                        int day = int.Parse(arr[1]);
                        int yearh = 0;

                        int hours = 0;
                        int minutes = 0;
                        int secconds = 0;

                        int idx = arr[2].IndexOf(' ');
                        if (idx != -1)
                        {
                            yearh = int.Parse(arr[2].Remove(idx));

                            string str = arr[2].Remove(0, idx);
                            string[] arr2 = str.Split(':');
                            Debug.Assert(arr.Length == 3);

                            hours = int.Parse(arr2[0]);
                            minutes = int.Parse(arr2[1]);
                            secconds = int.Parse(arr2[2]);
                        }
                        else
                        {
                            yearh = int.Parse(arr[2]);
                        }

                        res = new DateTime(yearh, month, day, hours, minutes, secconds);
                    }
                }

                if (res.HasValue)
                {
                    if (ExtensionMethods.EqualsYearMonthDay(res.Value,DateTime.MinValue))
                    {
                        res = DateTime.MinValue;
                    }
                }
            }
            return res;
        }
    }

    public static class TypeDescriptorEx
    {
        private static readonly Dictionary<string, TypeConverter> _hashInstances = new Dictionary<string, TypeConverter>();
        private static readonly Dictionary<string, Type> _converters = new Dictionary<string, Type>();

        public static void RegisterConverter(string name,Type converterType)
        {
            if (_converters.ContainsKey(name))
            {
                Debug.Assert(false);
            }
            _converters.Add(name, converterType);
        }

        static TypeDescriptorEx()
        {
            TypeDescriptorEx.RegisterConverter(typeof(DateTime?).FullName, typeof(DateTimeNullableConverterEx));
            TypeDescriptorEx.RegisterConverter(typeof(DateTime).FullName, typeof(DateTimeConverterEx));
            TypeDescriptorEx.RegisterConverter(typeof(int).FullName, typeof(IntConverterEx));
            TypeDescriptorEx.RegisterConverter(typeof(int?).FullName, typeof(IntNullableConverterEx));
            TypeDescriptorEx.RegisterConverter(typeof(double?).FullName, typeof(DoubleNullableConverterEx));
            TypeDescriptorEx.RegisterConverter(typeof(double).FullName, typeof(DoubleConverterEx));
            TypeDescriptorEx.RegisterConverter(typeof(object).FullName, typeof(ObjectConverterEx));
            TypeDescriptorEx.RegisterConverter("enum", typeof(NewEnumConverterEx));
            TypeDescriptorEx.RegisterConverter("array", typeof(ArrayConverterEx));
            TypeDescriptorEx.RegisterConverter("list", typeof(ListConverterEx));
            TypeDescriptorEx.RegisterConverter(typeof(bool).FullName, typeof(BooleanConverterEx));
            TypeDescriptorEx.RegisterConverter(typeof(bool?).FullName, typeof(BooleanNullableConverterEx));
            TypeDescriptorEx.RegisterConverter(typeof(Guid).FullName, typeof(GuidConverter));
            TypeDescriptorEx.RegisterConverter(typeof(Guid?).FullName, typeof(GuidNullableConverter));
            TypeDescriptorEx.RegisterConverter(typeof(ObjectEx).FullName, typeof(ComplexTypeConverter));
        }

        private static string GetName(Type type)
        {
            string name = type.FullName;
            if (type.IsArray)
            {
                name = "array";
            }
            else if (type.IsGenericType && type.FullName.Contains("List"))
            {
                name = "list";
            }
            else if (type.IsEnum)
            {
                name = "enum";
            }
            else if (type.IsSubclassOf(typeof(ObjectEx)))
            {
                name = typeof(ObjectEx).FullName;
            }
            return name;
        }
        public static TypeConverter GetConverter(Type type)
        {
            TypeConverter lConvertert = null;

            string name = GetName(type);

            if (_hashInstances.ContainsKey(name))
            {
                lConvertert = _hashInstances[name];
            }
            else if (typeof(ObjectEx).IsSubclassOf(type))
            {
                lConvertert = _hashInstances[typeof(ObjectEx).FullName]; 
            }
            else 
            {
                if (_converters.ContainsKey(name))
                {
                    Type convertertType = _converters[name];
                    lConvertert = Activator.CreateInstance(convertertType) as TypeConverter;
                }
                else
                {
                    lConvertert = TypeDescriptor.GetConverter(type);
                }
                _hashInstances[name] = lConvertert;
            }

            return lConvertert;
        }
    }

    public class DateTimeNullableConverterEx : TypeConverter
    {
        public DateTime? ConvertTo(object value)
        {
            return ExtensionMethods.ParseDate(value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            DateTime? res = ConvertTo(value);
            return res;
        }
    }

    public class DateTimeConverterEx : TypeConverter
    {
        public DateTime ConvertTo(object value)
        {
            DateTime? res = ExtensionMethods.ParseDate(value);
            return res.HasValue ? res.Value : DateTime.MinValue;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            DateTime res = ConvertTo(value);
            return res;
        }
    }

    public class IntNullableConverterEx : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (null != value)
            {
                int? res = Convert.ToInt32(value.ToString());
                return res;
            }
            return null;
        }
    }

    public class NewEnumConverterEx : EnumConverter
    {
        public NewEnumConverterEx():base(typeof(object))
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            object val = Enum.Parse(destinationType, value.ToString());
            return val;
        }
    }

    public class DoubleNullableConverterEx : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (null != value)
            {
                double? res = Convert.ToDouble(value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                return res;
            }
            return null;
        }
    }

    public class DoubleConverterEx : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (null != value)
            {
                double res = Convert.ToDouble(value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                return res;
            }
            return 0;
        }
    }

    public class ListConverterEx : ArrayConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            Type elementType = destinationType.GetGenericArguments()[0];
            IList res = Activator.CreateInstance(destinationType) as IList;
            try
            {
                if (null != value && !string.IsNullOrEmpty(value.ToString()) && value.ToString() != "undefined")
                {
                    if(value is JContainer)
                    {
                        value = (value as JContainer).ToObject<object[]>();
                    }
                    object[] values = (object[])value;
                    foreach (object val in values)
                    {
                        TypeConverter converter = TypeDescriptorEx.GetConverter(elementType);
                        object obj = converter.ConvertTo(val, elementType);

                        res.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                throw ex;
            }

            return res;
        }
    }

    public class IntConverterEx : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (null != value)
            {
                int res = Convert.ToInt32(value.ToString(), CultureInfo.GetCultureInfo("en-US"));
                return res;
            }
            return 0;
        }
    }

    public class ObjectConverterEx : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return value;
        }
    }

    public class BooleanConverterEx : BooleanConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (null != value)
            {
                string sVal = value.ToString().ToLower();
                if (sVal.Contains("t") || sVal.Contains("1"))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class BooleanNullableConverterEx : BooleanConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (null != value)
            {
                string sVal = value.ToString().ToLower();
                if (sVal.Contains("t") || sVal.Contains("1"))
                {
                    return true;
                }
                else if (sVal.Contains("f") || sVal.Contains("0"))
                {
                    return false;
                }
            }
            return null;
        }
    }

    public class GuidConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            Guid res = Guid.Empty;
            if (null != value && value.ToString().Length == 36)
            {
                res = new Guid(value.ToString());
                return res;
            }
            return res;
        }
    }

    public class GuidNullableConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            Guid? res = null;
            if (null != value && value.ToString().Length == 36)
            {
                res = new Guid(value.ToString());
                return res;
            }
            return res;
        }
    }

    public class ComplexTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType2)
        {
            ObjectEx instance = null;
            try
            {
                if (null != value && !string.IsNullOrEmpty(value.ToString()) && value.ToString() != "undefined")
                {
                    if (value is JContainer)
                    {
                        value = (value as JContainer).ToObject<Dictionary<String, object>>();
                    }
                    Dictionary<String, object> lValues = (Dictionary<String, object>)value;

                    string sInstanceType = lValues["_cstype"] as string;
                    Type lInstanceType = AppAssembly.Inst().GetObjectType(sInstanceType);
                    Debug.Assert(null != lInstanceType);
                    instance = Activator.CreateInstance(lInstanceType) as ObjectEx;
                    instance.Deserialize(lValues);
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                throw ex;
            }

            return instance;
        }
    }

    public class ArrayConverterEx : ArrayConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            Type arrayType = destinationType.GetElementType();
            TypeConverter converter = TypeDescriptorEx.GetConverter(destinationType);
            ArrayList lRes = new ArrayList();
            if (null != value && value.ToString() != String.Empty)
            {
                if (value is JContainer)
                {
                    value = (value as JContainer).ToObject<object[]>();
                }
                object[] lSource = (object[])value;
                for (int i = 0; i < lSource.Length; ++i)
                {
                    object lSourceObj = lSource[i];
                    object lObj = converter.ConvertTo(lSourceObj, arrayType);
                    lRes.Add(lObj);
                }
            }
            return lRes.ToArray(arrayType);
        }
    }
}
