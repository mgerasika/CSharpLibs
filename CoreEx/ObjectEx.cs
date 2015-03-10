using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace CoreEx
{
    public interface IObjectEx
    {
        string _cstype { get; }
    }

    public class ObjectEx : IObjectEx
    {
        private Dictionary<string, object> _fields = new Dictionary<string, object>();

        public string _cstype
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public virtual object GetValue(string name)
        {
            if (_fields.ContainsKey(name))
            {
                return _fields[name];
            }
            return null;
        }

        public virtual bool ContainsKey(string name)
        {
            return (_fields.ContainsKey(name));
        }

        public string GetValueStr(string name)
        {
            return GetValue(name) as String;
        }

        public T GetValueCustom<T>(string name) {
            T res;
            object value = GetValue(name);
            if (!(value is T)) {
                TypeConverter typeConverter = TypeDescriptorEx.GetConverter(typeof (T));
                res = (T) typeConverter.ConvertTo(value, typeof (T));
            }
            else {
                res = (T)value;
            }
            return res;
        }

        public bool GetValueBool(string name)
        {
            return GetValueCustom<bool>(name);
        }

        public DateTime GetValueDate(string name)
        {
            return GetValueCustom<DateTime>(name);
        }

        public int GetValueInt(string name)
        {
            return GetValueCustom<int>(name);
        }

        public long GetValueLong(string name)
        {
            return GetValueCustom<long>(name);
        }

        public float GetValueFloat(string name)
        {
            return GetValueCustom<float>(name);
        }

        public double GetValueNum(string name)
        {
            return GetValueCustom<double>(name);
        }

        public Guid GetValueGuid(string name)
        {
            return GetValueCustom<Guid>(name);
        }

        public object GetValue(string name, object defaultValue)
        {
            object res = GetValue(name);
            return (null == res) ? defaultValue : res;
        }

        public virtual void SetValue(string name, object value)
        {
            _fields[name] = value;
        }

        public void SetValueReflection(string name, object value)
        {
            PropertyInfo lProperty = this.GetType().GetProperty(name);
            if (null != lProperty && lProperty.CanWrite)
            {
                TypeConverter lTypeConverter = TypeDescriptorEx.GetConverter(lProperty.PropertyType);
                lProperty.SetValue(this, lTypeConverter.ConvertTo(value, lProperty.PropertyType), null);
            }
            else
            {
                FieldInfo lFieldInfo = this.GetType().GetField(name);
                if (null != lFieldInfo)
                {
                    TypeConverter lTypeConverter = TypeDescriptorEx.GetConverter(lFieldInfo.FieldType);
                    lFieldInfo.SetValue(this, lTypeConverter.ConvertTo(value, lFieldInfo.FieldType));
                }
            }
        }

        public void Deserialize(IDictionary values)
        {
            try
            {
                foreach (DictionaryEntry lEntry in values)
                {
                    SetValueReflection(lEntry.Key.ToString(),lEntry.Value);
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false,ex.Message);
            }
        }


        public IEnumerable<KeyValuePair<string, object>> GetFields()
        {
            return _fields;
        }
    }//ObjectEx
}
