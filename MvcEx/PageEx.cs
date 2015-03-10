using System.Collections.Generic;
using System.Web.UI;

namespace MvcEx
{
    public class PageEx : Page
    {
        public Dictionary<string,object> ViewData
        {
            get
            {
                Dictionary<string, object> dic = this.Context.Session["ViewData"] as Dictionary<string, object>;
                return dic;
            }
        }
    }
}
