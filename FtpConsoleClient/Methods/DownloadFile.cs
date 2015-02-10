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
    /// default (downloads and shows file) and with saving downloaded file
    /// </summary>
    public class DownloadFile : AbstractFtpMethod
    {
        /// <summary>
        /// Method chooses between private methods based on number of parameters.
        /// If method is gotten 2 params it invokes Save private method.
        /// If method is gotten 1 param it invokes Show private method
        /// </summary>
        /// <param name="consoleArgs">Filename and path to save or only filename</param>
        public override void SendRequest(params string[] consoleArgs)
        {
            if (0 == consoleArgs.Length)
            {
                Console.WriteLine("Not enough parameters!\nIf you only want to show the file you have to specify filename.\n" + 
                                  "If you want to save file you have to specify filename and path to save.");
                return;
            }

            if (consoleArgs.Length > 2)
            {
                Console.WriteLine("Too many arguments!\nIf you only want to show the file you have to specify filename.\n" +
                                  "If you want to save file you have to specify filename and path to save.");
                return;
            }
            else
            {
                if (2 == consoleArgs.Length)
                    Save(consoleArgs[0], consoleArgs[1]);
                else
                    Show(consoleArgs[0]);
                
                return;
            }
        }

        /// <summary>
        /// Provides FTP RETR method with saving downloaded file
        /// </summary>
        /// <param name="consoleArgs">Filename and path to save</param>
        private void Save(string filename, string path)
        {
            request = CreateFtpRequest(WebRequestMethods.Ftp.DownloadFile, new Uri(ftpUri, filename));
            Console.Write("Connecting to {0}...\n\n", ftpUri);

            FtpWebResponse response = null;

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException e)
            {
                Console.Write("Error: Couldn't find file {0} at {1}\nShow full message? (y/n) ", filename, ftpUri);

                if ('y' == (char)Console.Read()) Console.Write("{0}\n", e);

                Console.WriteLine();
                Console.ReadLine();

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
                            using (StreamWriter file = new StreamWriter(path + filename))
                            {
                                // save file to specified directory
                                file.Write(reader.ReadToEnd());
                                Console.Write("Download complete, status {0}", response.StatusDescription);
                                Console.Write("File {0} successfully saved at {1}\n\n", filename, path);
                            }
                        }
                        catch (System.IO.DirectoryNotFoundException)
                        {
                            Console.Write("Directory {0} not found!\n\n", path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Provides FTP RETR method with showing downloaded file (without saving)
        /// </summary>
        /// <param name="consoleArgs">Filename</param>
        private void Show(string filename)
        {
            request = CreateFtpRequest(WebRequestMethods.Ftp.DownloadFile, new Uri(ftpUri, filename));

            Console.Write("Connecting to {0}...\n\n", ftpUri);

            FtpWebResponse response = null;

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException e)
            {
                Console.Write("Error: Couldn't find file {0} at {1}\nShow full message? (y/n) ", filename, ftpUri);

                // if expception user can view one's messsage by typing 'y'
                if ('y' == (char)Console.Read()) Console.Write("{0}\n", e);

                Console.WriteLine();
                Console.ReadLine();

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
    }
}
