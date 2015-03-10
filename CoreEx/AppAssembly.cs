using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.Configuration;

namespace CoreEx
{
    public class AppAssembly
    {
        private static object _lockObject = new object();
        private static AppAssembly _instance;
        private static readonly Dictionary<string, Type> c_aControllerCache = new Dictionary<string, Type>();
        private static readonly Dictionary<string, Type> c_aObjectCache = new Dictionary<string, Type>();

        private AppAssembly()
        {
            lock (_lockObject)
            {
                try
                {
                    //Assembly mvcAssembly = Assembly.Load(WebConfigurationManager.AppSettings["mvcApp"]);
                    //AddAssembly(mvcAssembly);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Exception exSub in ex.LoaderExceptions)
                    {
                        sb.AppendLine(exSub.Message);
                        if (exSub is FileNotFoundException)
                        {
                            FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                            if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                            {
                                sb.AppendLine("Fusion Log:");
                                sb.AppendLine(exFileNotFound.FusionLog);
                            }
                        }
                        sb.AppendLine();
                    }
                    string errorMessage = sb.ToString();
                    Logger.Inst.Error(errorMessage);
                }

                Assembly[] assemblyes = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblyes)
                {
                    try
                    {
                        AddAssembly(assembly);
                    }
                    catch (Exception ex)
                    {
                        Logger.Inst.Error(String.Format("Failed load assembly {0}. Message:{1}",assembly.FullName, ex.ToString()));
                    }
                }

            }
        }

        private void AddAssembly(Assembly a)
        {
            lock (c_aControllerCache)
            {
                Type objectType = typeof(ObjectEx);
                foreach (Type lType in a.GetTypes())
                {
                    if (lType.Name.EndsWith("Controller") && !c_aControllerCache.ContainsKey(lType.Name))
                    {
                        c_aControllerCache.Add(lType.Name, lType);
                    }
                    if (lType.IsSubclassOf(objectType) && !c_aObjectCache.ContainsKey(lType.FullName))
                    {
                        c_aObjectCache.Add(lType.FullName, lType);
                    }
                }
            }
        }

        public static AppAssembly Inst()
        {
            if(null == _instance)
            {
                _instance = new AppAssembly();
            }
            return _instance;
        }

        public bool HasController(string shortName)
        {
            return c_aControllerCache.ContainsKey(shortName);
        }

        public bool HasObject(string shortName)
        {
            return c_aObjectCache.ContainsKey(shortName);
        }

        public Type GetControllerType(string fullName)
        {
            if (c_aControllerCache.ContainsKey(fullName))
            {
                return c_aControllerCache[fullName];
            }
            return null;
        }

        public Type GetObjectType(string fullName)
        {
            if (c_aObjectCache.ContainsKey(fullName))
            {
                return c_aObjectCache[fullName];
            }
            return null;
        }
    }
}
