using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using ftpConsoleClient.Infrastructure;
using ftpConsoleClient.Methods;

namespace ftpConsoleClient
{
    class Program
    {
        private delegate void Command(params string[] consoleArguments);

        // dictionary for console commands
        static Dictionary<string, Command> commands;

        // add new command in dictionary
        static void DefineOperation(string cmd, FtpMethodFactory factory)
        {
            // Define abstract ftp method
            // And add function to dictionary implements abstract ftp method
            // with concrete ftp method from methods factory
            AbstractFtpMethod method = factory.GetInstance(cmd);

            // if command already exists then throws exception
            if (commands.ContainsKey(cmd))
                throw new ArgumentException(string.Format("Operation {0} already exists", cmd), "op");
            commands.Add(cmd, method.SendRequest);
        }

        static string[] ParseArguments(ref string currentCommand)
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

            if (uri != "")
                AbstractFtpMethod.FtpUri = new Uri(uri);
            AbstractFtpMethod.Reloggin(username, password);

            // Creating factory of ftp methods
            FtpMethodFactory factory = new FtpMethodFactory();

            // Register all implemented ftp methods
            factory.Register<DirectoryList>("ls");
            factory.Register<DownloadFile>("dl");
            factory.Register<ChangeDirectory>("cd");

            // Filling dictionary with commands
            // First param is key=command, second param is func which processes this command
            // Dictionary consists of delegate to provide flexibility. You can pass both simple
            // one-line method like 'exit' and whole method of some class.
            commands = new Dictionary<string, Command>
            {
                { "exit", (dummy) => Environment.Exit(0) },
            };

            string[] aliases = new string[] { "ls", "dl", "cd" };

            // Add aliases and its fucntions to commands dictionary
            foreach (string alias in aliases)
                DefineOperation(alias, factory);

            Console.WriteLine();
            commands["ls"]();

            // program main loop
            // processes commands until 'exit' found
            do
            {
                Console.Write("{0}>", AbstractFtpMethod.FtpUri);
                currentCommand = Console.ReadLine();

                string[] currentArguments = ParseArguments(ref currentCommand);

                // calls the appropriate func 
                if (commands.ContainsKey(currentCommand))
                    commands[currentCommand](currentArguments);
            } while (currentCommand != "exit");

            Console.ReadKey();
        }
    }
}
