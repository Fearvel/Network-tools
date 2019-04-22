using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace de.fearvel.net.Security
{
   public class Ident
    {

        // ReSharper disable once InconsistentNaming
        public static string GetCPUId()
        {
            string cpuid = string.Empty;
            ManagementClass man = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = man.GetInstances();
            foreach (ManagementObject mob in moc)
            {
                if (cpuid == "")
                {
                    cpuid = mob.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            return cpuid;
        }


        public static Version GetFileVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return Version.Parse(version);
        }

    }
}
