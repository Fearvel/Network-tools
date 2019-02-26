using System;
using de.fearvel.net.DataTypes.AbstractDataTypes;

namespace de.fearvel.net.DataTypes.Manastone
{
    /// <summary>
    /// ManastoneActivationWrap
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class ManastoneActivationWrap : JsonSerializable<ManastoneActivationWrap>
    {
        /// <summary>
        /// DateOfExpiry
        /// </summary>
        public DateTime DateOfExpiry;

        /// <summary>
        /// programUUID
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public string programUUID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dateOfExpiry">dateOfExpiry</param>
        /// <param name="programUuid">programUuid</param>
        public ManastoneActivationWrap(DateTime dateOfExpiry, string programUuid)
        {
            DateOfExpiry = dateOfExpiry;
            programUUID = programUuid;
        }

        /// <summary>
        /// DecryptAndDeserialize
        /// </summary>
        /// <param name="s">string to Deserialize</param>
        /// <param name="key">password</param>
        /// <param name="iv">Initialization vector</param>
        /// <returns></returns>
        public static ManastoneActivationWrap DecryptAndDeserialize(string s, string key, string iv)
        {
            return DeSerialize(de.fearvel.net.Security.Crypto.SimpleDES.Decrypt(s, key, iv));
        }
    }
}