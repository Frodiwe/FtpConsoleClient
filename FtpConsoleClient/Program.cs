using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace ftpConsoleClient
{
    class Program
    {
        private delegate void Command(params string[] consoleArguments);

        // dictionary for console commands
        static Dictionary<string, Command> commands;

        // add new command in dictionary
        static void DefineOperation(string cmd, Command body)
        {
            // if command already exists then throws exception
            if (commands.ContainsKey(cmd))
                throw new ArgumentException(string.Format("Operation {0} already exists", cmd), "op");
            commands.Add(cmd, body);
        }

        static string[] ParseArguments(string currentCommand)
        {
            // separate command arguments
            string[] currentCommandAndArguments = currentCommand.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            string[] currentArguments = null;

            // if command wasn't specified
            try
            {
                currentCommand = currentCommandAndArguments[0];
            }
            catch (System.IndexOutOfRangeException)
            {
                currentCommand = "";
            }

            // if command doesn't have arguments
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

        static void Main(string[] args)
        {
            string uri, username, password;
            string currentCommand;

            Console.Write("Welcome to console ftp client!\n\nEnter ftp server you want to use (without ftp:// routine) (ftp.mozilla.org as default): ");
            uri = Console.ReadLine();
            Console.Write("Enter username: ");
            username = Console.ReadLine();
            Console.Write("Enter password: ");
            password = Console.ReadLine();

            FtpClient ftpClient = new FtpClient(uri, username, password);

            // filling dictionary with commands
            // first param is key=command, second param is func which processes this command
            commands = new Dictionary<string, Command>
            {
                { "help", ftpClient.GetCommandsHelp },
                { "exit", (dummy) => Environment.Exit(0) },
            };
            DefineOperation("ls", ftpClient.GetListDerictory);
            DefineOperation("cat", ftpClient.DownloadAndShowFile);
            DefineOperation("dl", ftpClient.DownloadAndSafeFile);
            DefineOperation("cd", ftpClient.ChangeDirectory);

            Console.WriteLine();
            commands["ls"]();

            // program main loop
            // processes commands until 'exit' found
            do
            {
                Console.Write("{0}>", ftpClient.FtpUri);
                currentCommand = Console.ReadLine();

                string[] currentArguments = ParseArguments(currentCommand);

                // calls the appropriate func 
                if (commands.ContainsKey(currentCommand))
                    commands[currentCommand](currentArguments);
            } while (currentCommand != "exit");

            Console.ReadKey();
        }
    }
}
