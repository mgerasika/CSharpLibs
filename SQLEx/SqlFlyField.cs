using CoreEx;

namespace SQLEx
{
    public class SqlFlyField : ObjectEx
    {
        public string ID
        {
            get { return GetValueStr("ID"); }
            set { SetValue("ID", value); }
        }

        public string Name
        {
            get { return GetValueStr("Name"); }
            set { SetValue("Name", value); }
        }

        public string Value
        {
            get { return GetValueStr("Value"); }
            set { SetValue("Value", value); }
        }
    }
}
