using System;
using System.Collections.Generic;
using CoreEx;

namespace MvcEx
{
    public class point : ObjectEx
    {
        public int x;
        public int y;
       
    }
    public class UserController : ControllerBase
    {
        public string Login()
        {
            return "login";
        }

        public int Sum(int x,int y)
        {
            return x+y;
        }

        public int Sum2(point p1)
        {
            return p1.x + p1.y;
        }

        public int Sum3(point p1,point p2)
        {
            return p1.x + p1.y + p2.x + p2.y;
        }

        public int Sum4(point p1, int x)
        {
            return p1.x + p1.y + x;
        }

        public int Sum_array(int[] x)
        {
            int sum = 0;
            foreach (int c in x)
            {
                sum += c;
            }
            return sum;
        }

        public int Sum_list(List<int> x)
        {
            int sum = 0;
            foreach (int c in x)
            {
                sum += c;
            }
            return sum;
        }
    }
}
