using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS1
{
    public class BaseMeter
    {
        public virtual void Read(string descr) { }
        public virtual string Write() { string str = ""; return str; }
       
    }
}
