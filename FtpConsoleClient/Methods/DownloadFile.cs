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
    /// This class provides FTP RETR method in 2 implementations:
    ///     default (downloads and shows file) and with saving downloaded file
    /// </summary>
    public class DownloadFile : AbstractFtpMethod
    {
        /// <summary>
        /// Provides FTP RETR method without saving downloaded file (just show)
        /// </summary>
        /// <param name="consoleArgs">Filename</param>
        public void SendRequest(string[] consoleArgs)
        {
            try
            {
                request = CreateFtpRequest(WebRequestMethods.Ftp.DownloadFile, ftpUri + "/" + consoleArgs[0]);
            }
            catch (System.IndexOutOfRangeException)
            {
                Console.WriteLine("\tYou must specify file name!");
                return;
            }

            Console.Write("Connecting to {0}...\n\n", ftpUri);

            FtpWebResponse response = null;

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException e)
            {
                Console.Write("Error: Couldn't find file {0} at {1}\nShow full message? (y/n) ", consoleArgs[0], ftpUri);

                // if expception user can view one's messsage by typing 'y'
                if ('y' == (char)Console.Read()) Console.Write("{0}\n", e);

                Console.WriteLine();
                return;
            }

            using (response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        // shows downloaded file on the screen
                        Console.WriteLine(reader.ReadToEnd());
                        Console.WriteLine("Download complete, status {0}", response.StatusDescription);
                    }
                }
            }
        }

        /// <summary>
        /// Provides FTP RETR method with saving downloaded file
        /// </summary>
        /// <param name="consoleArgs">Filename and path to save</param>
        public void SendRequestAndSafe(string[] consoleArgs)
        {
            // this function takes not less than 2 params
            if (consoleArgs.Length < 2)
            {
                Console.WriteLine("\tYou must specify file name and the path to safe!");
                return;
            }

            request = CreateFtpRequest(WebRequestMethods.Ftp.DownloadFile, ftpUri + "/" + consoleArgs[0]);
            Console.Write("Connecting to {0}...\n\n", ftpUri);

            FtpWebResponse response = null;

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException e)
            {
                Console.Write("Error: Couldn't find file {0} at {1}\nShow full message? (y/n) ", consoleArgs[0], ftpUri);

                if ('y' == (char)Console.Read()) Console.Write("{0}\n", e);

                Console.WriteLine();
                return;
            }

            using (response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        try
                        {
                            using (StreamWriter file = new StreamWriter(consoleArgs[1] + consoleArgs[0]))
                            {
                                // safe file to specified directory
                                file.Write(reader.ReadToEnd());
                                Console.Write("Download complete, status {0}", response.StatusDescription);
                                Console.Write("File {0} successfully saved at {1}\n\n", consoleArgs[0], consoleArgs[1]);
                            }
                        }
                        catch (System.IO.DirectoryNotFoundException)
                        {
                            Console.Write("Directory {0} not found!\n\n", consoleArgs[1] + consoleArgs[0]);
                        }
                    }
                }
            }
        }
    }
}
