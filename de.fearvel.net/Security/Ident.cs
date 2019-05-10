using System;
using System.Diagnostics;
using System.Management;

namespace de.fearvel.net.Security
{
    /// <summary>
    /// Class for identifying devices and more
    /// </summary>
   public class Ident
    {

        /// <summary>
        /// Returns the first CPU ID of the installed processer
        /// uses System.Management to get this information
        /// </summary>
        /// <returns>cpu1 id as string</returns>
        // ReSharper disable once InconsistentNaming
        public static string GetCPUId()
        {
            string cpuid = string.Empty;
            ManagementClass man = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = man.GetInstances();
            foreach (var o in moc)
            {
                var mob = (ManagementObject) o;
                if (cpuid == "")
                {
                    cpuid = mob.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            return cpuid;
        }

        /// <summary>
        /// Returns the fileVersion of this assembly
        /// uses System.Diagnostics
        /// </summary>
        /// <returns>file version of this assembly</returns>
        public static Version GetFileVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return Version.Parse(version);
        }

    }
}
