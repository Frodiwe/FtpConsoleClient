using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ftpConsoleClient.Methods
{
    /// <summary>
    /// Abstract FTP method
    /// </summary>
    public abstract class AbstractFtpMethod
    {
        // Default uri for all derived classes
        static protected Uri ftpUri = new Uri("ftp://ftp.mozilla.org/");

        // Request used in all derived classes to send request to server
        protected FtpWebRequest request;

        // Default credentials
        static protected NetworkCredential credentials = new NetworkCredential("","");

        /// <value>
        /// Simple smart field for ftpUri
        /// </value>
        static public Uri FtpUri
        {
            get { return ftpUri; }
            set 
            {
                if (!value.IsAbsoluteUri)
                {
                    UriBuilder uriBuilder = new UriBuilder(Uri.UriSchemeFtp, value.ToString());
                    ftpUri = uriBuilder.Uri;
                }
            }
        }

        /// <summary>
        /// Method to reauthorize in the ftp server
        /// </summary>
        /// <param name="username">New username</param>
        /// <param name="password">Password to this username</param>
        static public void Reloggin(string username, string password)
        {
            credentials.UserName = username;
            credentials.Password = password;
        }

        /// <summary>
        /// Creates and initializes an instance of FtpWebRequest
        /// </summary>
        /// <param name="method">FTP method</param>
        /// <param name="uri">Uri to send request</param>
        /// <returns>Instanse of FtpWebRequest</returns>
        protected FtpWebRequest CreateFtpRequest(string method, Uri uri)
        {
            FtpWebRequest requestInstance = (FtpWebRequest)WebRequest.Create(uri);
            requestInstance.Method = method;
            requestInstance.Credentials = credentials;

            return requestInstance;
        }

        /// <summary>
        /// Method which sends request to server and processes the response
        /// </summary>
        /// <param name="consoleArgs">Any arguments from console required in the request</param>
        public abstract void SendRequest(params string[] consoleArgs);
    }
}
