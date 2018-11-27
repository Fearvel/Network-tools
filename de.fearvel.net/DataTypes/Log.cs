using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using de.fearvel.net.DataTypes.AbstractDataTypes;
using Newtonsoft.Json;

namespace de.fearvel.net.DataTypes
{
    public class Log : JsonSerializable<Log>
    {
        public string ProgramName;
        public string ProgramVersion;
        public string FnLogVersion;
        public string Title;
        public string Description;
        public int LogType;
        public string Guid;        
    }
}
