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

            // If first letter of path is '/' then constructor of Uri class will create uri where hostname is our path (like ftp://path)
            if (consoleArgs[0][0] != '/')
                ftpUri = new Uri(ftpUri, consoleArgs[0]);
            else
                Console.WriteLine("Wrong path!");
        }
    }
}
