using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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

        public static string SendPostRequest(List<RequestDataPackage> data, string serverUrl, bool trustedCertificateOnly = true)
        {
            using (WebClient client = new WebClient())
            {
                if (!trustedCertificateOnly)
                    ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
                NameValueCollection postData = new NameValueCollection();
                foreach (var var in data)
                {
                    postData.Add(var.Identifier, var.Data);
                }
                return Encoding.UTF8.GetString(client.UploadValues(serverUrl, postData));
            }
        }

        private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
        {
            // all Certificates are accepted
            return true;
        }

        public class RequestDataPackage
        {
            public string Identifier;
            public string Data;

            public RequestDataPackage(string identifier, string data)
            {
                Identifier = identifier;
                Data = data;
            }
            public RequestDataPackage() { }
        }

    }
}
