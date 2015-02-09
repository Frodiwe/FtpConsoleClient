using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ftpConsoleClient.Methods
{
    // Abstract FTP method
    public abstract class AbstractFtpMethod
    {
        // Default uri for all derived classes
        protected Uri ftpUri = new Uri("ftp://mozilla.org");

        protected FtpWebRequest request;

        protected NetworkCredential credentials;
    }
}
