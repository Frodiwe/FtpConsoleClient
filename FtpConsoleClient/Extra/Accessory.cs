using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ftpConsoleClient.Infrastructure;
using ftpConsoleClient.Methods;

namespace ftpConsoleClient.Extra
{
    /// <summary>
    /// Some extra functions 
    /// </summary>
    public static class Accessory
    {
        /// <summary>
        /// Separates console line to massive of arguments and command
        /// </summary>
        /// <param name="currentCommand">Console line. In this variable will be only command after processing </param>
        /// <returns>Massive of arguments</returns>
        public static string[] ParseArguments(ref string currentCommand)
        {
            // Separate command arguments
            string[] currentCommandAndArguments = currentCommand.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            string[] currentArguments = null;

            // If command wasn't specified
            try
            {
                currentCommand = currentCommandAndArguments[0];
            }
            catch (System.IndexOutOfRangeException)
            {
                currentCommand = "";
            }

            // If command doesn't have arguments
            try
            {
                currentArguments = currentCommandAndArguments[1].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (System.IndexOutOfRangeException)
            {
                currentArguments = new string[0];
            }

            return currentArguments;
        }

        /// <summary>
        /// Welcome message. Also some params recieved from user to fill in AbstractFtpMethod class
        /// </summary>
        public static void WelcomeMessageAndInitParameters()
        {
            string uri, username, password;

            Console.Write("Welcome to console ftp client!\n\nEnter ftp server you want to use (without ftp://) (ftp.mozilla.org as default): ");
            uri = Console.ReadLine();
            Console.Write("Enter username: ");
            username = Console.ReadLine();
            Console.Write("Enter password: ");
            password = Console.ReadLine();

            if (uri != "")
                AbstractFtpMethod.FtpUri = new Uri(uri, UriKind.RelativeOrAbsolute);
            AbstractFtpMethod.Reloggin(username, password);
        }
    }
}
