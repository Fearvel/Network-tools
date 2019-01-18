using System;
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    public class ManastoneActivationWrap : JsonSerializable<ManastoneActivationWrap>
    {
        public DateTime DateOfExpiry;

        // ReSharper disable once InconsistentNaming
        public string programUUID;

        public ManastoneActivationWrap(DateTime dateOfExpiry, string programUuid)
        {
            DateOfExpiry = dateOfExpiry;
            programUUID = programUuid;
        }

        public static ManastoneActivationWrap DecryptAndDeserialize(string s, string key, string iv)
        {
            return DeSerialize(de.fearvel.net.Security.Crypto.SimpleDES.Decrypt(s, key, iv));
        }
    }
}