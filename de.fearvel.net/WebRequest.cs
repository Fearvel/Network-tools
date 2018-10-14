using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace de.fearvel.net
{
   public static class WebRequest
    {
        public static MemoryStream Download(string url)
        {
            MemoryStream ms;
                WebClient client = new WebClient();
                try
                {
                   ms = new MemoryStream(client.DownloadData(url));
                }
                finally
                {
                    client.Dispose();
                }
            
            return ms;
        }

        public static void DownloadToFile(string url, string path)
        {
            var ms = Download(url);
            var f = File.OpenWrite(path);
            ms.CopyTo(f);
            ms.Close();
            f.Close();
        }
    }
}
