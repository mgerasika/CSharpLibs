using System;
using System.Collections.Generic;
using System.Text;

namespace MvcEx
{
    public class Factory
    {
        private static Factory _instance = new Factory();
        private AbstractFactoryBase _factoryBase = null;

        private Factory()
        {
            
        }

        public AbstractFactoryBase GetAbstactFactory()
        {
            if(null == _factoryBase)
            {
                throw new NullReferenceException("RegisterFactory before use this method");
            }
            return _factoryBase;
        }

        public void RegisterFactory(AbstractFactoryBase factory)
        {
            _factoryBase = factory;
        }

        public static Factory Inst()
        {
            return _instance;
        }

        
    }
}
