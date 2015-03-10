using System;
using CoreEx;

namespace MvcEx
{
    public class RequestBase : ObjectEx
    {
        public virtual ResponseBase Send()
        {
            return new SuccessResponse();
        }

        public virtual void Validate()
        {
        }

        public Guid DebugID
        {
            get { return GetValueGuid("DebugID"); }
            set { SetValue("DebugID",value); }
        }
    }
}
