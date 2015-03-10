using System;
using CoreEx;

namespace MvcEx
{
    public class ResponseBase : ObjectEx
    {
        public Guid DebugID
        {
            get { return GetValueGuid("DebugID"); }
            set { SetValue("DebugID", value); }
        }
    }
}
