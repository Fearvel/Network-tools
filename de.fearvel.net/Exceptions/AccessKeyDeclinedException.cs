using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.fearvel.net.Exceptions
{
    public class AccessKeyDeclinedException : Exception
    {
        public AccessKeyDeclinedException() { }
        public AccessKeyDeclinedException(string s) : base(s) { }
    }
}
