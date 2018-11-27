using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.fearvel.net.Exceptions
{
    public class InstanceNotSetException : Exception
    {
        public InstanceNotSetException(){}
        public InstanceNotSetException(string s) : base(s){}
    }
}
