using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ftpConsoleClient.Methods
{
    /// <summary>
    /// Implemets FTP CHD method
    /// </summary>
    public class ChangeDirectory : AbstractFtpMethod
    {
        /// <summary>
        /// Send FTP CHD request
        /// </summary>
        /// <param name="consoleArgs">Path to directory to follow</param>
        public override void SendRequest(params string[] consoleArgs)
        {
            if (consoleArgs.Length != 1)
            {
                Console.WriteLine("You must specify directory you want to move!");
                return;
            }

            request = CreateFtpRequest(WebRequestMethods.Ftp.PrintWorkingDirectory, ftpUri + consoleArgs[0]);
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                /*if (ftpUri[ftpUri  - 1] != '/' && consoleArgs[0][0] != '/')
                {
                    Console.Write("Wrong path!\n\n");
                    return;
                }*/
                ftpUri = response.ResponseUri;
            }
        }
    }
}
