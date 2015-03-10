using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web.Configuration;

namespace CoreEx
{
    public class Logger
    {
        /*
         all - 0,
         trace = 1
         */
        private static Logger _instance = new Logger();
        private static string _path;
        private static object _lockObject = new object();

        private  Logger() {
            _path = Directory.GetCurrentDirectory();
        }

        public void Error(object msg)
        {
            try
            {
                lock (_lockObject)
                {
                    string date = String.Format("{0}-{1}_", DateTime.Now.Year,DateTime.Now.Month);
                    using (StreamWriter streamWriter = new StreamWriter(String.Format("{0}\\{1}error.log", _path,date),true))
                    {
                        string s = String.Format("{0} {1}", DateTime.Now.ToLocalTime(), msg);
                        Console.WriteLine(s);
                        streamWriter.WriteLine(s);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Trace(string msg)
        {
            try
            {
                lock (_lockObject) {
                    string date = String.Format("{0}-{1}_", DateTime.Now.Year, DateTime.Now.Month);
                    using (
                        StreamWriter streamWriter = new StreamWriter(String.Format("{0}\\{1}trace.log", _path,date),
                                                                     true))
                    {
                        string s = String.Format("{0} {1}", DateTime.Now.ToLocalTime(), msg);
                        Console.WriteLine(s);
                        streamWriter.WriteLine(s);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static Logger Inst {
            get { return _instance; }
        }

        public void Init(string path) {
            _path = path;
        }
    }
}
