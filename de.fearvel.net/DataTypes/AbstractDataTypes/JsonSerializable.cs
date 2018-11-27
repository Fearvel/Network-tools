using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace de.fearvel.net.DataTypes.AbstractDataTypes
{
    public abstract class JsonSerializable<T>
    {
        public string Serialize() =>
            JsonConvert.SerializeObject(this, Formatting.Indented).Trim().
                Replace(System.Environment.NewLine, "");
        public static T DeSerialize(string s) =>
            JsonConvert.DeserializeObject<T>(s);
    }
}
