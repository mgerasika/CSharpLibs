using System;
using System.Collections.Generic;
using System.Text;
using CoreEx;
using MvcEx.httpclient;

namespace MvcEx
{
    public abstract class AbstractFactoryBase
    {
        public abstract DataManagerBase CreateDataManager();

        public virtual ControllerBase CreateController(IHttpContextEx context, Type type)
        {
            return Activator.CreateInstance(type) as ControllerBase;
        }

        public AbstractFactoryBase() {
        }
    }
}
