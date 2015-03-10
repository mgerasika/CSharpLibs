using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SQLEx
{
    public class SafeReader
    {
        private const string ValueNULL = "NULL";

        public static string GetNullValue()
        {
            return ValueNULL;
        }

        public static string Escape(string lVal)
        {
            string lRes = lVal.ToString().Replace("'", "''");
            return lRes;
        }


        public static string Get(object lVal)
        {
            string lRes = GetNullValue();

            if (lVal is DateTime)
            {
                lRes = ConvertDateTime((DateTime)lVal);
            }
            else if(lVal is Guid)
            {
                lRes = String.Format("'{0}'", lVal);
            }
            else if (lVal is String)
            {
                lRes = lVal.ToString().Replace("'", "''");
                lRes = String.Format("N'{0}'", lRes);
            }
            else
            {
                lRes = Convert.ToString(lVal,new CultureInfo("en-US"));
            }

            return lRes;
        }

        public static string ConvertDateTime(DateTime value)
        {
            if (DateTime.MinValue.Equals(value))
                return GetNullValue();
            if (DateTime.MaxValue.Equals(value))
                value = DateTime.Now.ToUniversalTime();
            return string.Format("convert(datetime, '{0}/{1}/{2} {3}:{4}:{5}', 101)",
                value.Month, value.Day, value.Year,
                value.Hour, value.Minute, value.Second);
        }
    }
}
