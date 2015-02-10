using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

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

            Regex uriValidation = new Regex("([^\\?#]*)");
            if (uriValidation.IsMatch(consoleArgs[0]))
                ftpUri = new Uri(ftpUri, consoleArgs[0]);
            else
                Console.WriteLine("Wrong path!");
        }
    }
}
