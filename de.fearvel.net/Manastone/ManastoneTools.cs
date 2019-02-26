using System.Management;

namespace de.fearvel.net.Manastone
{
    /// <summary>
    /// Tools for the Manastone DRM
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public static class ManastoneTools
    {
        /// <summary>
        /// Delivers the Processor1Id of the pc used on
        /// </summary>
        /// <returns>processor1Id</returns>
        public static string GetHardwareId()
        {
            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            var mbsList = mbs.Get();
            var id = "";
            foreach (var mo in mbsList)
            {
                id = mo["ProcessorId"].ToString();
                break;
            }

            return id;
        }
    }
}