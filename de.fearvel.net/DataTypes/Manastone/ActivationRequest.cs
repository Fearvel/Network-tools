using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de.fearvel.net.DataTypes.AbstractDataTypes;
using de.fearvel.net.Security;

namespace de.fearvel.net.DataTypes.Manastone
{
   public sealed class ActivationRequest : JsonSerializable<ActivationRequest>
   {
       public string SerialNumber;
       public string HardwareId;
       public string ProductUUID;

       public ActivationRequest(string serialNumber, string productUuid)
       {
           
            SerialNumber = serialNumber;
           HardwareId = Ident.GetCPUId();
           ProductUUID = productUuid;
       }
   }
}
