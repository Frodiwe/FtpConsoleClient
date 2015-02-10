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
    /// Provides FTP CHD method
    /// </summary>
    public class ChangeDirectory : AbstractFtpMethod
    {
        /// <summary>
        /// Changes directory to specified one on server
        /// </summary>
        /// <param name="consoleArgs">Path to directory to move</param>
        public override void SendRequest(params string[] consoleArgs)
        {
            if (consoleArgs.Length != 1)
            {
                Console.WriteLine("You must specify directory you want to move!");
                return;
            }

            // Adds '/' in the end of string if it hasn't this one yet for correct joining ftpUri and string by Uri constructor 
            string path = consoleArgs[0];
            path += consoleArgs[0][consoleArgs[0].Length - 1] == '/' ? "" : "/";

            // Remove all UNACCEPTABLE!!1!! charcters ('\', '/') from beginning of the path
            while ((path != "") && ((path[0] == '/') || (path[0].ToString() == "\\")))
                path = path.Substring(1);

            // If path is empty then ftpUri doesn't have to change
            if ("" == path)
                return;

            Uri temporaryUri = new Uri(ftpUri, path);

            request = CreateFtpRequest(WebRequestMethods.Ftp.ListDirectoryDetails, temporaryUri);
            
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch(System.Net.WebException)
            {
                Console.Write("Specified directory doesn't exists!\n\n");
                return;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            ftpUri = temporaryUri;
        }
    }
}
