using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class CurrentUser
    {
        public string Name { get; set; }
        public string Pwd { get; set; }

        public bool IsSameUser(string name, string pwd)
        {
            return name.Equals(Name) && pwd.Equals(Pwd);
        }
    }
}
