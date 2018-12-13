using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace de.fearvel.net
{
    /// <summary>
    /// Simple WebRequests
    /// </summary>
    public static class WebRequest
    {
        /// <summary>
        /// Downloads file to MemoryStream
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Downloads file to the specified path
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        public static void DownloadToFile(string url, string path)
        {
            var ms = Download(url);
            var f = File.OpenWrite(path);
            ms.CopyTo(f);
            ms.Close();
            f.Close();
        }

        /// <summary>
        /// Sends a Post Request
        /// </summary>
        /// <param name="data"></param>
        /// <param name="serverUrl"></param>
        /// <param name="trustedCertificateOnly"></param>
        /// <returns></returns>
        public static string SendPostRequest(Dictionary<string, string> data, string serverUrl, bool trustedCertificateOnly = true)
        {
            using (WebClient client = new WebClient())
            {
                if (!trustedCertificateOnly)
                    ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
                NameValueCollection postData = new NameValueCollection();
                foreach (var itm in data)
                {
                    postData.Add(itm.Key, itm.Value);
                }
                return Encoding.UTF8.GetString(client.UploadValues(serverUrl, postData));
            }
        }

        /// <summary>
        /// Function to bypass certificate validity checks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="x509Certificate"></param>
        /// <param name="x509Chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private static bool TrustCertificate(object sender, X509Certificate x509Certificate,
            X509Chain x509Chain, SslPolicyErrors sslPolicyErrors) => true; // all Certificates are accepted
    }
}