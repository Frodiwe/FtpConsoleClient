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
        protected Uri ftpUri = new Uri("ftp://mozilla.org");

        protected FtpWebRequest request;

        // Default credentials
        protected NetworkCredential credentials = new NetworkCredential("","");

        /// <summary>
        /// Creates and initializes an instance of FtpWebRequest
        /// </summary>
        /// <param name="method">FTP method</param>
        /// <param name="uri">Where we send request</param>
        /// <returns>Instanse of FtpWebRequest</returns>
        protected FtpWebRequest CreateFtpRequest(string method, string uri)
        {
            FtpWebRequest requestInstance = (FtpWebRequest)WebRequest.Create(uri);
            requestInstance.Method = method;
            requestInstance.Credentials = credentials;

            return requestInstance;
        }

        public abstract void SendRequest(params string[] consoleArgs);
    }
}
