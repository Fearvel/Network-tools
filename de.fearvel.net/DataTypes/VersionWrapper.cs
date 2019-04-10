using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes
{
    public class VersionWrapper: JsonSerializable<VersionWrapper>
    {
        public string Version;

    }
}
